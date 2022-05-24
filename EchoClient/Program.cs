using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoClient
{
	class MainApp
	{
		static void Main(string[] args)
		{
#if DEBUG
			args = new[] { "210.119.12.69", "10254", "210.119.12.69", "hello" };     // 디버그 할때 args에 자동으로 값이 입력됨 = F5, ctrl+F5 가능
#endif
			if (args.Length < 4)
			{
				Console.WriteLine("사용법 : {0} <Bind IP> <Bind Port> <Server IP> <Message>", Process.GetCurrentProcess().ProcessName);
				return;
			}

			string bindIp = args[0];                    // 클라이언트 IP					 // 루프백 : 127.0.0.1  =  외부가 아닌 본인 스스로와의 내부통신을 위한 루프IP
			int bindPort = Convert.ToInt32(args[1]);	// 클라이언트 포트번호			 // 1024 이상을 부여(미만은 Well Known Port로 주요 프로토콜/프로그램등이 사용)
			string serverIp = args[2];                  // 서버 IP			- 일치해야함	 // 루프백 : 127.0.0.1  =  외부가 아닌 본인 스스로와의 내부통신을 위한 루프IP
			const int serverPort = 5425;				// 서버의 포트번호	- 일치해야함
			string message = args[3];					// 서버로 송신할 메세지

			try
			{
				IPEndPoint clientAddress = new IPEndPoint(IPAddress.Parse(bindIp), bindPort);		// 클라이언트IP 및 포트를 통한 클라이언트 주소 생성
				IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);   // 서버IP 및 포트를 통한 서버 주소 생성

				Console.WriteLine("클라이언트 : {0}, 서버 : {1}",clientAddress.ToString(),serverAddress.ToString());

				// 생서된 주소를 가진 클라이언트 생성
				TcpClient client = new TcpClient(clientAddress);

				// 서버 주소에 해당하는 서버와 연결시도
				client.Connect(serverAddress);

				// NetworkStream을 생성하고 client에게 Stream소유권?권한?을 부여함
				NetworkStream stream = client.GetStream();

				byte[] data = Encoding.Default.GetBytes(message);	 // 송신할 메세지를 byte형으로 인코딩해 data에 저장				 
				stream.Write(data, 0, data.Length);					 // 생성된 데이터를 넘겨줌

				// 넘겨줬음을 출력
				Console.WriteLine("송신: {0}", message);

				// data초기화
				data = new byte[256];

				string responseData = "";

				// NetworkStream을 통해 서버가 응답해 보낸 정보를 읽어서 data에 저장
				int bytes = stream.Read(data, 0, data.Length);
				responseData = Encoding.Default.GetString(data, 0, bytes);      // 받은 데이터를 responseData에 저장함(분류,보관 목적?)

				Console.WriteLine("수신: {0}", responseData);     // 수신했음을 확인

				stream.Close();			// NetworkStream 닫음
				client.Close();			// 클라이언트 닫음
			}
			catch (SocketException e)
			{
				Console.WriteLine(e);
			}

			Console.WriteLine("클라이언트를 종료합니다.");
		}
	}
}
