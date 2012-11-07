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

		public override bool CanFocus {
			get {
				return true;
			}
		}

		void Clear()
		{
			Fill(' ');
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
			if (color != null) {
				Curses.attrset(color.Attribute);
			}

			Fill(Format);

		}

		public override void SetCursorPosition()
		{
			Move(2, 0);
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

