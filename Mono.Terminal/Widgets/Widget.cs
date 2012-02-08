using System;

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

		private bool invalid = false;
		public virtual bool Invalid {
			get {
				return invalid;
			}
			set {
				if (value) {
					if (Container != null && !Container.Invalid) {
						Container.Invalid = true;
					}
				}
				invalid = value;
			}
		}

		public bool Visible {
			get {
				return true;
			}
		}

		public virtual void Redraw()
		{
			Invalid = false;
		}

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

		public void Set(int x, int y, int c)
		{
			if (Move(x, y)) {
				Curses.Add(c);
			}
		}
		public void Set(int x, int y, char c)
		{
			if (Move(x, y)) {
				Curses.Add(c);
			}
		}
		public void Set(int x, int y, int w, int h, int c)
		{
			for (int i = x; i < w; i++) {
				for (int j = y; j < h; j++) {
					Set(i, j, c);
				}
			}
		}
		public void Set(int x, int y, int w, int h, char c)
		{
			for (int i = x; i < w; i++) {
				for (int j = y; j < h; j++) {
					Set(i, j, c);
				}
			}
		}

		public void Set(int x, int y, string str)
		{
			if (Move(x, y)) {
				Curses.Add(str);
			}
		}

		public void Set(int x, int y, int w, int h, string str, out int endx, out int endy)
		{
			int xi = x;
			int yi = y;
			for (int i = 0; i < str.Length; i++) {
				if (str[i] == '\n') {
					xi = x;
					yi++;
					if (yi > h) {
						endx = xi;
						endy = yi;
						return;
					}
					continue;
				}

				Set(xi, yi, str[i]);

				xi++;

				if (xi > w) {
					if (yi > h) {
						endx = xi;
						endy = yi;
						return;
					}
					xi = x;
					yi++;
				}
			}
			endx = xi;
			endy = yi;
		}

		public void Set(int x, int y, int w, int h, string str)
		{
			int endx, endy;
			Set(x, y, w, h, str, out endx, out endy);
		}
		
		public void Fill(int ch)
		{
			Fill(0, 0, ch);
		}
		public void Fill(char ch)
		{
			Fill(0, 0, ch);
		}
		public void Fill(int x, int y, int ch)
		{
			for (int i = x; i < Width; i++) {
				for (int j = y; j < Height; j++) {
					Set(i, j, ch);
				}
			}
		}
		public void Fill(int x, int y, char ch)
		{
			for (int i = x; i < Width; i++) {
				for (int j = y; j < Height; j++) {
					Set(i, j, ch);
				}
			}
		}

		public void Fill(int x, int y, int w, int h, string str, char ch)
		{
			int endx, endy;
			Set(x, y, w, h, str, out endx, out endy);
			Fill(endx, endy, ch);
		}
		public void Fill(int x, int y, int w, int h, string str)
		{
			Fill(x, y, w, h, str, ' ');
		}

		public void Fill(int x, int y, string str, char ch)
		{
			Fill(x, y, Width, Height, str, ch);
		}
		public void Fill(int x, int y, string str)
		{
			Fill(x, y, str, ' ');
		}
		public void Fill(string str, char ch)
		{
			Fill(0, 0, str, ch);
		}
		public void Fill(string str)
		{
			Fill(str, ' ');
		}
	}
}
