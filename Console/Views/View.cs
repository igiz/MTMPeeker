using System;
using System.Collections.Generic;
using TFSPeeker.Interrogation;
using SystemConsole = System.Console;

namespace Console.Views
{
	public class View : IView
	{

		private readonly IDictionary<string, Func<TestCaseDescription, string>> tokenToResolver = new Dictionary<string, Func<TestCaseDescription, string>> {
			{"{caseId}", point => point.Id.ToString()},
			{"{caseTitle}" , point => point.Title},
			{"{caseState}", point => point.State},
			{"{url}", point => point.Url}
		};

		public ConsoleColor Background { get; set; }

		public ConsoleColor Foreground { get; set; }

		public string DisplayFormat { get; set; }

		private readonly IEnumerable<TestCaseDescription> viewResult;

		public View(IEnumerable<TestCaseDescription> viewResult)
		{
			this.Background = ConsoleColor.Black;
			this.Foreground = ConsoleColor.Green;
			this.DisplayFormat = "{caseId}: {caseTitle} Status: {caseState} \nURL: {url}\n";
			this.viewResult = viewResult;
		}

		public void ConsoleOut()
		{
			using (new ConsoleFormatter(Background, Foreground)) {
				foreach (TestCaseDescription testPoint in viewResult) {
					string displayResult = DisplayFormat;

					foreach (var token in tokenToResolver) {
						displayResult = displayResult.Replace(token.Key, token.Value(testPoint));
					}

					SystemConsole.WriteLine(displayResult);
				}
			}
		}
	}
}