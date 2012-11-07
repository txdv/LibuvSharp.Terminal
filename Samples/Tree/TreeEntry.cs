using System;
using System.Collections.Generic;
using LibuvSharp.Terminal;

namespace Tree
{
	class TreeEntry : Container, IList<TreeEntry>
	{
		List<TreeEntry> list = new List<TreeEntry>();

		#region IEnumerable implementation
		public System.Collections.IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator<TreeEntry> IEnumerable<TreeEntry>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region ICollection implementation
		public void Add(TreeEntry item)
		{
			list.Add(item);
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(TreeEntry item)
		{
			return list.Contains(item);
		}

		public void CopyTo(TreeEntry[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public bool Remove(TreeEntry item)
		{
			return list.Remove(item);
		}

		public int Count {
			get {
				return list.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion

		#region IList implementation
		public int IndexOf(TreeEntry item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, TreeEntry item)
		{
			list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public TreeEntry this[int index] {
			get {
				return list[index];
			}
			set {
				list[index] = value;
			}
		}
		#endregion

		public TreeEntry()
		{
		}

		public override void Redraw()
		{
			base.Redraw();

			foreach (TreeEntry element in this) {
				element.Redraw();
			}
		}

		/*
		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);

			foreach (TreeEntry element in this) {
				element.SetDim(0, 0, Width, 1);
			}
		}
		*/

		public virtual void DrawElements(int offset)
		{
		}

	}
}

