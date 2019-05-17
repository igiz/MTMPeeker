using System;
using System.Collections.Generic;
using Console.Views;
using TFSPeeker;
using TFSPeeker.Interrogation;
using SystemConsole = System.Console;

namespace Console
{
	public class ConsoleApplication : IApplication
	{
		private readonly ApplicationContext context;

		private IDisposable subscription;

		public ConsoleApplication(ApplicationContext context)
		{
			this.context = context;
		}

		public void Run(IInterrogator<DataSet> interrogator)
		{
			subscription = interrogator.DataProvider.Subscribe(this);

			interrogator.Start();

			string command;
			do {
				SystemConsole.WriteLine();
				using (new ConsoleFormatter(ConsoleColor.DarkRed, ConsoleColor.White)) {
					SystemConsole.WriteLine("Type 'quit' to exit the application");
				}

				using (new ConsoleFormatter(ConsoleColor.Gray, ConsoleColor.White)) {
					SystemConsole.WriteLine("Type 'refresh' to refresh views");
				}

				command = SystemConsole.ReadLine();

				if (command == "refresh") {
					interrogator.Refresh();
				}

			} while (command != "quit");

			interrogator.Stop();
		}

		public void Dispose()
		{
			subscription.Dispose();
			subscription = null;
		}

		public void OnNext(DataSet value)
		{
			ConsoleViewFactory factory = new ConsoleViewFactory(dataset: value);
			IEnumerable<IView> consoleViews = factory.GetViews(context.SelectedViews, context.Keyword);

			foreach (IView consoleView in consoleViews) {
				consoleView.ConsoleOut();
			}
		}

		public void OnError(Exception error)
		{
			throw error;
		}

		public void OnCompleted()
		{
			Dispose();
		}
	}
}