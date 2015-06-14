using System;

namespace cson.net
{
	public class NodeBase
	{
		public NodeType Type;
		public object Value;

		public NodeBase (NodeType type, object value) {
			Type = type;
			Value = value;
		}

		public bool HasValue () {
			return Value != TokenBase.nothing;
		}

		public bool IsArray () {
			return Type == NodeType.Array;
		}

		public bool IsKey () {
			return Type == NodeType.Key;
		}
	}
}

