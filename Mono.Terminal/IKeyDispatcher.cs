using System;

namespace Mono.Terminal
{
	public interface IKeyDispatcher
	{
		event Action<int> KeyPress;
		void Run(int timeout);
		void Run();
		void Finish();
	}
}

