using System;
using System.IO;

namespace Tree
{
	class FileTreeView : TreeView
	{
		public FileTreeView(string path)
			: this(new DirectoryInfo(path))
		{
		}

		public FileTreeView(DirectoryInfo di)
		{
			Discover(Root, di, 0, 3);
		}

		void Discover(TreeEntry entry, DirectoryInfo di, int level, int maxlevel) 
		{
			if (level > maxlevel) {
				return;
			}

			var child = new TextTreeEntry(di.Name);
			entry.Add(child);

			foreach (var dir in di.GetDirectories()) {
				Discover(child, dir, level + 1, maxlevel);
			}
		}
	}
}
