using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FUP
{
	// 파일전송요청메시지에 사용할 본문 클래스(Header의 MSGTYPE이 REQ_FILE_SEND) 
	// 클라이언트 -> 서버
	public class BodyRequest : ISerializable
	{
		public long FILESIZE;			// 파일의 크기
		public byte[] FILENAME;			// 파일의 이름

		public BodyRequest() { }
		public BodyRequest(byte[] bytes)
		{
			FILESIZE = BitConverter.ToInt64(bytes, 0);          // bytes의 크기를 int형으로 변환해 FILESIZE에 저장
			FILENAME = new byte[bytes.Length - sizeof(long)];	// byte형 배열을 만드는데 그 크기를 [bytes의 크기 - FILESIZE의 크기(long형)]로 한다.
			Array.Copy(bytes, sizeof(long), FILENAME, 0, FILENAME.Length);  // FILENAME에 bytes의 크기를 나타내는 부분 이후(= bytes[sizeof(long)] 이후)을 모두 복사해
																			// FILENAME[0]부터 순서대로 삽입
		}

		public byte[] GetBytes()
		{
			byte[] bytes = new byte[GetSize()];
			byte[] temp = BitConverter.GetBytes(FILESIZE);                  // FILESIZE를 byte형으로 변환해 temp에 저장
			Array.Copy(temp, 0, bytes, 0, temp.Length);						// bytes에 temp를 순서대로 복사삽입
			Array.Copy(FILENAME, 0, bytes, temp.Length, FILENAME.Length);   // FILENAME을 bytes[temp.Length] 부터 시작해 각 요소 순서대로 복사삽입

			return bytes;
		}

		public int GetSize()
		{
			return sizeof(long) + FILENAME.Length;      // long형의 크기(FILESIZE의 크기) + FILENAME의 크기
		}
	}

	// 파일전송요청메시지에 답장할 때의 본문 클래스(Header의 MSGTYPE이 REP_FILE_SEND) 
	// 서버 -> 클라이언트
	public class BodyResponse : ISerializable
	{
		public uint MSGID;							// 메세지 식별번호
		public byte RESPONSE;                       // ACCEPTED / DENIED

		public BodyResponse() { }
		public BodyResponse(byte[] bytes)
		{
			MSGID = BitConverter.ToUInt32(bytes, 0);
			RESPONSE = bytes[4];
		}

		public byte[]  GetBytes()
		{
			byte[] bytes = new byte[GetSize()];
			byte[] temp = BitConverter.GetBytes(MSGID);
			Array.Copy(temp, 0, bytes, 0, temp.Length);
			bytes[temp.Length] = RESPONSE;

			return bytes;
		}

		public int GetSize()
		{
			return sizeof(uint) + sizeof(byte);         // MSGID의 크기 + RESPONSE의 크기
		}
	}

	// 파일전송메시지의 본문 클래스(Header의 MSGTYPE이 FILE_SEND_DATA) 
	// 클라이언트 -> 서버
	public class BodyData : ISerializable
	{
		public byte[] DATA;						// 전송하고자하는 파일데이터

		public BodyData(byte[] bytes)			// 전송하고자하는 파일을 byte로 변환해 전달받음
		{
			DATA = new byte[bytes.Length];
			bytes.CopyTo(DATA, 0);				// 전달받은 데이터를 DATA에 저장
		}

		public byte[] GetBytes()
		{
			return DATA;						//  DATA반환
		}

		public int GetSize()
		{
			return DATA.Length;					// DATA의 크기 반환
		}
	}

	// 파일전송결과메시지의 본문 클래스(Header의 MSGTYPE이 FILE_SEND_RES) 
	// 서버 -> 클라이언트
	public class BodyResult : ISerializable
	{
		public uint MSGID;                      // 메세지 식별번호
		public byte RESULT;                     // FAIL / SUCCESS

		public BodyResult() { }

		public BodyResult(byte[] bytes)
		{
			MSGID = BitConverter.ToUInt32(bytes, 0);
			RESULT = bytes[4];
		}

		public byte[] GetBytes()
		{
			byte[] bytes = new byte[GetSize()];
			byte[] temp = BitConverter.GetBytes(MSGID);
			Array.Copy(temp, 0, bytes, 0, temp.Length);
			bytes[temp.Length] = RESULT;

			return bytes;
		}

		public int GetSize()
		{
			return sizeof(uint) + sizeof(byte);
		}
	}
}
