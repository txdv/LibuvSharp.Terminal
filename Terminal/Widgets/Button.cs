using System;

namespace Terminal
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

		public override void Redraw()
		{
			if (color != null) {
				Curses.attrset(color.Attribute);
			}

			string text = Text;
			if (text.Length + 4 > Width) {
				int width = Width - 4 - 3;
				text = text.Substring(0, width < 0 ? 0 : width) + "...";
			}

			Fill(' ');
			Draw("[");
			Draw(text, 2, 0);
			Draw("]", Width - 1, 0);
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

