using System;
using LibuvSharp.Terminal;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Tree
{
	class TreeView : Container
	{
		public TreeView()
		{
			Root = new TreeEntry();
		}

		public TreeEntry Root { get; protected set; }

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);
			SetDimElements(x, y, w, Root[0]);
		}

		public override void Redraw()
		{
			base.Redraw();

			//foreach (TreeEntry element in Root) {
			//	element.Redraw();
			//	DrawGrid(X, Y, Width, Height, element);
			//}
			Root[0].Redraw();
			DrawGrid(0, 0, Width, Root[0]);

		}

		public int SetDimElements(int x, int y, int w, TreeEntry entry)
		{
			int heigth = entry.Height;
			entry.SetDim(x, y, w, heigth);
			foreach (TreeEntry child in entry) {
				heigth += SetDimElements(x + 1, y + heigth, w - 1, child);
			}
			return heigth;
		}

		public int DrawGrid(int x, int y, int w, TreeEntry entry)
		{
			switch (entry.Count) {
			case 0:
				return entry.Height;
			case 1:
				Set(x, 1 + y, ACS.LLCORNER);
				return DrawGrid(x, y, w, entry[0]) + entry.Height;
			default:
				int height = 0;
				int i = 0, c = entry.Count - 1;
				foreach (TreeEntry child in entry) {
					bool first = i == 0;
					bool last = i == c;
					bool middle = !first && !last;
					if (first || middle) {
						Set(x, 1 + y + height, ACS.LTEE);
					} else {
						Set(x, 1 + y + height, ACS.LLCORNER);
					}

					int childHeight = DrawGrid(x + 1, 1 + y + height + child.Height - 1, w - 1, child);

					if (!last) {
						for (int j = 1; j < childHeight; j++) {
							Set(x, 1 + y + height + j, ACS.VLINE);
						}
					}

					height += childHeight;
					i++;
				}
				return height + entry.Height;
			}
		}
	}

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

	class TextTreeEntry : TreeEntry
	{
		public string Text { get; protected set; }

		public TextTreeEntry(string text)
		{
			Text = text;
			Height = 1;
		}

		public override void Redraw ()
		{
			base.Redraw();
			Fill(Text);
		}
	}

	class FileTreeView : TreeView
	{
		public FileTreeView(string path)
			: this(new DirectoryInfo(path))
		{
		}

		public FileTreeView(DirectoryInfo di)
		{
			Discover(Root, di, 0, 3);
		}

		void Discover(TreeEntry entry, DirectoryInfo di, int level, int maxlevel) 
		{
			if (level > maxlevel) {
				return;
			}

			var child = new TextTreeEntry(di.Name);
			entry.Add(child);

			foreach (var dir in di.GetDirectories()) {
				Discover(child, dir, level + 1, maxlevel);
			}
		}
	}

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
			switch (key) {
			case 259: // up
				return Previous();
			case 258: // down
				return Next();
			}

			if (FocusedWidget != null) {
				return FocusedWidget.ProcessKey(key);
			}

			return false;
		}
	}

	public class Label : Widget
	{
		string text;
		public string Text {
			get {
				return text;
			}
			set {
				Invalid = true;
				text = value;
			}
		}

		public Label(string text)
		{
			Text = text;
			SetDim(X, Y, text.Length, 1);
		}

		public override void Redraw()
		{
			base.Redraw();
			Fill(Text);
		}
	}

	class MainClass : Widget
	{
		static Random r = new Random();

		public static void Fill(TreeEntry entry, double chance)
		{
			double f;
			while ((f = r.NextDouble()) < chance) {
				var e = new TextTreeEntry(f.ToString());
				entry.Add(e);
				Fill(e, chance * chance);
			}
		}

		public static void Main(string[] args)
		{
			Application.Init();

			var t = new SimpleList();
			var l = new Label("Hello World");

			foreach (var str in new string[] { "Hello", "World", "No", "Yes" }) {
				var b = new Button(str) { Height = 1 };
				b.PressEvent += () => l.Text = str;
				t.Add(b);
			}

			t.Add(l);
			Application.Run(new FileTreeView("/home/bentkus/"));
			//Application.Run(new FullsizeContainer(l));
		}
	}
}
