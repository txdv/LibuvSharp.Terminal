using System;
using System.Runtime.InteropServices;

namespace Mono.Interop
{
	enum ModuleFlags : int
	{
		LAZY = 1,
		NOW = 2,
		BINDING_MASK = 3,
		NOLOAD = 4,
		DEEPBIND = 8,
		GLOBAL = 0x00100,
		LOCAL = 0,
		NODELETE = 0x01000,
	}

	public class Module
	{
		public static Module Open()
		{
			return new Module(null);
		}

		public static Module Open(string filename)
		{
			if (filename == null) {
			}

			string fullfilename = null;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				fullfilename = string.Format("lib{0}.so", filename);
			} else {
				throw new Exception("Platform not supported");
			}

			return new Module(fullfilename);
		}

		private IntPtr handle = IntPtr.Zero;

		private Module(string filename)
			: this(dlopen(filename, ModuleFlags.LAZY))
		{
		}

		internal Module(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero) {
				throw new Exception(dlerror());
			}

			handle = ptr;
		}

		public IntPtr GetSymbol(string symbolName)
		{
			EmptyErrors();
			IntPtr ptr = dlsym(handle, symbolName);
			CheckError();
			return ptr;
		}

		public bool TryGetSymbol(string symbolName, out IntPtr ptr)
		{
			try {
				ptr = GetSymbol(symbolName);
				return true;
			} catch {
				return false;
			}
		}

		private void EmptyErrors()
		{
			while (GetLastError() != null) { }
		}

		private string GetLastError()
		{
			return dlerror();
		}

		private void CheckError()
		{
			string err = GetLastError();
			if (err != null) {
				throw new Exception(err);
			}
		}

		[DllImport("__Internal")]
		private static extern IntPtr dlopen(string filename, ModuleFlags flag);

		[DllImport("__Internal", EntryPoint = "dlopen")]
		private static extern IntPtr dlopen2(IntPtr filename, ModuleFlags flag);

		[DllImport("__Internal")]
		private static extern string dlerror();

		[DllImport("__Internal")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("__Internal")]
		private static extern int dlclose(IntPtr handle);
	}


}

