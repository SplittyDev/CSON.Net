using System;
using System.Text;
using cson.net;

namespace cson.net.tests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var cson = @"
YouTube: [
  Videos: [
    MLGMontage: [
      URL: 'youtube.com/view?v=test'
      Stats: [
        Views: 28173
        Likes: 451
        Dislikes: 7
      ]
    ]
  ]
]
".Trim (' ', '\n', '\t', '\r');
			Console.WriteLine ("== Input ============================");
			Console.WriteLine (cson);
			Console.WriteLine ();

			Console.WriteLine ("== Scanner ==========================");
			var tokens = Scanner.Tokenize (cson);
			for (var i = 0; i < tokens.Count; i++) {
				Console.WriteLine ("{0}: {1}", tokens [i].Type, tokens [i].Value);
			}
			Console.WriteLine ();

			Console.WriteLine ("== Parser ===========================");
			var tree = Parser.Parse (tokens);
			Console.WriteLine (BuildTree (tree).Trim (' ', '\n', '\t', '\r'));
			Console.WriteLine ();

			Console.WriteLine ("== Reconstructor ====================");
			Console.WriteLine (Restore (tree));
		}

		static string BuildTree (NodeArray parent, int depth = 0) {
			var accum = new StringBuilder ();
			accum.AppendFormat ("{0}*{1}\n", "".PadLeft (depth * 2, ' '), parent.Type);
			++depth;
			foreach (var node in parent) {
				if (node.IsArray ())
					accum.AppendFormat ("{0}\n", BuildTree ((NodeArray)node, depth));
				else
					accum.AppendFormat ("{0}*--{1}\n", "".PadLeft ((depth - 1) * 2, ' '), node.Type);
			}
			return accum.ToString ();
		}

		static string Restore (NodeArray parent, int depth = 0) {
			var accum = new StringBuilder ();
			bool waskey = false;
			foreach (var node in parent) {
				if (node.IsArray ())
					accum.AppendFormat ("{0}{1}: [\n{2}{0}]\n", "".PadLeft (depth * 2, ' '), ((NodeArray)node).Name, Restore ((NodeArray)node, depth + 1));
				else if (node.Type == NodeType.Key) {
					waskey = true;
					accum.AppendFormat ("{0}{1}: ", "".PadLeft (depth * 2, ' '), node.Value);
				} else if (node.Type == NodeType.ValueString) {
					accum.AppendFormat ("{0}'{1}'\n", waskey ? string.Empty : "".PadLeft (depth * 2, ' '), node.Value);
					waskey = false;
				} else if (node.Type == NodeType.ValueInteger) {
					accum.AppendFormat ("{0}{1}\n", waskey ? string.Empty : "".PadLeft (depth * 2, ' '), node.Value);
					waskey = false;
				}
			}
			return accum.ToString ();
		}
	}
}
