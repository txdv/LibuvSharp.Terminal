using System;
using Mono.Terminal;
using Manos.IO;

namespace Test
{
	public class Empty : Widget
	{
		public Empty(char ch)
		{
			Char = ch;
		}

		public char Char { get; set; }
		public override void Redraw ()
		{
			Fill(Char);
		}
	}

	class MainClass
	{
		public static void Main(string[] args)
		{

			Application.Init(Context.Create(Backend.Poll));

			Application.Run(new FullsizeContainer(new Empty('x')));
		}
	}
}
