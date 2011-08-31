using System;

namespace Mono.Terminal
{
	public class FullsizeContainer : Container
	{

		public Widget Widget { get; set; }

		public FullsizeContainer(Widget widget)
			: base()
		{
			if (widget == null) {
				throw new ArgumentException("widget");
			}

			Widget = widget;
			Widget.Container = this;
		}

		public override void Redraw()
		{
			Widget.Redraw();
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

