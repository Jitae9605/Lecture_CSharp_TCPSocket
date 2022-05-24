using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
	class MainApp
	{
		static void Main(string[] args)
		{
#if DEBUG
			args = new[] { "210.119.12.69" };     // 디버그 할때 args에 자동으로 값이 입력됨 = F5, ctrl+F5 가능
#endif
			// args없으면 사용법 소개
			if (args.Length < 1)
			{
				Console.WriteLine("사용법 : {0} <Bind IP>", Process.GetCurrentProcess().ProcessName);	
				return;
			}

			// args있으면 서버 생성
			string BindIP = args[0];		// 서버 IP			// 루프백 : 127.0.0.1  =  외부가 아닌 본인 스스로와의 내부통신을 위한 루프IP
			const int bindPort = 5425;		// 서버포트번호
			TcpListener server = null;		// TCP소켓서버 생성
			try
			{
				IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse(BindIP), bindPort);

				// 생성된 서버에 IP 및 포트부여
				server = new TcpListener(localAddress);

				// 서버 동작시작
				server.Start();

				Console.WriteLine("메아리 서버 시작...");

				// 클라이언트의 응답/요청 대기
				while(true)
				{
					// 클라이언트의 접속이 있으면 client가 생성되고 server는 접속을 승인한다(현재는 별도 보안없음)
					TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("클라이언트 접속 : {0}", ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

					// NetworkStream을 생성하고 client에게 Stream소유권?권한?을 부여함
					NetworkStream stream = client.GetStream();

					int length;
					string data = null;
					byte[] bytes = new byte[256];

					// NetworkStream의 남은 데이터를 읽어와 byte에 저장한다(이게 0byte일때까지 반복)
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						data = Encoding.Default.GetString(bytes, 0, length);	// byte형으로 bytes에 저장된 내용을 data에 string형으로 인코딩해 저장
						Console.WriteLine(String.Format("수신: {0}", data));		
						
						byte[] msg = Encoding.Default.GetBytes(data);			// string형으로 data에 저장된 내용을 msg에 byte형으로 인코팅해 저장

						stream.Write(msg, 0, msg.Length);                       // NetworkStream에 msg를 넘겨줌
						Console.WriteLine(String.Format("송신: {0}", data));		// data를 보냈음을 출력
					}

					stream.Close();         // NetworkStream 닫음
					client.Close();			// 클라이언트 닫음
				}
			}

			catch(SocketException e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				server.Stop();
			}

			Console.WriteLine("서버를 종료합니다.");
		}
	}
}
