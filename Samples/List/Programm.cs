using System;
using LibuvSharp.Terminal;

namespace List
{
	public class Programm
	{
		public static void Main(string[] args)
		{
			Application.Init();
			var list = new SimpleList();
			var label = new Label("Hello World");

			foreach (var str in new string[] {
				"A very long sentence indeeed",
				"Hello",
				"World",
				"No",
				"Yes"
			}) {
				var b = new Button(str) { Height = 1 };
				b.PressEvent += () => label.Text = str;
				list.Add(b);
			}

			list.Add(label);
			Application.Run(list);
		}
	}
}

