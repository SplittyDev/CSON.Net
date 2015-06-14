using System;

namespace cson.net
{
	public class TokenBase
	{
		public static readonly object nothing = new object ();

		public TokenType Type;
		public object Value;

		public TokenBase (TokenType type) {
			Type = type;
			Value = nothing;
		}

		public TokenBase (TokenType type, object value) {
			Type = type;
			Value = value;
		}

		public bool HasValue () {
			return Value != nothing;
		}
	}
}

