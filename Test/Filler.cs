using System;
using Mono.Terminal;

namespace Test
{
	class Filler : Widget
	{
		public char Char { get; set; }

		public Filler(char ch)
		{
			Char = ch;
		}
		public override void Redraw()
		{
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					Set(x, y, Char);
				}
			}
		}

		public override bool ProcessKey(int key)
		{
			return false;
		}

		public override void SetCursorPosition()
		{
			Curses.Cursor.Move(0, 0);
		}
	}
}

