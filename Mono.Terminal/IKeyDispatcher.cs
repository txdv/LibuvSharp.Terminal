using System;

namespace Mono.Terminal
{
	public interface IKeyDispatcher
	{
		void Dispatch(int timeout, Action<int> callback);
	}
}

