using System;
using System.Text;

namespace cson.net
{
	public static class CsonToJson
	{
		public static string ToJSON (string cson) {
			var tokens = Scanner.Tokenize (cson);
			var nodes = Parser.Parse (tokens);
			var accum = new StringBuilder ();
			return accum.ToString ();
		}
	}
}

