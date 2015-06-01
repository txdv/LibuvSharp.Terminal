using System;
using Terminal;

namespace List
{
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
}

