using System;

namespace Terminal
{
	public class FullsizeContainer : Container
	{
		public Widget widget = null;
		public Widget Widget {
			get { return widget; }
			set {
				widget = value;
				widget.Container = this;

				// TODO: fix here, check if application is running?
				if (Width > 0 && Height > 0) {
					widget.SetDim(X, Y, Width, Height);
					Fill(' ');
					widget.Redraw();
				}
			}
		}

		public FullsizeContainer(Widget widget)
			: base()
		{
			if (widget == null) {
				throw new ArgumentException("widget");
			}

			Widget = widget;

			if (widget != null && widget.Invalid) {
				Invalid = true;
			}
		}

		public override void Redraw()
		{
			base.Redraw();
			Widget.Redraw();
		}

		public override void ForceRedraw()
		{
			base.ForceRedraw();
			if (widget != null) {
				widget.ForceRedraw();
			}
		}

		public override bool ProcessKey(int key)
		{
			return Widget.ProcessKey(key);
		}

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);
			Widget.SetDim(x, y, w, h);
		}

		public override bool CanFocus {
			get {
				return Widget.CanFocus;
			}
		}

		public override void SetCursorPosition()
		{
			Widget.SetCursorPosition();
		}
	}
}

