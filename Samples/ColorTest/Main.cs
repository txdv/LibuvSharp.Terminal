using System;
using LibuvSharp;
using Terminal;

namespace ColorTest
{
	class ColorWidget : Widget
	{
		int start = 0;
		public int Start {
			get {
				return start;
			}
			set {
				if (value < 0) {
					start = 0;
				} else {
					start = Math.Min(Curses.Colors - Height, value);
				}
				Debug.Log("start value = {0}", start);
			}
		}

		int TerminalHeight {
			get {
				return (int)(Curses.Terminal.Height * 0.8);
			}
		}

		public override bool ProcessKey (int key)
		{
			switch (key) {
			case 338: // page down
				Start += TerminalHeight;
				return (Invalid = true);
			case 339: // page up
				Start -= TerminalHeight;
				return (Invalid = true);
			case 259: // arrow up
				Start -= 1;
				return (Invalid = true);
			case 258: // arrow down
				Start += 1;
				return (Invalid = true);
			case 262: // home
				Start = 0;
				return (Invalid = true);
			case 360: // End
				Start = int.MaxValue;
				return (Invalid = true);
			default:
				return base.ProcessKey(key);
			}
		}

		string FillSpace(int number)
		{
			return FillSpace(number, Curses.Colors);
		}

		string FillSpace(int number, int max)
		{
			int length = max.ToString().Length;
			string str = number.ToString();
			while (str.Length < length) {
				str = " " + str;
			}
			return str;
		}

		public override void Redraw()
		{
			// Hack if you need more than 256 colors
			ColorPair.ReleaseAll();
			for (int i = 0; i < Curses.Colors; i++) {
				if (i >= Height) {
					break;
				}

				int y = start + i;

				string str = string.Format("\x0000{0} {1} \x0000{0},{0} ", y, FillSpace(y));
				ColorString.Fill(this, str, 0, i, Width, 1);
			}
		}

		public override void SetCursorPosition()
		{
			Curses.Cursor.Move(-1, -1);
		}

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);

			int size = Curses.Colors - (Start + Height);

			if (size < 0) {
				Start += size;
			}
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init();
			Application.Run(new FullsizeContainer(new ColorWidget()));
		}
	}
}

