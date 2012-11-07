using System;
using LibuvSharp.Terminal;
using System.Linq;

namespace Tree
{
	class MainClass : Widget
	{
		static Random r = new Random();

		public static void Fill(TreeEntry entry, double chance)
		{
			double f;
			while ((f = r.NextDouble()) < chance) {
				var e = new TextTreeEntry(f.ToString());
				entry.Add(e);
				Fill(e, chance * chance);
			}
		}

		public static void Main(string[] args)
		{
			Application.Init();

			var t = new SimpleList();
			var l = new Label("Hello World");

			foreach (var str in new string[] { "Hello", "World", "No", "Yes" }) {
				var b = new Button(str) { Height = 1 };
				b.PressEvent += () => l.Text = str;
				t.Add(b);
			}

			t.Add(l);
			Application.Run(new FileTreeView("/home/bentkus/"));
		}
	}
}
