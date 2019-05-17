using System;

namespace TFSPeeker
{
	public class ConsoleFormatter : IDisposable
	{
		private readonly ConsoleColor originalBackground;

		private readonly ConsoleColor originalForeground;

		public ConsoleFormatter(ConsoleColor bg, ConsoleColor fg)
		{
			originalBackground = Console.BackgroundColor;
			originalForeground = Console.ForegroundColor;
			Console.BackgroundColor = bg;
			Console.ForegroundColor = fg;
		}

		public void Dispose()
		{
			Console.BackgroundColor = originalBackground;
			Console.ForegroundColor = originalForeground;
		}
	}
}