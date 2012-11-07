using System;
using System.IO;
using LibuvSharp;
using LibuvSharp.Terminal;
using System.Text;

namespace Fancontrol
{
	class Fanbar : Widget
	{
		public override bool CanFocus {
			get {
				return true;
			}
		}

		UVTimer timer;

		public string Root { get; protected set; }
		public int ID { get; protected set; }

		int val = -1;
		public int Value {
			get {
				return val;
			}
			protected set {
				if (value >= Min && value <= Max) {
					val = value;
				}
			}
		}

		public int Min {
			get {
				return 0;
			}
		}

		public int Max {
			get {
				return 255;
			}
		}

		public string PWM {
			get {
				return Path.Combine(Root, "pwm" + ID);
			}
		}

		public string FanInput {
			get {
				return Path.Combine(Root, "fan" + ID + "_input"); 
			}
		}

		public int RPM { get; protected set; }

		public Fanbar(string root, int id)
		{
			Root = root;
			ID = id;
			Value = -1;

			timer = new UVTimer(Application.Loop);

			Get(PWM, (val) => {
				Invalid = true;
				Value = val;
			});


			timer.Start(TimeSpan.FromSeconds(1), () => {
				Get(FanInput, (val) => RPM = val);
			});
		}

		private void Get(string filename, Action<int> callback)
		{
			UVFile.Open(filename, UVFileAccess.Read, (e, file) => {
				byte[] r = new byte[512];
				file.Read(r, r.Length, 0, (e2, size) => {
					if (callback != null) {
						callback(int.Parse(Encoding.ASCII.GetString(r, 0, size)));
					}
				});
			});
		}

		private void Set(int value, Action<bool> callback)
		{
			UVFile.Open(PWM, UVFileAccess.Write, (e, file) => {
				if (e != null) {
					callback(false);
					return;
				}

				file.Write(Encoding.ASCII, value.ToString(), (e2, length) => {
					if (e2 != null) {
						callback(false);
						return;
					}
					if (callback != null) {
						callback(true);
					}
				});
			});
		}

		public double Percentage {
			get {
				return ((double)Value)/Max;
			}
		}

		public int Spaces {
			get {
				return (int)(Percentage * (Width - 2));
			}
		}

		public override void Redraw()
		{
			Fill(' ');

			base.Redraw();

			Curses.attron(Curses.Attributes.Bold);
			Set(0, 0, '[');
			Curses.attroff(Curses.Attributes.Bold);

			for (int i = 0; i < Spaces; i++) {
				Set(i + 1, 0, '|');
			}

			Curses.attron(Curses.Attributes.Bold);
			Set(Width - 1, 0, ']');


			string text = string.Format("{0}RPM {1}%", RPM, Math.Floor(Percentage * 100));
			Draw(text, Width - text.Length - 1, 0);
			Curses.attroff(Curses.Attributes.Bold);

		}

		public void Update(int value)
		{
			Set(value, (success) => {
				if (success) {
					Get(PWM, (val) => {
						Invalid = true;
						Value = val;
					});
				}
			});
		}

		public override bool ProcessKey (int key)
		{
			switch (key) {
			case 44:
				Update(Value - 5);
				return true;
			case 46:
				Update(Value + 5);
				return true;
			case 260:
				Update(Value - 1);
				return true;
			case 261:
				Update(Value + 1);
				return true;
			default:
				return base.ProcessKey (key);
			}
		}

		public override void SetCursorPosition()
		{
			Move(0, 0);
		}

	}

	class FanbarContainer : VBox
	{
		public string Root { get; protected set; } 

		public FanbarContainer(string root)
		{
			Root = root;
		}

		public void Populate()
		{
			for (int i = 1; File.Exists(Root + "pwm" + i); i++) {
				this.Add(new Fanbar(Root, i) { Height = 1 }, (i == 1 ? Box.Setting.Fill : Box.Setting.Size));
			}
		}
	}

	class MainClass
	{
		public static bool IsRoot {
			get {
				return Environment.GetEnvironmentVariable("USER") == "root";
			}
		}

		public static void Main(string[] args)
		{
			Application.Init();

			var fc = new FanbarContainer("/sys/class/hwmon/hwmon1/device/");
			fc.Populate();

			Application.Run(new FullsizeContainer(fc));
			Application.Exit = true;
		}
	}
}
