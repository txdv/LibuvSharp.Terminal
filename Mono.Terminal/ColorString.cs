using System;

namespace Mono.Terminal
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

		public void Draw(Action<char> callback)
		{
			ColorString.Draw(String, callback);
		}

		public void Draw()
		{
			ColorString.Draw(String);
		}

		public void Draw(Widget widget)
		{
			ColorString.Draw(widget, String);
		}

		public void Draw(Widget widget, int x, int y, int w, int h)
		{
			ColorString.Draw(widget, String, x, y, w, h);
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

		public static int Draw(string str, Action<char> callback)
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
						callback(c);
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

		public static int Draw(string str)
		{
			return Draw(str, delegate (char ch) {
				Curses.Add(ch);
			});
		}

		public static int Draw(Widget widget, string str)
		{
			int i = 0;
			int x = 0, y = 0;
			return Draw(str, delegate (char ch) {
				widget.Set(x, y, ch);
				x++;
				if (x >= widget.Width) {
					x = 0;
					y++;
				}
				i++;
			});
		}

		public static int Draw(Widget widget, string str, int x, int y, int w, int h)
		{
			int xi = 0, yi = 0;

			return Draw(str, delegate (char ch) {
				widget.Set(xi + x, yi + y, ch);

				xi++;
				if (xi >= w) {
					xi = 0;
					yi++;
				}
			});
		}
	}

}

