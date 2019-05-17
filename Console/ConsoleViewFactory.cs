using System;
using System.Collections.Generic;
using System.Linq;
using Console.Views;
using TFSPeeker.Interrogation;

namespace Console
{
	public class ConsoleViewFactory
	{
		private readonly DataSet dataset;

		public ConsoleViewFactory(DataSet dataset)
		{
			this.dataset = dataset;
		}

		public IEnumerable<IView> GetViews(IEnumerable<string> views, string keyword)
		{
			List<IView> result = views.Select(viewName => GetView(viewName, keyword))
				.ToList();
			return result;
		}

		public IView GetView(string view, string keyword)
		{
			IView result;
			ViewResult viewResult = ViewResultBuilder.BuildResult(dataset, view, keyword);

			switch (viewResult.Name) {
				case "unassigned":
					result = new View(viewResult.Result) { Foreground = ConsoleColor.DarkRed, Background = ConsoleColor.Black };
					break;
				case "assigned-ready":
					result = new View(viewResult.Result) { Foreground = ConsoleColor.DarkYellow, Background = ConsoleColor.Black };
					break;
				case "complete":
					result = new View(viewResult.Result) { Foreground = ConsoleColor.Green, Background = ConsoleColor.Black };
					break;
				case "priority-1-unassigned":
					result = new View(viewResult.Result) { Foreground = ConsoleColor.Red, Background = ConsoleColor.Black };
					break;
				case "priority-2-unassigned":
					result = new View(viewResult.Result) { Foreground = ConsoleColor.Red, Background = ConsoleColor.Black };
					break;
				default:
					throw new InvalidOperationException($"View {view} specified is not supported");
			}

			return result;
		}
	}
}