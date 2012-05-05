using System;

namespace LibuvSharp.Terminal
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

		public virtual void ForceRedraw()
		{
			Redraw();
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

		public void Fill(string str)
		{
			Fill(str, ' ', 0, 0);
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
			int xi = 0;
			int yi = 0;
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

				Set(xi + x, yi + y, str[i]);

				xi++;
				if (xi >= w) {
					xi = 0;
					yi++;
					if (yi >= h) {
						break;
					}
				}
			}

			endx = xi + x;
			endy = yi + y;
		}

		public void FillWords(string[] words)
		{
			FillWords(words, ' ');
		}
		public void FillWords(string[] words, int x, int y)
		{
			FillWords(words, ' ', x, y);
		}
		public void FillWords(string[] words, int x, int y, int w, int h)
		{
			FillWords(words, ' ', x, y, w, h);
		}
		public void FillWords(string[] words, char ch)
		{
			FillWords(words, ch, 0, 0);
		}
		public void FillWords(string[] words, char ch, int x, int y)
		{
			FillWords(words, ch, x, y, Width, Height);
		}
		public void FillWords(string[] words, char ch, int x, int y, int w, int h)
		{
			int endx, endy;
			DrawWords(words, ch, x, y, w, h, out endx, out endy);
		}

		public void DrawWords(string[] words)
		{
			DrawWords(words, ' ');
		}
		public void DrawWords(string[] words, int x, int y)
		{
			DrawWords(words, ' ', x, y);
		}
		public void DrawWords(string[] words, int x, int y, int w, int h)
		{
			DrawWords(words, ' ', x, y, w, h);
		}
		public void DrawWords(string[] words, int x, int y, int w, int h, out int endx, out int endy)
		{
			DrawWords(words, ' ', x, y, w, h, out endx, out endy);
		}
		public void DrawWords(string[] words, char ch)
		{
			DrawWords(words, ch, 0, 0);
		}
		public void DrawWords(string[] words, char ch, int x, int y)
		{
			DrawWords(words, ch, x, y, Width, Height);
		}
		public void DrawWords(string[] words, char ch, int x, int y, int w, int h)
		{
			int endx, endy;
			DrawWords(words, ch, x, y, w, h, out endx, out endy);

		}
		public void DrawWords(string[] words, char ch, int x, int y, int w, int h, out int endx, out int endy)
		{
			int xi = 0;
			int yi = 0;

			for (int i = 0; i < words.Length; i++) {
				string word = words[i];
				// word fits
				if (xi + word.Length <= w) {
					Draw(word, xi + x, yi + y, w - xi, 1, out xi, out yi);
					xi -= x;
					yi -= yi;
				} else {
					// word doesn't fit in one line
					if (xi != 0) {
						// if we didn't start at the beginning, fill the rest with
						// spaces and start freshly
						for (;xi < w; xi++) {
							Set(x + xi, y + yi, ch);
						}
						xi = 0;
						yi++;
					}
					// just draw for the reminding available height with an already
					// existing function
					int height = (int)System.Math.Ceiling((double)word.Length / w);
					height = System.Math.Min(height, h - yi);
					Draw(word, xi + x, yi + y, w, height, out xi, out yi);
					xi -= x;
					yi -= y;
					if (xi >= w || yi >= h) {
						break;
					}
				}

				// draw a space
				Set(xi + x, yi + y, ch);

				xi++;
				if (xi > w) {
					xi = 0;
					yi++;
					if (yi > h) {
						break;
					}
				}
			}

			endx = x + xi;
			endy = y + yi;
		}
	}
}
