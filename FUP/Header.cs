using System;

namespace FUP
{
	// 주고받을 메세지의 헤더부분을 정의
	// 헤더에는 메세지이름, 타입, 바디의 크기, 분할여부, 마지막여부, 전송단계 를 나타낸다.
	public class Header : ISerializable
	{
		// 헤더는 반드시 16바이트로 구성
		public uint MSGID { get; set; }			// 메세지 식별번호   - 4 바이트
		public uint MSGTYPE { get; set; }		// 메세지타입(=종류) - 4 바이트
		public uint BODYLEN { get; set; }		// 바디의 크기		- 4 바이트
		public byte FRAGMENTED { get; set; }	// 분할여부			- 1 바이트
		public byte LASTMSG { get; set; }		// 마지막 여부		- 1 바이트
		public ushort SEQ { get; set; }			// 파편번호			- 2 바이트

		public Header() { }
		public Header(byte[] bytes)
		{
			MSGID = BitConverter.ToUInt32(bytes, 0);		// bytes의 앞부분 4byte(byte[0] ~ [3])의 데이터를 uint로 컨버터해 저장
			MSGTYPE = BitConverter.ToUInt32(bytes, 4);      // bytes의 다음 부분 4byte(byte[4] ~ [7])의 데이터를 uint로 컨버터해 저장
			BODYLEN = BitConverter.ToUInt32(bytes, 8);      // bytes의 다음 부분 4byte(byte[8] ~ [11])의 데이터를 uint로 컨버터해 저장
			FRAGMENTED = bytes[12];							// bytes[12]를 byte형 그대로 저장 
			LASTMSG = bytes[13];                            // bytes[13]을 byte형 그대로 저장
			SEQ = BitConverter.ToUInt16(bytes, 14);         // bytes의 다음 부분 2byte(byte[14] ~ [15])의 데이터를 ushort로 컨버터해 저장
		}

		public byte[] GetBytes()
		{
			// 헤더의 정보를 바이트로 전환하여 순서대로 이어붙인다음 반환
			byte[] bytes = new byte[16];

			byte[] temp = BitConverter.GetBytes(MSGID);		// MSGID를 바이트로 변환해 temp에 저장
			Array.Copy(temp, 0, bytes, 0, temp.Length);		// temp[0] 부터 temp.Length개 만큼의 요소를 byte[0]부터 순서대로 삽입

			temp = BitConverter.GetBytes(MSGTYPE);          // MSGTYPE를 바이트로 변환해 temp에 저장
			Array.Copy(temp, 0, bytes, 4, temp.Length);		// temp[0] 부터 temp.Length개 만큼의 요소를 byte[4]부터 순서대로 삽입

			temp = BitConverter.GetBytes(BODYLEN);          // BODYLEN를 바이트로 변환해 temp에 저장
			Array.Copy(temp, 0, bytes, 8, temp.Length);		// temp[0] 부터 temp.Length개 만큼의 요소를 byte[8]부터 순서대로 삽입

			bytes[12] = FRAGMENTED;                         // bytes[12]에 FRAGMENTED 삽입
			bytes[13] = LASTMSG;                            // bytes[13]에 LASTMSG 삽입

			temp = BitConverter.GetBytes(SEQ);              // SEQ를 바이트로 변환해 temp에 저장
			Array.Copy(temp, 0, bytes, 14, temp.Length);	// temp[0] 부터 temp.Length개 만큼의 요소를 byte[14]부터 순서대로 삽입

			return bytes;
		}

		public int GetSize()
		{
			// 헤더는 항상 16byte이므로
			return 16;
		}
	}
}
