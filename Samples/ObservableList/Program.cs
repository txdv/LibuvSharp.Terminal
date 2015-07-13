using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terminal;

namespace ObservableList
{
	class MainClass
	{
		static IEnumerable<char> FromTo(char start, char end)
		{
			return Enumerable.Range(start, end - start + 1).Select(i => (char)i);
		}

		static IEnumerable<char> Chains(params string[] ranges) {
			var r = Enumerable.Empty<char>();
			foreach (var range in ranges) {
				var t = range.Split('-');
				Console.WriteLine(t.First());
				r = r.Concat(FromTo(t.First()[0], t.Last()[0]));
			}
			return r;
		}

		public static void Main(string[] args)
		{
			bool run = args.Length == 0 ? true : bool.Parse(args.First());

			if (run) {
				Application.Init();
			}

			var collection = new ObservableCollection<Widget>();

			var letters = Chains("a-z", "A-Z").ToArray();

			if (!run) {
				foreach (var letter in letters) {
					Console.WriteLine(letter);
				}
			}

			foreach (var letter in letters) {
				collection.Add(new Empty(letter));
			}

			if (run) {
				Application.Run(new FullsizeContainer(new ObservableList<Widget>(collection)));
			}
		}
	}
}
