using System;
using Terminal;

namespace Tree
{
	class TextTreeEntry : TreeEntry
	{
		public string Text { get; protected set; }

		public TextTreeEntry(string text)
		{
			Text = text;
			Height = 1;
		}

		public override void Redraw()
		{
			base.Redraw();
			Fill(Text);
		}
	}
}
