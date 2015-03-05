using System;
using System.Collections.Generic;

namespace LibuvSharp.Terminal
{
	public class AsyncWatcher<T> : IBaseWatcher
	{
		Queue<T> queue = new Queue<T>();
		IAsyncWatcher asyncWatcher;

		public AsyncWatcher(Context context, Action<T> callback)
		{
			asyncWatcher = context.CreateAsyncWatcher(() => {
				T item;
				lock (queue) {
					item = queue.Dequeue();
				}
				if (callback != null) {
					callback(item);
				}
			});
		}

		public void Send(T data)
		{
			lock (queue) {
				queue.Enqueue(data);
			}
			asyncWatcher.Send();
		}

		public void Start()
		{
			asyncWatcher.Start();
		}

		public void Stop()
		{
			asyncWatcher.Stop();
		}

		public bool IsRunning {
			get {
				return asyncWatcher.IsRunning;
			}
		}

		public void Dispose()
		{
			asyncWatcher.Dispose();
		}
	}
}

