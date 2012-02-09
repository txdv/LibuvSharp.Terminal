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

		public bool BaseMove(int x, int y)
		{
			if ((x >= 0) && (y >= 0) && (x < Curses.Terminal.Width) && (y < Curses.Terminal.Height)) {
				Window.Standard.Cursor.Move(x, y);
				return true;
			} else {
				return false;
			}
		}

		public bool Move(int x, int y)
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

		public void Fill(int c)
		{
			Fill(c, 0, 0);
		}
		public void Fill(int c, int x, int y)
		{
			Fill(c, x, y, Width, Height);
		}
		public void Fill(int c, int x, int y, int w, int h)
		{
			Fill(c, x, y, w, h, x, y);
		}
		public void Fill(int c, int x, int y, int w, int h, int startx, int starty)
		{
			for (int i = x; i < x + w; i++) {
				for (int j = y; j < y + h; j++) {
					if ((i >= startx) && (j == starty)) {
						Set(i, j, c);
					} else if (j > starty) {
						Set(i, j, c);
					}
				}
			}
		}

		public void Fill(char c)
		{
			Fill(c, 0, 0);
		}
		public void Fill(char c, int x, int y)
		{
			Fill(c, x, y, Width, Height);
		}
		public void Fill(char c, int x, int y, int w, int h)
		{
			Fill(c, x, y, w, h, x, y);
		}
		public void Fill(char c, int x, int y, int w, int h, int startx, int starty)
		{
			for (int i = x; i < x + w; i++) {
				for (int j = y; j < y + h; j++) {
					if ((i >= startx) && (j == starty)) {
						Set(i, j, c);
					} else if (j > starty) {
						Set(i, j, c);
					}
				}
			}
		}

		public void Fill(string str, int x, int y)
		{
			Fill(str, ' ', x, y);
		}
		public void Fill(string str, char ch, int x, int y)
		{
			Fill(str, ch, x, y, Width, Height);
		}
		public void Fill(string str, int x, int y, int w, int h)
		{
			Fill(str, ' ', x, y, w, h);
		}
		public void Fill(string str, char ch, int x, int y, int w, int h)
		{
			int endx, endy;
			Draw(str, x, y, w, h, out endx, out endy);
			Fill(ch, x, y, w, h, endx, endy);
		}

		public void Draw(string str)
		{
			Draw(str, 0, 0);
		}
		public void Draw(string str, int x, int y)
		{
			Draw(str, x, y, Width - x, Height - y);
		}
		public void Draw(string str, int x, int y, int w, int h)
		{
			int endx, endy;
			Draw(str, x, y, w, h, out endx, out endy);
		}
		public void Draw(string str, int x, int y, int w, int h, out int endx, out int endy)
		{
			int xi = x;
			int yi = y;
			for (int i = 0; i < str.Length; i++) {

				/*
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
				*/

				Set(xi, yi, str[i]);

				xi++;

				if (xi > x + w) {
					if (yi > y + h) {
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
	}
}
