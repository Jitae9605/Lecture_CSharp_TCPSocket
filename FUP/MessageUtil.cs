using System;
using System.IO;

namespace FUP
{
	public class MessageUtil
	{
		// 스트림으로 부터 메세지를 보내고 받기위한 메소드를 가지는 클래스

		// 송신
		public static void Send(Stream writer, Message msg)
		{
			writer.Write(msg.GetBytes(), 0, msg.GetSize());
			//				메세지		오프셋	메세지크기

			// 메세지를 스트림에 작성 = 송신완료
		}

		// 수신
		public static Message Receive(Stream reader)	// 메세지를 반환
		{
			// 스트림 -> 송신버퍼 -> 메모리 로 옮기는 함수
			// 네트워크 불량 등으로 한번에 16byte가 못올수 있으므로 사이에 버퍼를 두어 쌓는다.
			int totalRecv = 0;						// 총 받은 크기
			int sizeToRead = 16;					// 읽어오고자 하는 바이트 수(헤더에 넣을꺼라서 16)
			byte[] hbuffer = new byte[sizeToRead];	// 헤더가 담길 버퍼

			while(sizeToRead > 0)									// 총 16바이트 받아올때까지 진행
			{
				byte[] buffer = new byte[sizeToRead];
				int recv = reader.Read(buffer, 0, sizeToRead);      // reader에서 sizeToRead 만큼 읽어와서 buffer에 저장하고 읽어온 byte수를 반환해 recv에 저장
				if (recv == 0)										// 읽어온 byte가 0 = reader가 비었음 = 스트림에서 buffer로 옮길게 없음
					return null;

				buffer.CopyTo(hbuffer, totalRecv);					// buffer의 값을 hbuffer에 삽입
				totalRecv += recv;									// 이때까지의 받은 byte수를 누적 저장(CopyTo에서의 시작 인덱스에 사용)
				sizeToRead -= recv;									// 받아와야할 남은 바이트수계산(헤더이므로 16byte만 받아와야함)
			}

			Header header = new Header(hbuffer);					// header를 선언하고 hbuffer를 입력

			totalRecv = 0;											// 수신데이터 초기화
			byte[] bBuffer = new byte[header.BODYLEN];              // body를 받을 bBuffer를 선언하고 크기는 header의 BODYLEN을 통해 설정
			sizeToRead = (int)header.BODYLEN;                       // 읽어와야할 데이터의 크기 = header의 BODYLEN

			while (sizeToRead > 0)
			{
				byte[] buffer = new byte[sizeToRead];
				int recv = reader.Read(buffer, 0, sizeToRead);
				if (recv == 0)
					return null;

				buffer.CopyTo(bBuffer, totalRecv);
				totalRecv += recv;
				sizeToRead -= recv;
			}

			ISerializable body = null;
			switch(header.MSGTYPE)					// 메세지 타입에따른 body 생성
			{
				case CONSTRAINT.REQ_FILE_SEND:			// 파일전송요청
					body = new BodyRequest(bBuffer);	// body = 파일크기, 이름
					break;
				case CONSTRAINT.REP_FILE_SEND:			// 파일전송요청에 대한 대답
					body = new BodyResponse(bBuffer);   // body = 메세지식별번호, ACCEPTED / DENIED
					break;
				case CONSTRAINT.FILE_SEND_DATA:			// 파일전송
					body = new BodyData(bBuffer);       // body = 전송파일의 데이터
					break;
				case CONSTRAINT.FILE_SEND_RES:			// 파일전송결과
					body = new BodyResult(bBuffer);     // body = 메세지식별번호, FAIL / SUCCESS
					break;
				default:
					throw new Exception(String.Format("Unkown MSGTYPE : {0}", header.MSGTYPE));

			}

			// 이렇게 완성한 헤더와 바디로 메세지를 만들어 반환 = 수신완료
			return new Message() { Header = header, Body = body };
		}
	}
}
