using System;
using Mono.Terminal;

namespace Test
{
	public class StatusBar : Widget
	{
		public static ColorPair Color { get; protected set; }
		public StatusBar()
		{
			Color = ColorPair.From(-1, 237);
		}

		public override void Redraw()
		{
			Curses.attron(Curses.Attributes.Bold);
			Curses.attron(Color.Attribute);

			Fill(' ');

			//Move(0, 0);

			Set(0, 0, "Awesome wicked text");

			Curses.attroff(Color.Attribute);
			Curses.attroff(Curses.Attributes.Bold);
		}

		public void DrawString(string str, int fg, int bg)
		{

		}
	}

	public class IrssiControl : VBox
	{
		public ViewPort ViewPort { get; protected set; }
		public StatusBar StatusBar { get; protected set; }
		public Entry Entry { get; protected set; }

		public IrssiControl()
		{
			ViewPort = new ViewPort();
			StatusBar = new StatusBar() { Height = 1 };
			Entry = new Entry() { Height = 1 };

			Entry.Enter += delegate {
				if (Entry.Text.Length == 0) {
					return;
				}

				string text = Entry.Text.TrimEnd(new char [] { ' ', '\t' });

				if (text.StartsWith("/quit")) {
					Application.Exit = true;
				} else if (text.StartsWith("/resize")) {
					SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);
					Redraw();
				} else {
					ViewPort.Add(new ViewPortEntry(new ViewPortInfo() {
						DateTime = DateTime.Now,
						Nick = "ToXedVirus",
						Message = text
					}));
					//ViewPort.Position = ViewPort.MinimumPosition;
				}

				Entry.AddHistory(text);

				Entry.Text = "";
				Entry.Position = 0;
			};

			this.Add(ViewPort, Box.Setting.Fill);
			this.Add(StatusBar, Box.Setting.Size);
			this.Add(Entry, Box.Setting.Size);
		}

		public override bool ProcessKey(int key)
		{
			ViewPort.Add(new ViewPortEntry(new ViewPortInfo() {
				DateTime = DateTime.Now,
				Nick = "KeyPress",
				Message = key.ToString()
			}));

			switch (key) {
			case 338:
			case 339:
				return ViewPort.ProcessKey(key);
			default:
				return base.ProcessKey(key);
			}
		}
	}

}

