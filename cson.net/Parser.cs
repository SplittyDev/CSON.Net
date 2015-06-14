using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cson.net
{
	public static class Parser
	{
		static int pos;
		static List<TokenBase> tokens;
		static NodeArray parent;

		public static NodeArray Parse (List<TokenBase> tokens) {
			Parser.tokens = tokens;
			pos = -1;
			parent = new NodeArray ("base");

			Expect (TokenType.Identifier);
			var ident = Read ();

			while (pos < tokens.Count) {

				if (Peek () == null)
					break;

				if (Match (TokenType.ArrayStart)) {
					RecursiveMatchArray ((string)ident.Value, parent);
				}
			}

			return parent;
		}

		static NodeArray RecursiveMatchArray (string _ident, NodeArray _parent) {
			Skip ();
			var current = new NodeArray (_ident);

			while (!Match (TokenType.ArrayEnd)) {
				
				Expect (
					TokenType.Identifier,
					TokenType.IntegerLiteral, 
					TokenType.StringLiteral
				);

				// Key or Array identifier
				if (Match (TokenType.Identifier)) {
					var ident = Read ();

					// Key
					if (Match (TokenType.StringLiteral, TokenType.IntegerLiteral)) {
						
						Expect (TokenType.StringLiteral, TokenType.IntegerLiteral);
						current.Add (new NodeBase (NodeType.Key, ident.Value));

						// String value
						if (Match (TokenType.StringLiteral)) {
							var val = Read ();
							current.Add (new NodeBase (NodeType.ValueString, val.Value));
						}

						// Integer value
						else if (Match (TokenType.IntegerLiteral)) {
							var val = Read ();
							current.Add (new NodeBase (NodeType.ValueInteger, val.Value));
						}
					}

					// Array
					else if (Match (TokenType.ArrayStart)) {
						RecursiveMatchArray ((string)ident.Value, current);
					}
				}

				// String value
				else if (Match (TokenType.StringLiteral)) {
					var val = Read ();
					current.Add (new NodeBase (NodeType.ValueString, val.Value));
				}

				// Integer value
				else if (Match (TokenType.IntegerLiteral)) {
					var val = Read ();
					current.Add (new NodeBase (NodeType.ValueInteger, val.Value));
				}
			}

			_parent.Add (current);
			Skip ();

			return current;
		}

		static void Expect (TokenType type) {
			if (type != Peek ().Type)
				throw new ParserException (string.Format ("Expected {0}", type));
		}

		static void Expect (params TokenType[] types) {
			if (!types.Any (type => type == Peek ().Type)) {
				var accum = new StringBuilder (types.First ().ToString ());
				types.Skip (1).ToList ().ForEach (type => accum.AppendFormat ("|{0}", type));
				throw new ParserException (string.Format ("Expected {0}", accum));
			}
		}

		static bool Match (TokenType type) {
			return type == Peek ().Type;
		}

		static bool Match (params TokenType[] types) {
			for (int i = 0; i < types.Length; i++) {
				var _match = types[i] == Peek ().Type;
				if (_match)
					return true;
			}
			return false;
		}

		static void Skip (int count = 1) {
			pos += count;
		}

		static TokenBase Peek (int lookahead = 1) {
			return pos + lookahead < tokens.Count ? tokens [pos + lookahead] : null;
		}

		static TokenBase Read () {
			return pos + 1 < tokens.Count ? tokens [++pos] : null;
		}
	}
}

