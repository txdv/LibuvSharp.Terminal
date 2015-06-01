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

		public static void Main(string[] args)
		{
			bool run = args.Length == 0 ? true : bool.Parse(args.First());

			if (run) {
				Application.Init();
			}

			var collection = new ObservableCollection<Widget>();

			var letters = FromTo('a', 'z').Concat(FromTo('A', 'Z')).ToArray();

			if (!run) {
				foreach (var letter in letters) {
					Console.WriteLine(letter);
				}
			}

			foreach (var letter in letters) {
				collection.Add(new Empty(letter));
			}

			//UVTimer.Times(letters.Length, TimeSpan.FromSeconds(1), (i) => collection.Add(new Empty(letters[i - 1])));

			if (run) {
				Application.Run(new FullsizeContainer(new ObservableList<Widget>(collection)));
			}
		}
	}
}
