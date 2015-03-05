using System;
using Terminal;

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
				heigth += SetDimElements(x + 2, y + heigth, w - 1, child);
			}
			return heigth;
		}

		bool whiteSpace = false;
		public bool WhiteSpace {
			get {
				return whiteSpace;
			}
			set {
				whiteSpace = value;
			}
		}

		public int WhiteSpaceOffset {
			get {
				return WhiteSpace ? 1 : 0;
			}
		}

		private int Offset {
			get {
				// the entries are always at least 2 away from
				// the sideline
				return 2 + WhiteSpaceOffset;
			}
		}

		public int DrawGrid(int x, int y, int w, TreeEntry entry)
		{
			switch (entry.Count) {
			case 0:
				return entry.Height;
			case 1:
				Set(x    , y + 1, ACS.LLCORNER);
				Set(x + 1, y + 1, '-');
				return DrawGrid(x + Offset, y + 1, w, entry[0]) + entry.Height;
			default:
				int height = 0;
				int i = 0, c = entry.Count - 1;
				foreach (TreeEntry child in entry) {
					bool first = i == 0;
					bool last = i == c;
					bool middle = !first && !last;
					if (first || middle) {
						Set(x,     y + height + 1, ACS.LTEE);
						Set(x + 1, y + height + 1, '-');
					} else {
						Set(x,     y + height + 1, ACS.LLCORNER);
						Set(x + 1, y + height + 1, '-');
					}

					int childHeight = DrawGrid(x + Offset, y + height + child.Height, w - 1, child);

					if (!last) {
						for (int j = 1; j < childHeight; j++) {
							Set(x, y + height + j + 1, ACS.VLINE);
						}
					}

					height += childHeight;
					i++;
				}
				return height + entry.Height;
			}
		}
	}
}

