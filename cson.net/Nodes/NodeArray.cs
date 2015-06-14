using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cson.net
{
	public class NodeArray : NodeBase, IEnumerable<NodeBase>, ICollection<NodeBase>
	{
		public string Name;
		public List<NodeBase> children;

		public NodeBase this[int i] {
			get {
				return children [i];
			}
		}

		public NodeBase this[string str] {
			get {
				return children.First (node => {
					if (node.Type == NodeType.Array)
						return str == ((NodeArray)node).Name;
					else if (node.Type == NodeType.Key && node.HasValue ())
						return str == (string)node.Value;
					return false;
				});
			}
		}

		public NodeArray (string name) : base (NodeType.Array, name)
		{
			Name = name;
			children = new List<NodeBase> ();
		}

		#region IEnumerable<NodeBase> implementation

		public IEnumerator<NodeBase> GetEnumerator () {
			foreach (var node in children)
				yield return node;
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator () {
			foreach (var node in children)
				yield return node;
		}

		#endregion

		#region ICollection<NodeBase> implementation

		public void Add (NodeBase item) {
			children.Add (item);
		}

		public void Clear () {
			children.Clear ();
		}

		public bool Contains (NodeBase item) {
			return children.Contains (item);
		}

		public void CopyTo (NodeBase[] array, int arrayIndex) {
			children.CopyTo (array, arrayIndex);
		}

		public bool Remove (NodeBase item) {
			return children.Remove (item);
		}

		public int Count {
			get {
				return children.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		#endregion
	}
}

