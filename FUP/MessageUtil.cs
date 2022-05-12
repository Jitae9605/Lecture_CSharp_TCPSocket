using System;
using System.IO;

namespace FUP
{
	public class MessageUtil
	{
		public static void Send(Stream writer, Message msg)
		{
			writer.Write(msg.GetBytes(), 0, msg.GetSize());
		}

		public static Message Receive(Stream reader)
		{
			int totalRecv = 0;
			int sizeToRead = 16;
			byte[] hbuffer = new byte[sizeToRead];

			while(sizeToRead > 0)
			{
				byte[] buffer = new byte[sizeToRead];
				int recv = reader.Read(buffer, 0, sizeToRead);
				if (recv == 0)
					return null;

				buffer.CopyTo(hbuffer, totalRecv);
				totalRecv += recv;
				sizeToRead -= recv;
			}

			Header header = new Header(hbuffer);

			totalRecv = 0;
			byte[] bBuffer = new byte[header.BODYLEN];
			sizeToRead = (int)header.BODYLEN;

			while(sizeToRead > 0)
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
			switch(header.MSGTYPE)
			{
				case CONSTRAINT.REQ_FILE_SEND:
					body = new BodyRequest(bBuffer);
					break;
				case CONSTRAINT.REP_FILE_SEND:
					body = new BodyResponse(bBuffer);
					break;
				case CONSTRAINT.FILE_SEND_DATA:
					body = new BodyData(bBuffer);
					break;
				case CONSTRAINT.FILE_SEND_RES:
					body = new BodyResult(bBuffer);
					break;
				default:
					throw new Exception(String.Format("Unkown MSGTYPE : {0}", header.MSGTYPE));

			}
			return new Message() { Header = header, Body = body };
		}
	}
}
