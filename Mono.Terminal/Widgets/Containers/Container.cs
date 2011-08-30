namespace Mono.Terminal
{
	public abstract class Container : Widget
	{
		public Container()
		{
		}

		public Container(int w, int h)
			: base(w, h)
		{
		}

		public Container(int x, int y, int w, int h)
			: base(x, y, w, h)
		{
		}
	}
}

