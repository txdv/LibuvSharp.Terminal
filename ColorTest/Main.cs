using System;
using Manos.IO;
using Mono.Terminal;

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
				int max = Curses.Colors - Height;
				if (value < 0) {
					start = 0;
				} else if (value > max) {
					start = max;
				} else {
					start = value;
				}
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
			case 338:
				Start += TerminalHeight;
				Invalid = true;
				return true;
			case 339:
				Start -= TerminalHeight;
				Invalid = true;
				return true;
			case 259:
				Start -= 1;
				Invalid = true;
				return true;
			case 258:
				Start += 1;
				Invalid = true;
				return true;
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
			int l = max.ToString().Length;
			string str = number.ToString();
			while (str.Length < l) {
				str = " " + str;
			}
			return str;
		}

		string WhiteSpace(int size)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("", size);
			for (int i = 0; i < size; i++) {
				sb.Append(" ");
			}
			return sb.ToString();
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

				ColorString.Draw(this, string.Format("\x0000{0} {1}", y, FillSpace(y)), 0, i);
				ColorString.Draw(this, string.Format("\x0000{0},{0} {1}", y, WhiteSpace(Curses.Terminal.Width)), 4, i);
			}
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init(Context.Create(Backend.Poll));

			Application.Run(new FullsizeContainer(new ColorWidget()));
		}
	}
}

