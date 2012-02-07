using System;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using Manos.IO;

namespace Mono.Terminal
{
	public class SignalWatcher : IBaseWatcher
	{
		Thread thread;
		UnixSignal[] unixSignals;
		AsyncWatcher<Signum> watcher;

		public Signum[] Signals { get; protected set; }

		public SignalWatcher(Context context, Signum signum, Action callback)
			: this(context, new Signum[] { signum }, (num) => {
				if (callback != null) {
					callback();
				}
			})
		{
		}

		public SignalWatcher(Context context, Signum[] signals, Action<Signum> callback)
		{
			Signals = signals;
			unixSignals = new UnixSignal[signals.Length];
			for (int i = 0; i < signals.Length; i++) {
				unixSignals[i] = new UnixSignal(signals[i]);
			}

			watcher = new AsyncWatcher<Signum>(context, (key) => {
				if (callback != null) {
					callback(key);
				}
			});

			thread = new Thread((o) => {
				while (true) {
					var index = UnixSignal.WaitAny(unixSignals);
					watcher.Send(Signals[index]);
				}
			});
		}

		public void Start()
		{
			watcher.Start();
			thread.Start();
		}

		public void Stop()
		{
			watcher.Stop();
			thread.Abort();
		}

		public bool IsRunning {
			get {
				return watcher.IsRunning;
			}
		}

		public void Dispose()
		{
			watcher.Dispose();
		}
	}
}

