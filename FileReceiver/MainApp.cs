using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FUP;

namespace FileReceiver			// 파일 수신 = 서버
{
	class MainApp
	{
		static void Main(string[] args)
		{
#if DEBUG
			args = new[] { "upload" };		// 디버그 할때 args에 자동으로 값이 입력됨
#endif
			if (args.Length < 1)			// args가 입력되지 않으면 사용법이 팝업
			{
				Console.WriteLine("사용법 : {0} <Directory>", Process.GetCurrentProcess().ProcessName);
				return;
			}

			// 초기화
			uint msgId = 0;
			string dir = args[0];

			// 입력된 디렉토리 검사
			if (Directory.Exists(dir) == false)		// 경로에 해당 이름의 디렉토리가 없다면
				Directory.CreateDirectory(dir);		// 그 디렉토리를 경로에 생성

			const int bindPort = 5425;				// 서버포트번호
			TcpListener server = null;				// Tcp서버 선언

			try
			{
				IPEndPoint localAddress = new IPEndPoint(0, bindPort);	// 로컬IP주소 생성 - IP주소를 0으로 하면 OS에 할당된 어떤 주소로도 서버에 접속이 가능(당연히 127.0.0.1 포함)
				server = new TcpListener(localAddress);					// 생성한 로컬 IP주소를 토대로 서버를 재정의
				server.Start();											// 서버 시작

				Console.WriteLine("파일 업로드 서버 시작...");

				while(true)
				{
					TcpClient client = server.AcceptTcpClient();        // client가 server에게 보낸 접속허가요청(Connet)을 승인/접속허가 함
					Console.WriteLine("클라이언트 접속 : {0}", ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

					NetworkStream stream = client.GetStream();          // NetworkStream을 선언하고 이를 client에게 배정

					Message reqMsg = MessageUtil.Receive(stream);       // client가 네트워크스트림에 업로드한 메세지를 reqMsg에 저장

					if (reqMsg.Header.MSGTYPE != CONSTRAINT.REQ_FILE_SEND)  // 수신한 메세지(reqMsg)가 파일전송요청이 아닌경우
					{
						// 현재의 stream, client를 닫고 다시 클라이언트 접속 대기
						stream.Close();
						client.Close();
						continue;
					}

					// 수신한 메세지(reqMsg)가 파일전송요청인 경우
					BodyRequest reqBody = (BodyRequest)reqMsg.Body;

					Console.WriteLine("파일 업로드 요청이 왔습니다. 수락하시겠습니까? yes/no");
					string answer = Console.ReadLine(); // yes / no를 입력해야함

					// 입력된 yes / no 에 상관없이 답변메세지는 보내야하므로 메세지를 작성(단, RESPONSE의 값이 다름)
					// 일단 yes로 가정하고 메시지를 작성(밑에서 아닌 경우를 판별해 메세지를 재작성(덮어쓰기) 한다.)
					Message rspMsg = new Message();

					rspMsg.Body = new BodyResponse()			// 답변메세지 바디작성
					{
						MSGID = reqMsg.Header.MSGID,			// 전송요청 메세지의 MSGID를 가져다쓴다.(어떤 메세지에대한 대답인지를 보여주는것)
						RESPONSE = CONSTRAINT.ACCEPTED			//  = ACCEPTED
					};

					rspMsg.Header = new Header()					// 답변메세지 헤더작성
					{
						MSGID = msgId++,							// 답변메세지의 식별번호
						MSGTYPE = CONSTRAINT.REP_FILE_SEND,         //  = REP_FILE_SEND
						BODYLEN = (uint)rspMsg.Body.GetSize(),		// 앞에서 작성한 답변메세지의 body부분의 크기
						FRAGMENTED = CONSTRAINT.NOT_FRAGMENTED,     // body의 크기가 작으므로 분할할 이유도 가능성도 없으므로  = NOT_FRAGMENTED
						LASTMSG = CONSTRAINT.LASTING,               // 분할안했으므로  = LASTING
						SEQ = 0										// 분할안했으므로  = 0
					};

					if (answer != "yes")					// no인 경우  = 메세지 덮어쓰기 및 송신후 현재 연결들 종료하고 다시 클라이언트 접속대기상태로 돌아감
					{
						rspMsg.Body = new BodyResponse()
						{
							MSGID = reqMsg.Header.MSGID,    // 전송요청 메세지의 MSGID를 가져다쓴다.
							RESPONSE = CONSTRAINT.DENIED    //  = DENIED
						};

						MessageUtil.Send(stream, rspMsg);	// 송신
						stream.Close();
						client.Close();

						continue;                           // 다시 클라이언트 접속대기상태로 돌아감
					}

					else											// yes가 맟는경우
						MessageUtil.Send(stream, rspMsg);			// 이상없다 판단해서 바로 송신진행

					// yes를 보냈으므로 client는 파일전송을 시작함
					Console.WriteLine("파일전송을 시작합니다...");

					// 앞서받은 파일전송요청 메세지에서 전송되어올 파일의 이름과 크기를 가져온다.
					long fileSize = reqBody.FILESIZE;
					string fileName = Encoding.Default.GetString(reqBody.FILENAME);

					// 서버를 열때 지정한 경로에 "\\파일이름"을 붙여 파일을 생성한다.
					FileStream file = new FileStream(dir + "\\" + fileName, FileMode.Create);

					// 데이터를 전송받아 파일에 데이터를 저장
					uint? dataMsgId = null;		// 데이터전송메세지의 식별번호
					ushort prevSeq = 0;			// 분할파일 번호(순서맞추기)

					while ((reqMsg = MessageUtil.Receive(stream)) != null)			// stream에 받아올게 없을때까지 반복
					{
						Console.Write("#");											// 데이터 파편 하나 받아올때 마다 "#"을 출력
						if (reqMsg.Header.MSGTYPE != CONSTRAINT.FILE_SEND_DATA)     // FILE_SEND_DATA가 아닐경우 바로 데이터 전송 탈출
							break;

						if (dataMsgId == null)                                      // 데이터전송메세지의 식별번호가 null인경우 = 데이터전송 처음부분
							dataMsgId = reqMsg.Header.MSGID;						// 파일전송요청메세지의 식별번호를 부여함
						else
						{
							if (dataMsgId != reqMsg.Header.MSGID)                   // dataMsgId가 null도 파일전송요청메세지의 식별번호도 아닌경우
								break;												// 탈출
						}

						if(prevSeq++ != reqMsg.Header.SEQ)								// 데이터 파편이 순서대로 전송된건지 체크
						{
							Console.WriteLine("{0}, {1}", prevSeq, reqMsg.Header.SEQ);	// 일종의 진행상황을 알수있음
							break;														// 순서대로가 아닌경우 바로 탈출
						}

						file.Write(reqMsg.Body.GetBytes(), 0, reqMsg.Body.GetSize());	// 생성한 파일에 데이터를 입력

						if (reqMsg.Header.FRAGMENTED == CONSTRAINT.NOT_FRAGMENTED)      // 헤더의 파편화 여부가 NOT_FRAGMENTED인경우 한번으로 복사가 끝나므로
							break;														// 탈출

						if (reqMsg.Header.LASTMSG == CONSTRAINT.LASTING)                // 헤더의 마지막 파편인지 여부가 LASTING인경우 마지막데이터가 전송되었으므로
							break;														// 탈출
					}

					long recvFileSize = file.Length;									// 복사가 끝난 파일의 크기를 저장
					file.Close();														// 파일 닫음

					Console.WriteLine();
					Console.WriteLine("수신 파일 크기 : {0} bytes", recvFileSize);

					// 파일 처리결과 메세지 생성
					Message rstMsg = new Message();
					rstMsg.Body = new BodyResult()
					{
						MSGID = reqMsg.Header.MSGID,					// 전송요청 메세지의 MSGID를 가져다쓴다 
						RESULT = CONSTRAINT.SUCCESS                     // = SUCCESS
					};

					rstMsg.Header = new Header()
					{
						MSGID = msgId++,								// 답변메세지의 식별번호
						MSGTYPE = CONSTRAINT.FILE_SEND_RES,             // = FILE_SEND_RES
						BODYLEN = (uint)rstMsg.Body.GetSize(),          // Body의 크기
						FRAGMENTED = CONSTRAINT.NOT_FRAGMENTED,			// 파편화X
						LASTMSG = CONSTRAINT.LASTING,					// 파편화없으므로 당연히 처음이자 마지막 메세지
						SEQ = 0											// 파편화 없으므로 0
					};

					if (fileSize == recvFileSize)						// 수신파일크기 == 송신파일크기 == 누락없이 파일전송이 완료됨
						MessageUtil.Send(stream, rstMsg);				
					else
					{
						// 이상이 있는경우
						rstMsg.Body = new BodyResult()
						{
							MSGID = reqMsg.Header.MSGID,
							RESULT = CONSTRAINT.FAIL                    // 경과메세지의 결과를 FAIL로 바꿈
						};

						MessageUtil.Send(stream, rstMsg);				// 그리고 전송
					}

					// 전송 끝났으므로 클라이언트, 스트림 접속 끊음
					Console.WriteLine("파일전송을 마쳤습니다.");

					stream.Close();
					client.Close();
				}
			}
			catch(SocketException e)
			{
				// 오류 발생시의 오류를 출력
				Console.WriteLine(e);
			}
			finally
			{
				// 오류발생해 갑작스러운 종료에도 서버가 살아있어 또다른 오류가 발생하지 않게 서버닫음(포트 재사용)  
				server.Stop();
			}

			Console.WriteLine("서버를 종료합니다.");
		}
	}
}
