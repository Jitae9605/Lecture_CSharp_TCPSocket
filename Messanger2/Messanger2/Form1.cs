using System;
using System.Windows.Forms;

using System.Net;   // IP Adress
using System.Net.Sockets; // Tcp Listen(server, Client) Class
using System.Threading; // 쓰레드
using System.IO;    // Stream
using Microsoft.Win32;  // 레지스트리 클래스 사용. 별칭등의 간단한 정보저장 및 재사용

// 레지스트리 사용 권한을 줘야함
/*
	1. 프로젝트 속성에서 보안 -> one .... 체크 -> 완전신뢰
	2. 생성된 app.manifest의 Level을
		<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" /> 
	   로 바꾼다
	3. 1.의 체크(one...)를 푼다.
	4. 비주얼스튜디오를 관리자 권한으로 실행해야 경고메시지가 안뜬다.
 */


namespace Messanger2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		

		private TcpListener Server;	// Tcp 통신시 클라이언트의 연결을 수신
		private TcpClient SerClient, Client ;   //  서버측의 클라이언트, 서버에 접속하려는 클라이언트
		private NetworkStream mystream; // 네트워크 스트림(데이터 송수신 통로)
		private StreamReader myRead;    // 읽기 스트림
		private StreamWriter myWrite;   // 쓰기 스트림

		private Boolean Start = false;	// 서버 기동 flag
		private Boolean ClientCon = false;  // 클라이언트 기동 flag

		private int myPort;	// 포트번호
		private string myName;	// 별칭
		private Thread myReader;    // 전송된 테이터의 읽기 쓰레드
		private Thread myServer;    // 서버에 접속하려는 클라이언트를 받아 줌

		private RegistryKey Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramwork",true);
		private Delegate AddText;

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				ServerStop();
				Disconnection();
			}
			catch
			{

				return;
			}
			
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// 레지스트리에 별칭이 등록되어 있는지 체크
			if ((string)Key.GetValue("Message_name") == "")
			{
				this.myName = this.txtId.Text;
				this.myPort = 62000;            // 포트번호는 웰노운포트를 피해서 할것!
			}
			else
			{
				try
				{
					this.myName = (string)Key.GetValue("Message_name");
					this.myPort = 62000;
				}
				catch
				{


				}
			}
		}

		private void 설정ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.설정ToolStripMenuItem.Enabled = false;
			this.plOption.Visible = true;
			this.txtId.Focus();
			this.txtId.Text = (string)Key.GetValue("Message_name"); //별칭
			this.txtPort.Text = "62001"; // 포트 번호
		}

		private void 닫기ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void tsbtnConn_Click(object sender, EventArgs e)
		{
			// 서버용 기동인지 클라이언트 기동인지 구분
			if (this.cbServer.Checked == true) // 서버로 기동
			{
				var addr = new IPAddress(0);

				try
				{
					this.myName = (string)Key.GetValue("Message_name");
				}
				catch
				{
					this.myName = this.txtId.Text;
					this.myPort = Convert.ToInt32(this.txtPort.Text);

				}

				// 서버가 기동상태가 아니면, 서버를 기동
				if(!this.Start)
				{
					try
					{
						Server = new TcpListener(addr, this.myPort);
						Server.Start();

						// 서버 상태 flag를 기동상태로 변경
						this.Start = true;

						// 화면 컨트롤 제어
						this.txtMessage.Enabled = true;
						this.btnSender.Enabled = true;
						this.txtMessage.Focus();
						this.tsbtnConn.Enabled = false;
						this.tsbtnDisconn.Enabled = true;
						this.cbServer.Enabled = false;

						// 서버에 접속하는 클라이언트를 대기 => 쓰레드
						myServer = new Thread(ServerStart);
						myServer.Start();

						this.설정ToolStripMenuItem.Enabled = false;

					}
					catch
					{
						// 접속 에러 Message로 처리
						Invoke(AddText, "서버를 실행할 수 없습니다.");
						
					}
				}
				else
				{
					// 서버종료처리
				}

			}

			else // 클라이언트로 기동
			{
				// 클라이언트가 기동되지 않았는지 => 기동시킹
				if(!(this.ClientCon))
				{
					// 클라이언트 접속 메소드 호출
					ClientConnection();
				}
				else
				{
					this.txtMessage.Enabled = false;
					this.btnSender.Enabled = false;
				}
			}
		}

		private void ClientConnection()
		{
			try
			{
				// 클라이온트가 서버에 접속
				Client = new TcpClient(this.txtIp.Text, this.myPort);

				Invoke(AddText, "서버에 접속했습니다.");

				// 서버와 연결된 통신 통로(스트림) 획득
				mystream = Client.GetStream();

				myRead = new StreamReader(mystream);
				myWrite = new StreamWriter(mystream);

				//컨트롤 제어
				this.ClientCon = true;
				this.tsbtnConn.Enabled = false;
				this.tsbtnDisconn.Enabled = true;
				this.txtMessage.Enabled = true;
				this.btnSender.Enabled = true;
				this.txtMessage.Focus();

				// 수신 쓰레드 생성
				// Receive 메소드는 서버 클라이언트 겸용임.
				myReader = new Thread(Receive);
				myReader.Start();



			}
			catch
			{
				// 서버에 접속 못한 상태를 상태 flag에 반영 후, 메시지로 처리
				this.ClientCon = false;
				Invoke(AddText, "서버에 접속하지 못했습니다.");
			}
		}

		// 서버 종료 및 자원 해제처리
		private void ServerStop()
		{
			this.Start = false;

			// 컨트롤 제어
			this.txtMessage.Enabled = false;
			this.txtMessage.Clear();
			this.btnSender.Enabled = false;
			this.tsbtnConn.Enabled = true;
			this.tsbtnDisconn.Enabled = false;
			this.cbServer.Enabled = true;

			// 클라이언트 접속상태 flag도 false로 변경
			this.ClientCon = false;

			// 자원해제
			// StreadReader 해제
			if (!(myRead == null))
			{
				myRead.Close();
			}
			// StreadWriter 해제
			if (!(myWrite == null))
			{
				myWrite.Close();
			}
			// Network 스트림 해제
			if (!(mystream == null))
			{
				mystream.Close();
			}

			// 서버측 TcpClient 해제
			if (!(SerClient == null))
			{
				SerClient.Close();
			}
			// TcpListner = 서버 해제
			if(!(Server == null))
			{
				Server.Stop();
			}

			// 수신쓰레드 해제
			// 클라이언트 접속담당 쓰레드 해제
			if (!(myReader == null))
			{
				myReader.Join();
			}

			// 서버와 접속 종료되었음을 메시지로 알림
			if(!(AddText == null))
			{
				Invoke(AddText,"서버와 접속이 끊어졌습니다.");
			}

		}

		// 클라이언트의 접속 해제 처리
		private void Disconnection()
		{
			this.ClientCon = false;

			try
			{
				// 스트림 리더 클래스 해제
				if(!(myRead == null))
				{
					myRead.Close();
				}
				// 스트림 쓰는 클래스 해제
				if(!(myWrite == null))
				{
					myWrite.Close();
				}
				// 네트워크 스트림 해제
				if(!(mystream == null))
				{
					mystream.Close();
				}
				// TcpClient 해제
				if(!(Client == null))
				{
					Client.Close();
				}
				// 스레드 종료
				if(!(myReader == null))
				{
					myReader.Join();
				}
			}
			catch
			{

				return;
			}
		}

		// 서버 및 클라이언트가 상대방에게 보내는 메세지를 전송함
		private void Msg_Send()
		{
			try
			{
				// 전송 시간정보 획득
				var dt = Convert.ToString(DateTime.Now);

				// 스트림을 통해서 메세지 전송
				myWrite.WriteLine(this.myName + "&" + this.txtMessage + "&" + dt);
				myWrite.Flush();

				// 리치 텍스트박스에 전송메세지 추가
				MessageView(this.myName + ";" + this.txtMessage.Text);
				this.txtMessage.Clear();
			}
			catch
			{

				Invoke(AddText, "데이터 전송에 문제가 발생했습니다.");
				this.txtMessage.Clear();
			}
		}

		private void MessageView(string strText)
		{
			// 메시지 추가
			this.rtbText.AppendText(strText + "\r\n");
			this.rtbText.Focus();

			// 마지막 위치로 이동
			this.rtbText.ScrollToCaret();
			this.txtMessage.Focus();
		}


		private void ServerStart()
		{
			Invoke(AddText, "서버 실행 : 클라이언트의 접속을 기다립니다.");

			while(Start)
			{
				try
				{
					// 서버에 접속된 클라이언트
					SerClient = Server.AcceptTcpClient();

					Invoke(AddText, "클라이언트가 접속되었습니다..");


					mystream = SerClient.GetStream();

					myRead = new StreamReader(mystream);
					myWrite = new StreamWriter(mystream);

					this.ClientCon = true;

					// 클라이언트가 보내온 데이터를 수신하는 쓰레드;
					myReader = new Thread(Receive);
					myReader.Start();
				}
				catch
				{

					
				}
				
			}
		}

		private void Receive()
		{
			try
			{
				// 클라이언트가 접속 되어 있는 동안 반복
				while(this.ClientCon)
				{
					Thread.Sleep(1);
					// 스트림으로 부터 읽어올 데이터가 있는지 판단
					if(mystream.CanRead)
					{
						var msg = myRead.ReadLine();
						var Smsg = msg.Split('&');

						if(Smsg[0] == "S001")
						{
							// 전송시각화지만 생략!
						}
						else
						{
							if(msg.Length > 0)
							{
								// 서버 및 클라이언트가 보내온 메세지 출력
								Invoke(AddText, Smsg[0] + " : " + Smsg[1]);
							}
						}

						//  수신데이터 처리
					}
				}

			}
			catch
			{

				
			}
		}

		private void tsbtnDisconn_Click(object sender, EventArgs e)
		{

		}

		private void cbServer_CheckedChanged(object sender, EventArgs e)
		{
			if (this.cbServer.Checked)
			{
				// 서버로 기동시 IP는 내부에서 받아와 사용 => 입력막기
				this.txtIp.Enabled = false;
			}
			else
			{
				// 클라이언트로 기동시 IP는 접속할 서버 IP입력
				this.txtIp.Enabled = true;
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (this.cbServer.Checked == true) //서버용으로 사용
			{
				// 입력된 정보를 레지스트리에 등록
				ControlCheck();
			}
			else // 클라이언트용으로 사용
			{
				if (this.txtIp.Text == "")
				{
					this.txtIp.Focus();
				}
				else
				{
					// 입력된 정보를 레지스트리에 등록
					ControlCheck();
				}
			}
		}
		private void ControlCheck()
		{
			if(this.txtId.Text == "")
			{
				this.txtId.Focus();
			}
			else if(this.txtPort.Text == "")
			{
				this.txtPort.Focus();
			}
			else
			{
				// 컨트롤러에 입력된 정보를 레지스트리에 등록
				try
				{
					// 레지스트리에 별칭 및 포트 저장
					var name = this.txtId.Text;
					var port = this.txtPort.Text;
					Key.SetValue("Mesaage_name", "name");
					Key.SetValue("Mesaage_Port", "port");

					// 컨트롤 제어
					this.plOption.Visible = false;
					this.설정ToolStripMenuItem.Enabled = true;
					this.tsbtnConn.Enabled = true;
				}
				catch
				{
					MessageBox.Show("설정 정보가 저장되지 않았습니다.", "에러",MessageBoxButtons.OK,MessageBoxIcon.Error);
					
				}
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.설정ToolStripMenuItem.Enabled = true;
			this.plOption.Visible = false;
			this.txtMessage.Focus();
		}

		private void btnSender_Click(object sender, EventArgs e)
		{
			if (this.txtMessage.Text == "")
			{
				this.txtMessage.Focus();
			}
			else
			{
				Msg_Send();
			}
		}

		private void rtbText_TextChanged(object sender, EventArgs e)
		{

		}

		private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ENTER 키가 입력되면 작동
			if(e.KeyChar == (char)13)
			{
				if (this.txtMessage.Text == "")
				{
					this.txtMessage.Focus();
				}
				else
				{
					Msg_Send();
				}
			}
		}
	}
}
