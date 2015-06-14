using System;
using System.Collections.Generic;
using System.Text;

namespace cson.net
{
	public static class Scanner
	{
		#region Constants

		const char ARR_OPEN = '[';
		const char ARR_CLOSE = ']';
		const char NEWLINE = '\n';
		const char EOI = '\0';
		const char STRING = '\'';
		const char IDENT = ':';
		const string VERBATIM = "'''";

		#endregion

		#region Static Fields

		static string src;
		static int pos;

		static List<TokenBase> _tokens;
		static object sync_tokens = new object ();

		static List<TokenBase> tokens {
			get {
				if (_tokens == null)
					lock (sync_tokens)
						if (_tokens == null)
							_tokens = new List<TokenBase> ();
				return _tokens;
			}
		}

		#endregion

		#region Public Members

		public static List<TokenBase> Tokenize (string src) {
			Scanner.src = src;
			pos = -1;
			tokens.Clear ();

			bool exit_loop = false;
			while (pos < src.Length) {

				EatWhitespace ();

				if (Peek () == -1)
					break;

				// Verbatim string: '''text'''
				if (Check (VERBATIM)) {
					var vsl = ScanVerbatimStringLiteral ();
					tokens.Add (new TokenBase (TokenType.StringLiteral, vsl));
					continue;
				}

				// String: 'text'
				else if (Check (STRING)) {
					var sl = ScanStringLiteral ();
					tokens.Add (new TokenBase (TokenType.StringLiteral, sl));
					continue;
				}

				// Integer: 123
				else if (char.IsNumber (Peekc ())) {
					var num = ScanIntegerLiteral ();
					tokens.Add (new TokenBase (TokenType.IntegerLiteral, num));
					continue;
				}

				// Identifier: ident:
				else if (char.IsLetter (Peekc ())) {
					var ident = ScanIdentifier ();
					tokens.Add (new TokenBase (TokenType.Identifier, ident));
					continue;
				}

				switch (Peekc ()) {

				// Array opening bracket: [
				case ARR_OPEN:
					Skip ();
					tokens.Add (new TokenBase (TokenType.ArrayStart));
					break;

				// Array closing bracket: ]
				case ARR_CLOSE:
					Skip ();
					tokens.Add (new TokenBase (TokenType.ArrayEnd));
					break;
				
				// End of input
				case EOI:
					exit_loop = true;
					break;

				// Unexpected character
				default:
					throw new ScannerException (string.Format ("Unexpected character: {0}", Peekc ()));
				}

				if (exit_loop)
					break;
			}

			return new List<TokenBase> (tokens);
		}

		#endregion

		#region Private Members

		static void EatWhitespace () {
			
			while (char.IsWhiteSpace (Peekc ()) || Check (NEWLINE))
				Skip ();
		}

		static string ScanIdentifier () {

			var accum = new StringBuilder ();

			while (!Check (IDENT) && !Check (NEWLINE))
				accum.Append (Readc ());

			if (Check (NEWLINE))
				throw new ScannerException ("Unexpected character in identifier: NEWLINE");

			Skip ();
			return accum.ToString ();
		}

		static string ScanStringLiteral () {

			Skip ();
			var accum = new StringBuilder ();

			while (!Check (STRING) && !Check (NEWLINE))
				accum.Append (Readc ());

			if (Check (NEWLINE))
				throw new ScannerException ("Unexpected character in string literal: NEWLINE");
			
			Skip ();
			return accum.ToString ();
		}

		static string ScanVerbatimStringLiteral () {

			Skip (VERBATIM.Length);
			var accum = new StringBuilder ();

			while (!Check (VERBATIM))
				accum.Append (Readc ());

			Skip (VERBATIM.Length);
			return accum.ToString ();
		}

		static int ScanIntegerLiteral () {
		
			var accum = new StringBuilder ();

			while (char.IsNumber (Peekc ()))
				accum.Append (Readc ());

			return int.Parse (accum.ToString ());
		}

		static void Skip (int count = 1) {
			pos += count;
		}

		static int Peek (int lookahead = 1) {
			return pos + lookahead < src.Length ? (int)src [pos + lookahead] : -1;
		}

		static char Peekc (int lookahead = 1) {
			return pos + lookahead < src.Length ? src [pos + lookahead] : EOI;
		}

		static int Read () {
			return pos + 1 < src.Length ? (int)src [++pos] : -1;
		}

		static char Readc () {
			return pos + 1 < src.Length ? src [++pos] : EOI;
		}

		static bool Check (char chr) {
			return Peekc () == chr;
		}

		static bool Check (string str) {
			
			var accum = new StringBuilder ();

			for (int i = 0; i < str.Length; i++)
				accum.Append (Peekc (i + 1));
			
			return accum.ToString () == str;
		}

		#endregion
	}
}

