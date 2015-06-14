using System;

namespace cson.net
{
	public class ParserException : Exception
	{
		string message;

		public ParserException (string msg)
		{
			message = msg;
		}

		public override string Message { get { return message; } }
	}
}

