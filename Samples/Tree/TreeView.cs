using System;
using LibuvSharp.Terminal;

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
}

