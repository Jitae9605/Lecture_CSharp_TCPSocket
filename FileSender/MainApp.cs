using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using FUP;

namespace FileSender                // 파일 송신 = 클라이언트
{
	class MainApp
	{
		const int CHUNK_SIZE = 4096;		// 파편 하나의 바이트크기

		static void Main(string[] args)
		{
#if DEBUG
			args = new[] { "127.0.0.1", "answer.zip" };		// 디버그 할때 args에 자동으로 값이 입력됨
#endif
			if (args.Length < 2)                            // args가 입력되지 않으면 사용법이 팝업
			{
				Console.WriteLine("사용법 : {0} <Server IP> <File Path>", Process.GetCurrentProcess().ProcessName);
				return;
			}

			string serverIp = args[0];			// 서버IP
			const int serverPort = 5425;		// 서버포트번호
			string filepath = args[1];			// 전송할 파일의 경로

			try
			{
				// 로컬IP주소 생성 - IP주소를 0으로 하면 OS에 할당된 어떤 주소로도 서버에 접속이 가능(당연히 127.0.0.1 포함)
				//				  - port번호를 0으로 하면 OS가 자동 할당
				IPEndPoint clientAddress = new IPEndPoint(0, 0);
				IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);	// Parse = string형을 IpAddress로 바꿔준다

				Console.WriteLine("클라이언트 : {0}, 서버 : {1}", clientAddress.ToString(), serverAddress.ToString());

				uint msgID = 0;

				// 파일전송요청 메세지 생성
				Message reqMsg = new Message();
				reqMsg.Body = new BodyRequest()							// 바디작성
				{
					FILESIZE = new FileInfo(filepath).Length,			// 전송할 파일의 크기
					FILENAME = Encoding.Default.GetBytes(filepath)		// 전송할 파일의 경로를 byte화 해서 저장
				};

				reqMsg.Header = new Header()					// 헤더작성
				{
					MSGID = msgID++,                            // 요청메세지의 식별번호
					MSGTYPE = CONSTRAINT.REQ_FILE_SEND,         //  = REQ_FILE_SEND
					BODYLEN = (uint)reqMsg.Body.GetSize(),		// 앞에서 작성한 요청메세지의 body부분의 크기
					FRAGMENTED = CONSTRAINT.NOT_FRAGMENTED,		// body의 크기가 작으므로 분할할 이유도 가능성도 없으므로  = NOT_FRAGMENTED
					LASTMSG = CONSTRAINT.LASTING,				// 분할안했으므로  = LASTING
					SEQ = 0										// 분할안했으므로  = 0
				};

				
				TcpClient client = new TcpClient(clientAddress); // TcpClient 생성 및 정보입력 
				client.Connect(serverAddress);                   // TcpServer(serverAddress)인 곳에 접속요청

				NetworkStream stream = client.GetStream();      // NetworkStream을 선언하고 이를 client에게 배정

				MessageUtil.Send(stream, reqMsg);				// 작성한 요청메세지 송신

				Message rspMsg = MessageUtil.Receive(stream);   // 서버가 보내준 답변메세지를 NetworkStream에서 읽어와 rspMsg에 저장

				// rspMsg의 MSGTYPE가 REP_FILE_SEND인지 판단
				if (rspMsg.Header.MSGTYPE != CONSTRAINT.REP_FILE_SEND)
				{
					Console.WriteLine("정상적인 서버 응답이 아닙니다.{0}",rspMsg.Header.MSGTYPE);
					return;
				}

				// RESPONSE가 DENIED / ACCEPTED 인지 판단
				if (((BodyResponse)rspMsg.Body).RESPONSE == CONSTRAINT.DENIED)
				{
					// RESPONSE = DENIED
					Console.WriteLine("서버에서 파일 전송을 거부했습니다.");
					return;
				}


				// RESPONSE = ACCEPTED
				// 파일전송을 위한 메세지 작성
				using (Stream fileStream = new FileStream(filepath,FileMode.Open))	// 전송대상 파일열기
				{
					// 초기화
					byte[] rbytes = new byte[CHUNK_SIZE];	// 전송대상파일의 데이터 저장할 공간

					long readValue = BitConverter.ToInt64(rbytes, 0);

					int totalRead = 0;
					ushort msgSeq = 0;
					byte fragmented = (fileStream.Length < CHUNK_SIZE) ? CONSTRAINT.NOT_FRAGMENTED : CONSTRAINT.FAGMENTED;	// 대상파일의 크기를 통한 파편화여부 결정 
					while(totalRead < fileStream.Length)		// 총 읽은 파일의 데이터크기가 대상의 크기보다 작은동안 == 아직 전송할게 남았으면 반복
					{
						int read = fileStream.Read(rbytes, 0, CHUNK_SIZE);  // 파일의 데이터를 CHUNK_SIZE만큼 읽어와 rbytes에 저장하고
																			// 읽은데이터의 크기(rbytes에 저장된 데이터의 크기)를 반환해 read에 저장

						totalRead += read;									// 총 칡은 파일크기 누적
						Message fileMsg = new Message();

						byte[] sendBytes = new byte[read];                  // 보낼 데이터를 저장할 sendBytes를 선언하고 그 크기를 read로 한다.
						Array.Copy(rbytes, 0, sendBytes, 0, read);          // rbytes의 데이터를 sendBytes에 read개 저장

						fileMsg.Body = new BodyData(sendBytes);             // fileMsg 바디생성
						fileMsg.Header = new Header()                       // fileMsg 헤더생성
						{
							MSGID = msgID,                                                                              // 요청메세지의 식별번호
							MSGTYPE = CONSTRAINT.FILE_SEND_DATA,                                                        //  = FILE_SEND_DATA
							BODYLEN = (uint)fileMsg.Body.GetSize(),                                                     // fileMsg의 body부분의 크기
							FRAGMENTED = fragmented,																	// 앞에서의 파편화 여부 판단결과 
							LASTMSG = (totalRead < fileStream.Length) ? CONSTRAINT.NOT_LASTING : CONSTRAINT.LASTING,    // 총 읽은 파일의 데이터크기가 대상의 크기보다 작으면 아직 전송할게 남았으므로 NOT_LASTING
																														// 아니면 이게 마지막 파편이므로 LASTING

							SEQ = msgSeq++																				// 파편번호
						};

						// 파편화 성공시 출력됨
						Console.Write("#");

						// 파편 전송
						MessageUtil.Send(stream, fileMsg);
					}

					Console.WriteLine();

					// 파일전송결과 메세지 수신
					Message rstMsg = MessageUtil.Receive(stream);

					BodyResult result = ((BodyResult)rstMsg.Body);

					// 파일전송결과 (성공여부)출력 (True / False)
					Console.WriteLine("파일전송 성공 : {0}", result.RESULT == CONSTRAINT.SUCCESS);
				}

				// 전송 끝났으므로 클라이언트, 스트림 접속 끊음
				stream.Close();
				client.Close();
			}
			catch(SocketException e)
			{
				// 오류 발생시의 오류를 출력
				Console.WriteLine(e);
			}

			Console.WriteLine("클라이언트를 종료합니다.");
		}
	}
}
