using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Terminal;
using Mono.Terminal.Poll;
using Mono.Interop;

namespace Test
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();

			Loop loop = new Loop();
			PollKeyDispatcher pkd = new PollKeyDispatcher(loop);

			var irssi = new IrssiControl();

			for (int i = 0; i < 100000; i++) {
				irssi.ViewPort.Add(new ViewPortEntry(new ViewPortInfo() {
					DateTime = DateTime.Now,
					Nick = "ToXedVirus",
					Message = str(string.Format("A very \x0000{0} long\x0000  test string! {0} ", i % 256), i % 10)
				}));
			}

			Application.Run(pkd, irssi);
		}

		public static string str(string str, int n)
		{
			string ret = string.Empty;
			for (int i = 0; i < n; i++) {
				ret += str;
			}
			return ret;
		}
	}
}
