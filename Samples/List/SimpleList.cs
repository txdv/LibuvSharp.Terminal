using System;
using System.Collections.Generic;
using System.Linq;
using Terminal;

namespace List
{
	class SimpleList : Container, IList<Widget>
	{
		List<Widget> list = new List<Widget>();

		#region IEnumerable implementation
		public System.Collections.IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region ICollection implementation
		public void Add(Widget item)
		{
			list.Add(item);
			CheckFocus(item);
		}

		public void Clear()
		{
			list.Clear();
			FocusedWidget = null;
		}

		public bool Contains(Widget item)
		{
			return list.Contains(item);
		}

		public void CopyTo(Widget[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);

			if (FocusedWidget != null) {
				return;
			}
			foreach (var e in array) {
				if (CheckFocus(e)) {
					break;
				}
			}
		}

		public bool Remove(Widget item)
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
		public int IndexOf(Widget item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, Widget item)
		{
			list.Insert(index, item);
			CheckFocus(item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public Widget this[int index] {
			get {
				return list[index];
			}
			set {
				CheckFocus(value);
				list[index] = value;
			}
		}
		#endregion

		bool CheckFocus(Widget widget)
		{
			widget.Container = this;

			if (widget != null) {
				if (FocusedWidget == null && widget.CanFocus) {
					FocusedWidget = widget;
					return true;
				}
			}
			return false;
		}

		public Widget FocusedWidget { get; protected set; }

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);
			int height = 0;
			foreach (Widget widget in this) {
				if (widget.Height <= 0) {
					widget.Height = 1;
				}

				widget.SetDim(x, y + height, w, widget.Height);

				height += widget.Height;
			}
		}

		public override void Redraw()
		{
			base.Redraw();
			foreach (Widget widget in this) {
				widget.Redraw();
			}
		}

		public override bool CanFocus {
			get {
				return this.Any((e) => e.CanFocus);
			}
		}

		public override void SetCursorPosition()
		{
			if (FocusedWidget != null) {
				FocusedWidget.SetCursorPosition();
			}
		}

		public bool Previous()
		{
			if (!CanFocus) {
				return false;
			}

			int p = IndexOf(FocusedWidget);
			for (int i = p - 1; i >= 0; i--) {
				var w = this[i];
				if (w.CanFocus) {
					FocusedWidget = w;
					return true;
				}
			}
			for (int i = this.Count - 1; i > p; i--) {
				var w = this[i];
				if (w.CanFocus) {
					FocusedWidget = w;
					return true;
				}
			}
			return false;
		}

		public bool Next()
		{
			if (!CanFocus) {
				return false;
			}

			int p = IndexOf(FocusedWidget);
			for (int i = p + 1; i < this.Count; i++) {
				var w = this[i];
				if (w.CanFocus) {
					FocusedWidget = w;
					return true;
				}
			}
			for (int i = 0; i < p; i++) {
				var w = this[i];
				if (w.CanFocus) {
					FocusedWidget = w;
					return true;
				}
			}
			return false;
		}

		public override bool ProcessKey(int key)
		{
			base.ProcessKey(key);

			switch (key) {
			case 259: // up
				Invalid = true;
				return Previous();
			case 258: // down
				Invalid = true;
				return Next();
			}

			if (FocusedWidget != null) {
				return FocusedWidget.ProcessKey(key);
			}

			return false;
		}
	}
}

