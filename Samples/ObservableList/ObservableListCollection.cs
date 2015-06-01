using System;
using System.Collections.Generic;
using System.Linq;
using Terminal;

namespace ObservableList
{
	public partial class ObservableList<T> : Container where T : Widget
	{
		public int Index { get; private set; }

		public void FocusCollection(ICollection<T> collection, Func<int, int> selector)
		{
			var newIndex = selector(Index);

			if (newIndex < 0) {
				newIndex = 0;
			}

			int count = collection.Count;
			if (newIndex >= count) {
				newIndex = count - 1;
			}

			if (newIndex != Index) {
				var element = collection.ElementAt(newIndex);
				SetCurrentFocus(element);
				Index = newIndex;
			}
		}

		public void FocusPreviousCollection(ICollection<T> collection)
		{
			FocusCollection(collection, i => --i);
		}

		public void FocusNextCollection(ICollection<T> collection)
		{
			FocusCollection(collection, i => ++i);
		}

		public void DeleteCurrentCollection(ICollection<T> collection)
		{
			try {
				var element = collection.ElementAt(Index);
				collection.Remove(element);
			} catch (ArgumentOutOfRangeException) {
				// do nothing?
			}
		}

		public void EnsureIndexCollection(ICollection<T> collection)
		{
			int count = collection.Count;
			if (count != 0) {
				if (Index < 0) {
					Index = count - 1;
				} else if (Index > count - 1) {
					Index = count - 1;
				}
				SetCurrentFocus(collection.ElementAt(Index));
			}
		}
	}
}

