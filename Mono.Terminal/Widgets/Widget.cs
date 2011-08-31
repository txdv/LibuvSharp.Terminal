namespace Mono.Terminal
{
	public abstract class Widget
	{
		private int x;
		public int X {
			get {
				return x;
			}
			set {
				SetDim(value, y, width, height);
			}
		}

		private int y;
		public int Y {
			get {
				return y;
			}
			set {
				SetDim(x, value, width, height);
			}
		}

		private int width;
		public int Width {
			get {
				return width;
			}
			set {
				SetDim(x, y, value, Height);
			}
		}

		private int height;
		public int Height {
			get {
				return height;
			}
			set {
				SetDim(x, y, Width, value);
			}
		}

		public virtual void SetDim(int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			width = w;
			height = h;
		}

		public Container Container { get; set; }

		public Widget()
		{
			Invalid = true;
		}

		public Widget(int w, int h)
			: this()
		{
			Width = w;
			Height = h;
		}

		public Widget(int x, int y, int w, int h)
			: this(w, h)
		{
			X = x;
			Y = y;
		}

		public virtual bool CanFocus {
			get {
				return false;
			}
		}

		public virtual bool HasFocus { get; set; }

		public abstract void Redraw();
		public virtual bool ProcessKey(int key)
		{
			return false;
		}

		public virtual void SetCursorPosition()
		{
			Move(0, 0);
		}

		protected bool BaseMove(int x, int y)
		{
			if ((x >= 0) && (y >= 0) && (x < Curses.Terminal.Width) && (y < Curses.Terminal.Height)) {
				Window.Standard.Cursor.Move(x, y);
				return true;
			} else {
				return false;
			}
		}

		protected bool Move(int x, int y)
		{
			if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height)) {
				return BaseMove(x + X, y + Y);
			} else {
				return false;
			}
		}

		public void Set(int x, int y, char c)
		{
			if (Move(x, y)) {
				Curses.Add(c);
			}
		}

		public void Set(int x, int y, string str)
		{
			if (Move(x, y)) {
				Curses.Add(str);
			}
		}
		
		public void Fill(char ch)
		{
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					Set(x, y, ch);
				}
			}
		}

		public bool Invalid { get; protected set; }
	}
}
