using System;
using SystemConsole = System.Console;

namespace Console
{
	public class ConsoleFormatter : IDisposable
	{
		private readonly ConsoleColor originalBackground;

		private readonly ConsoleColor originalForeground;

		public ConsoleFormatter(ConsoleColor bg, ConsoleColor fg)
		{
			originalBackground = SystemConsole.BackgroundColor;
			originalForeground = SystemConsole.ForegroundColor;
			SystemConsole.BackgroundColor = bg;
			SystemConsole.ForegroundColor = fg;
		}

		public void Dispose()
		{
			SystemConsole.BackgroundColor = originalBackground;
			SystemConsole.ForegroundColor = originalForeground;
		}
	}
}