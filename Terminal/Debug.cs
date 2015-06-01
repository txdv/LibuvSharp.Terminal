using System;
using System.IO;

namespace Terminal
{
	public class Debug
	{
		static StreamWriter sw;
		static Debug()
		{
			sw = new StreamWriter(File.Open("debug", FileMode.Append));
			sw.AutoFlush = true;
		}

		public static void Log(string log)
		{
			sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now, log));
		}

		public static void Log(string log, params object[] parameters)
		{
			Log(string.Format(log, parameters));
		}
	}
}

