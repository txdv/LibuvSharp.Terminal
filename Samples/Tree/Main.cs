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
			Application.Run(new FileTreeView("/home/bentkus/"));
		}
	}
}
