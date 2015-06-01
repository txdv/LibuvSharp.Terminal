using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using Terminal;

namespace ObservableList
{
	public partial class ObservableList<T> : Container where T : Widget
	{
		public ObservableCollection<T> ObservableCollection { get; private set; }

		public ObservableList(ObservableCollection<T> observableCollection)
		{
			ObservableCollection = observableCollection;

			ObservableCollection.CollectionChanged += CollectionChanged;

			lastCount = ObservableCollection.Count;

			if (ObservableCollection.Count > 0) {
				SetContainer(ObservableCollection);
				Invalidate(ObservableCollection);
				SetCurrentFocus(GetFirstFocusable(ObservableCollection));
			}

			AdjustSizes(ObservableCollection);
		}

		int lastCount;

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);

			AdjustSizes(ObservableCollection);
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Invalid = true;

			if (lastCount == 0) {
				SetCurrentFocus(GetFirstFocusable(ObservableCollection));
			}

			lastCount = ObservableCollection.Count;

			Debug.Log("ObseravableList: {0} {1}", HasFocus, lastCount);

			Debug.Log("CollectionChanged Action: {0}", e.Action);

			if (e.Action == NotifyCollectionChangedAction.Add) {
				AdjustSizes(e.NewItems.OfType<T>(), e.NewStartingIndex);
				//} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				//	var removed = e.OldItems.OfType<T>();
				//	Debug.Log("--------> {0}", removed.Count());
			} else {
				AdjustSizes(ObservableCollection);
			}

			Invalidate(ObservableCollection);
		}

		public void Invalidate(IEnumerable<Widget> widgets, bool value = true)
		{
			foreach (var widget in widgets) {
				widget.Invalid = value;
			}
		}

		public void SetContainer(IEnumerable<Widget> widgets, Container container = null)
		{
			if (container == null) {
				container = this;
			}

			foreach (var widget in widgets) {
				widget.Container = container;
			}
		}

		void AdjustSizes(IEnumerable<T> widgets, int i = 0)
		{
			if (!(Width > 0 && Height > 0)) {
				return;
			}

			foreach (var element in widgets) {
				element.SetDim(X, i, Width, 1);
				i++;
				element.Container = this;
			}
		}

		public override void Redraw()
		{
			Debug.Log("[{0}]", Invalid);
			base.Redraw();

			int height = 0;
			foreach (var element in ObservableCollection) {
				element.Redraw();
				height = element.Y + element.Height;
			}
			Fill(' ', 0, height);
		}

		public override bool ProcessKey(int key)
		{
			switch (key) {
			case 259:
				FocusCollection(ObservableCollection, i => --i);
				break;
			case 258:
				FocusCollection(ObservableCollection, i => ++i);
				break;
			case 339: // page up
				FocusCollection(ObservableCollection, i => i - Height/2);
				break;
			case 338: // down
				FocusCollection(ObservableCollection, i => i + Height/2);
				break;
			case 4: // ctrl + d
				DeleteCurrentCollection(ObservableCollection);
				break;
			}

			EnsureIndexCollection(ObservableCollection);


			return base.ProcessKey(key);
		}

		#if DEBUG
		string value(Empty empty)
		{
			if (empty == null) {
				return "(NULL)";
			} else {
				return empty.Char.ToString();
			}
		}
		#endif

		public void SetCurrentFocus(T widget)
		{
			if (widget == null) {
				throw new ArgumentNullException("widget");
			}

			#if DEBUG
			var current = (CurrentFocus as Empty);
			var next = (widget as Empty);
			Debug.Log("--------> Current: {0} Next: {1}", value(current), value(next));
			#endif

			if (CurrentFocus != null) {
				CurrentFocus.Invalid = true;
				CurrentFocus.HasFocus = false;
				CurrentFocus = null;
			}

			CurrentFocus = widget;
			if (CurrentFocus != null) {
				CurrentFocus.Invalid = true;
				CurrentFocus.HasFocus = true;
			}
		}

		public T CurrentFocus { get; set; }

		public override bool CanFocus {
			get {
				foreach (var widget in ObservableCollection) {
					if (widget.CanFocus) {
						return widget.CanFocus;
					}
				}
				return false;
			}
		}

		public T GetFirstFocusable(IEnumerable<T> widgets)
		{
			foreach (var widget in widgets) {
				if (widget.CanFocus) {
					return widget;
				}
			}
			return null;
		}
	}
}

