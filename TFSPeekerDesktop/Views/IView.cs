using System;

namespace TFSPeekerDesktop.Views
{
	public interface IView
	{
		ConsoleColor Background { get; set; }

		ConsoleColor Foreground { get; set; }

		string DisplayFormat { get; set; }

		void ConsoleOut();
	}
}