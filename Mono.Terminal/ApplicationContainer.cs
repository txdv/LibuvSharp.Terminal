using System;

namespace Mono.Terminal
{
	class ApplicationContainer : Container
	{
		public bool DrawingLoop { get; set; }

		public new Container Container { get; protected set; }

		public ApplicationContainer(Container container)
		{
			Container = container;
		}

		public override bool Invalid {
			get {
				return base.Invalid;
			}
			set {
				if (!value) {
					Application.Refresh();
				}
				base.Invalid = value;
			}
		}

		public override void Redraw ()
		{
			Container.Redraw();
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

