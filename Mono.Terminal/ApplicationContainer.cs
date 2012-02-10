using System;

namespace Mono.Terminal
{
	class ApplicationContainer : Container
	{
		public bool DrawingLoop { get; set; }

		public new Container Container { get; protected set; }

		public bool Drawing { get; protected set; }

		public ApplicationContainer(Container container)
		{
			Container = container;
			Container.Container = this;
		}

		public override bool Invalid {
			get {
				return base.Invalid;
			}
			set {
				if (!Drawing && value) {
					base.Invalid = value;
				}
			}
		}

		public override void Redraw()
		{
			Drawing = true;
			base.Redraw();
			Container.Redraw();
			Drawing = false;
		}

		public override void ForceRedraw()
		{
			base.ForceRedraw();
			if (Container != null) {
				Container.ForceRedraw();
			}
		}

		public override bool ProcessKey(int key)
		{
			return Container.ProcessKey(key);
		}

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);
			Container.SetDim(x, y, w, h);
		}

		public override bool CanFocus {
			get {
				return Container.CanFocus;
			}
		}

		public override bool HasFocus {
			get {
				return Container.HasFocus;
			}
			set {
				Container.HasFocus = value;
			}
		}

		public override void SetCursorPosition()
		{
			Container.SetCursorPosition();
		}
	}

}

