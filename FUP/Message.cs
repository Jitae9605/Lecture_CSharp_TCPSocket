
namespace FUP
{
	// 서버와 클라이언트가 주고받은 메세지의 속성을 나타낼 상수정의 
	public class CONSTRAINT
	{
		public const uint REQ_FILE_SEND =	0x01;	// 파일전송요청
		public const uint REP_FILE_SEND =	0x02;	// 파일전송요청에 대한 대답/반응	(거절/수락)
		public const uint FILE_SEND_DATA =	0x03;	// 파일전송
		public const uint FILE_SEND_RES =	0x04;	// 파일전송 결과(성공/실패)

		public const byte NOT_FRAGMENTED = 0x00;	// 분할되지않은 파일
		public const byte FAGMENTED = 0x01;			// 분할된 파일

		public const byte NOT_LASTING = 0x00;		// 분할된 파일의 첫 ~ 끝-1 번째 조각파일
		public const byte LASTING = 0x01;           // 분할되지 않은 파일이거나, 분할된 파일의 마지막 조각파일

		public const byte ACCEPTED = 0x00;			// 요청 수락
		public const byte DENIED = 0x01;			// 요철 거절

		public const byte FAIL = 0x00;				// 실패
		public const byte SUCCESS = 0x01;			// 성공


	}

	// 서버와 클라이언트가 가져야하는 필수 함수 및 기능을 포함하는 인터페이스
	public interface ISerializable
	{
		byte[] GetBytes();			// 크기반환(byte형)
		int GetSize();				// 크기반환(int형)
	}

	// 서버와 클라이언트가 주고받을 메세지의 형태 및 프로퍼티 정의(클래스)
	public class Message : ISerializable
	{
		public Header Header { get; set; }
		public ISerializable Body { get; set; }

		public byte[] GetBytes()
		{
			byte[] bytes = new byte[GetSize()];					// 헤더와 바디의 크기를 바이트 단위로 분할해 bytes의 크기를 생성
			Header.GetBytes().CopyTo(bytes, 0);
			Body.GetBytes().CopyTo(bytes, Header.GetSize());

			return bytes;
		}

		public int GetSize()	// 메세지의 헤더와 바디의 크기를 더해서 반환
		{
			return Header.GetSize() + Body.GetSize();
		}
	}
}
