using System;
using System.Linq;
using Terminal;

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
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			Application.Run(new FileTreeView(path));
		}
	}
}
