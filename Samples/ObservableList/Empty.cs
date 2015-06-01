using System;
using System.Drawing;
using Terminal;

namespace ObservableList
{
	public class Empty : Widget
	{
		public override bool CanFocus {
			get {
				return true;
			}
		}

		public Empty(char ch)
		{
			Char = ch;
		}

		public char Char { get; set; }

		public override void Redraw()
		{
			if (!Invalid) {
				return;
			}

			base.Redraw();

			Fill(HasFocus ? Curses.Attributes.Bold : Curses.Attributes.Normal, Char);
		}
	}
}

