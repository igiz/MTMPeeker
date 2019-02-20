using System;
using System.Collections.Generic;

namespace TFSPeekerDesktop.Views
{
	public class View : IView
	{

		private readonly IDictionary<string, Func<TestCaseDescription, string>> tokenToResolver = new Dictionary<string, Func<TestCaseDescription, string>> {
			{"{caseId}", point => point.Info.TestCaseId.ToString()},
			{"{caseTitle}" , point => point.Info.TestCaseWorkItem.Title},
			{"{caseState}", point => point.Info.State.ToString()},
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

					Console.WriteLine(displayResult);
				}
			}
		}
	}
}