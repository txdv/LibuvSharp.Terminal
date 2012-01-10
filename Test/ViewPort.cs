using System;
using System.Collections.Generic;
using Mono.Terminal;

namespace Test
{
	public class ViewPortInfo
	{
		public DateTime DateTime { get; set; }
		public string Nick { get; set; }
		public string Message { get; set; }
		public string OP { get; set; }
	}

	public class ViewPortEntry : Widget
	{
		public ViewPort ViewPort { get; internal set; }

		public ColorString Prefix { get; protected set; }
		public ColorString Message { get; protected set; }

		public ViewPortInfo Info { get; protected set; }

		public ViewPortEntry(ViewPortInfo info)
		{
			Mode = true;

			Info = info;

			int AccentColor = 202;

			Prefix = new ColorString(string.Format("{0} {1}\x0000255  ", DecorateTime(AccentColor), DecorateNick(AccentColor, '+')));
			Message = new ColorString(info.Message);
		}

		public int CalculateHeight(int width)
		{
			if (Mode) {
				return (int)Math.Ceiling((double)Message.Length / (width - Prefix.Length));
			} else {
				return (int)Math.Ceiling((double)(Prefix.Length + Message.Length) / width);
			}
		}

		public string DecorateTime(int color)
		{
			return string.Format("\x0000241 (\x0000255 {1}\x0000{0} :\x0000255 {2}\x0000{0} :\x0000255 {3}am\x0000241 )",
				color,
				Info.DateTime.ToString("hh"),
				Info.DateTime.ToString("mm"),
				Info.DateTime.ToString("ss")
			);
		}

		public string DecorateNick(int color, char op) {
			return string.Format("\x0000241 (\x0000{0} {1}\x0000255 {2}\x0000241 )",
				color,
				op,
				Info.Nick
			);
		}

		public bool Mode { get; set; }

		public override void Redraw()
		{
			Fill(' ');

			Move(0, 0);

			if (Mode) {
				Prefix.Draw(this);
				Message.Draw(this, Prefix.Length, 0, Width - Prefix.Length, Height);
			} else {
				Prefix.Draw(this);
				Message.Draw();
				//Message.Draw(this);
			}
		}

		public int Length { get; protected set; }
	}

	public class ViewPort : Widget
	{
		private LinkedList<ViewPortEntry> entries = new LinkedList<ViewPortEntry>();

		public ViewPort()
		{
			PageFactor = 0.8;
		}

		protected LinkedListNode<ViewPortEntry> Position { get; set; }

		public override void SetDim(int x, int y, int w, int h)
		{
			base.SetDim(x, y, w, h);

			Invalid = true;
		}

		public double PageFactor { get; set; }

		public void Add(ViewPortEntry entry)
		{
			Invalid = true;

			entry.ViewPort = this;

			entries.AddLast(entry);
		}

		protected LinkedListNode<ViewPortEntry> PrevPosition(int lines)
		{
			if (Position == null) {
				var current = entries.Last;
				int l = lines;
				while (current.Previous != null && l > 0) {
					l -= current.Value.CalculateHeight(Width);
					current = current.Previous;
				}

				while (current.Previous != null && lines > 0) {
					lines -= current.Value.CalculateHeight(Width);
					current = current.Previous;
				}

				return current;
			} else {
				var current = Position;
				while (current.Previous != null && lines > 0) {
					lines -= current.Value.CalculateHeight(Width);
					current = current.Previous;
				}

				return current;
			}
		}

		protected LinkedListNode<ViewPortEntry> NextPosition(int lines)
		{

			if (Position == null || Position == entries.Last) {
				return null;
			}

			LinkedListNode<ViewPortEntry> current;

			for (current = Position; current != null && lines > 0; current = current.Next) {
				lines -= current.Value.CalculateHeight(Width);
			}

			var ret = current;

			if (ret == entries.Last || ret == null) {
				return null;
			}

			lines = Height;

			for (;lines > 0; current = current.Next) {
				if (current == null) {
					return null;
				}
				lines -= current.Value.CalculateHeight(Width);
			}


			return ret;
		}

		public override bool ProcessKey(int key)
		{
			switch (key) {
			case 339:
				Position = PrevPosition((int)(Height * PageFactor));
				Invalid = true;
				return true;
			case 338:
				if (Position == null) {
					return true;
				}
				Position = NextPosition((int)(Height * PageFactor));
				Invalid = true;
				return true;
			default:
				return false;
			}
		}

		public override void Redraw()
		{
			if (Invalid) {
				Invalid = false;
			} else {
				return;
			}


			ViewPortEntry entry;

			LinkedListNode<ViewPortEntry> element;

			int x = X;
			int h = 0;
			int w = Width;

			if (Position == null) {
				for (element = entries.Last; element != null && h < Height; element = element.Previous) {
					entry = element.Value;
					entry.Height = entry.CalculateHeight(w);
					h += entry.Height;
				}

				if (h < Height) {
					h = 0;
					for (element = entries.First; element != null; element = element.Next) {
						entry = element.Value;

						entry.SetDim(x, h, w, entry.Height);

						h += entry.Height;

						entry.Redraw();
					}
				} else {
					h = Height;
					for (element = entries.Last; element != null; element = element.Previous) {


						entry = element.Value;
						int ch = entry.CalculateHeight(w);
						if (h < 0) {
							return;
						}
						h -= ch;
						entry.SetDim(x, h, w, ch);
						entry.Redraw();
					}
				}
			} else {
				for (element = Position; element != null && h < Height; element = element.Next) {
					entry = element.Value;

					entry.SetDim(x, h, w, entry.CalculateHeight(w));

					entry.Redraw();

					h += entry.Height;
				}
			}
		}

		protected void DrawTop()
		{
		}
	}

}

