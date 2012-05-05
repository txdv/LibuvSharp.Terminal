using System;

namespace LibuvSharp.Terminal
{
	public class Button : Widget
	{
		public Button(string text)
		{
			Text = text;
		}

		string text;
		public string Text {
			get {
				return text;
			}
			set {
				if (text != null) {
					Clear();
				}
				text = value;
			}
		}

		ColorPair color;
		public ColorPair Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}

		void Clear()
		{
			Curses.Cursor.Move(0, 0);
			Curses.attrset(0);
			for (int i = 0; i < text.Length + 4; i++) {
				Curses.Add(' ');
			}
		}

		private string Format
		{
			get {
				string text = Text;
				if (text.Length + 4 > Width) {
					text = text.Substring(0, Width - 3) + "...";
				}
				return string.Format("[ {0} ]", text);
			}
		}

		public override void Redraw()
		{

			Curses.Cursor.Move(0, 0);
			if (color != null) {
				Curses.attrset(color.Attribute);
			}

			if (HasFocus) {
			}

			Curses.Add(Format);
		}

		public override void SetCursorPosition()
		{
			Curses.Cursor.Move(0, 2);
		}

		public override bool ProcessKey(int key)
		{
			// enter
			if (key == 10) {
				if (PressEvent != null) {
					PressEvent();
				}
				return true;
			} else {
				return false;
			}
		}

		public event Action PressEvent;
	}
}

