using System;

namespace cson.net
{
	public class ScannerException : Exception
	{
		string message;

		public ScannerException (string msg)
		{
			message = msg;
		}

		public override string Message { get { return message; } }
	}
}

