using System;
using System.Text;

namespace LibuvSharp.Terminal
{
	public class ColorString
	{
		public string String { get; protected set; }
		public int Length { get; protected set; }

		public ColorString(string str)
		{
			String = str;
			Length = CalculateLength(str);
		}

		public void Draw(Func<char, bool> callback)
		{
			ColorString.Each(String, callback);
		}

		public int Draw(Widget widget)
		{
			return Draw(widget, String);
		}
		public int Draw(Widget widget, out int endx, out int endy)
		{
			return Draw(widget, String, out endx, out endy);
		}
		public int Draw(Widget widget, int x, int y)
		{
			return Draw(widget, String, x, y);
		}
		public int Draw(Widget widget, int x, int y, out int endx, out int endy)
		{
			return Draw(widget, String, x, y, out endx, out endy);
		}
		public int Draw(Widget widget, int x, int y, int w, int h)
		{
			return Draw(widget, String, x, y, w, h);
		}
		public int Draw(Widget widget, int x, int y, int w, int h, out int endx, out int endy)
		{
			return Draw(widget, String, x, y, w, h, out endx, out endy);
		}

		public int Fill(Widget widget)
		{
			return Fill(widget, ' ');
		}
		public int Fill(Widget widget, char ch)
		{
			return Fill(widget, ch, 0, 0);
		}
		public int Fill(Widget widget, int x, int y)
		{
			return Fill(widget, ' ', x, y);
		}
		public int Fill(Widget widget, char ch, int x, int y)
		{
			return Fill(widget, String, ch, x, y, widget.Width - x, widget.Height - y);
		}
		public int Fill(Widget widget, int x, int y, int w, int h)
		{
			return Fill(widget, ' ', x, y, w, h);
		}
		public int Fill(Widget widget, char ch, int x, int y, int w, int h)
		{
			return Fill(widget, String, ch, x, y, w, h);
		}

		enum DrawStringMode {
			Normal,
			ForegroundColor,
			Background
		}

		public static int CalculateLength(string str)
		{
			DrawStringMode mode = DrawStringMode.Normal;
			int len = 0;
			foreach (char c in str) {
				switch (mode) {
				case DrawStringMode.Normal:
					if (c == '\x0000') {
						mode = DrawStringMode.ForegroundColor;
					} else {
						len++;
					}
					break;
				case DrawStringMode.ForegroundColor:
					if (c == ' ') {
						mode = DrawStringMode.Normal;
					}
					break;
				}
			}
			return len;
		}

		public static int Each(string str, Func<char, bool> callback)
		{
			int n = 0;

			DrawStringMode mode = DrawStringMode.Normal;

			string fg = string.Empty;
			string bg = string.Empty;

			foreach (char c in str) {
				switch (mode) {
				case DrawStringMode.Normal:
					if (c == '\x0000') {
						mode = DrawStringMode.ForegroundColor;
					} else {
						if (!callback(c)) {
							return n;
						}
						n++;
					}
					break;
				case DrawStringMode.ForegroundColor:
					if (char.IsNumber(c)) {
						fg += c;
					} else if (c == ',') {
						mode = DrawStringMode.Background;
					} else if (c == ' ') {
						ushort foreground = ushort.MaxValue;
						if (fg != string.Empty) {
							foreground = ushort.Parse(fg);
						}
						Curses.attron(ColorPair.From(foreground, ushort.MaxValue).Attribute);
						fg = string.Empty;
						mode = DrawStringMode.Normal;
					} else {
						throw new Exception("Malformed color string");
					}
					break;
				case DrawStringMode.Background:
					if (char.IsNumber(c)) {
						bg += c;
					} else if (c == ' ') {
						Curses.attron(ColorPair.From(ushort.Parse(fg), ushort.Parse(bg)).Attribute);
						fg = string.Empty;
						bg = string.Empty;
						mode = DrawStringMode.Normal;
					} else {
						throw new Exception("Malformed color string");
					}
					break;
				}
			}
			return n;
		}

		public static int Draw(Widget widget, string str)
		{
			return Draw(widget, str, 0, 0);

		}
		public static int Draw(Widget widget, string str, out int endx, out int endy)
		{
			return Draw(widget, str, 0, 0, out endx, out endy);
		}
		public static int Draw(Widget widget, string str, int x, int y)
		{
			return Draw(widget, str, x, y, widget.Width, widget.Height);
		}
		public static int Draw(Widget widget, string str, int x, int y, out int endx, out int endy)
		{
			return Draw(widget, str, x, y, widget.Width - x, widget.Height - y, out endx, out endy);
		}
		public static int Draw(Widget widget, string str, int x, int y, int w, int h)
		{
			int endx, endy;
			return Draw(widget, str, x, y, w, h, out endx, out endy);
		}
		public static int Draw(Widget widget, string str, int x, int y, int w, int h, out int endx, out int endy)
		{
			int xi = 0, yi = 0;
			int length = Each(str, delegate (char ch) {
				widget.Set(xi + x, yi + y, ch);

				xi++;
				if (xi >= w) {
					xi = 0;
					yi++;
					if (yi >= h) {
						return false;
					}
				}
				return true;
			});
			endx = xi + x;
			endy = yi + y;
			return length;
		}

		public static int Fill(Widget widget, string str)
		{
			return Fill(widget, str, ' ', 0, 0);
		}
		public static int Fill(Widget widget, string str, char ch)
		{
			return Fill(widget, str, ch, 0, 0);
		}
		public static int Fill(Widget widget, string str, int x, int y)
		{
			return Fill(widget, str, ' ', x, y);
		}
		public static int Fill(Widget widget, string str, char ch, int x, int y)
		{
			return Fill(widget, str, ch, x, y, widget.Width - x, widget.Height - y);
		}
		public static int Fill(Widget widget, string str, int x, int y, int w, int h)
		{
			return Fill(widget, str, ' ', x, y, w, h);
		}
		public static int Fill(Widget widget, string str, char ch, int x, int y, int w, int h)
		{
			int endx, endy;
			int length = Draw(widget, str, x, y, w, h, out endx, out endy);
			widget.Fill(ch, x, y, w, h, endx, endy);
			return length;
		}

		public static ColorString Escape(string str, Func<char, ColorPair> exchange)
		{
			if (str == null) {
				return null;
			} else if (str.Length == 0) {
				return null;
			}

			char ch = str[0];
			ColorPair current = exchange(ch);
			StringBuilder sb = new StringBuilder(current.ToString());
			sb.Append(current);
			sb.Append(ch);
			for (int i = 1; i < str.Length; i++) {
				ch = str[i];
				ColorPair next = exchange(ch);
				if (current != next) {
					sb.Append(next.ToString());
					current = next;
				}
				sb.Append(ch);
			}
			return new ColorString(sb.ToString());
		}

		public static void Finish()
		{
			Curses.attron(ColorPair.From(-1, -1).Attribute);
		}
	}

}

