using System;
using System.Collections.Generic;
using Terminal;

namespace ObservableList
{
	public partial class ObservableList<T> : Container where T : Widget
	{
		public void Focus(IEnumerable<T> widgets, Func<IEnumerable<T>, T, T> selector)
		{
			if (CurrentFocus == null) {
				CurrentFocus = GetFirstFocusable(ObservableCollection);
			}

			var next = selector(widgets, CurrentFocus);
			if (next != null) {
				SetCurrentFocus(next);
			}

			if (CurrentFocus == null) {
				CurrentFocus = GetFirstFocusable(widgets);
			}

			if (CurrentFocus != null) {
				CurrentFocus.HasFocus = true;
			}
		}

		public T GetNext(IEnumerable<T> widgets, T widget)
		{
			T last = default(T);
			bool nextbreak = false;
			foreach (var element in widgets) {
				if (nextbreak) {
					return element;
				}
				if (element == widget) {
					nextbreak = true;
				}
				last = element;
			}
			return last;
		}

		public void FocusNext(IEnumerable<T> widgets)
		{
			Focus(widgets, GetNext);
		}

		public T GetPrevious(IEnumerable<T> widgets, T widget)
		{
			T previous = default(T);
			foreach (var element in widgets) {
				if (element == widget) {
					return previous;
				}
				previous = element;
			}
			return default(T);
		}

		public void FocusPrevious(IEnumerable<T> widgets)
		{
			Focus(widgets, GetPrevious);
		}

		public void DeleteCurrent(IEnumerable<T> widgets)
		{
			if (CurrentFocus != null) {
				var previous = GetPrevious(widgets, CurrentFocus);
				ObservableCollection.Remove(CurrentFocus);
				SetCurrentFocus(GetNext(widgets, previous));
				Invalid = true;
			}
		}
	}
}

