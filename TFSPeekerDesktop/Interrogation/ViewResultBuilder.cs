using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace TFSPeeker.Interrogation
{
	public class ViewResultBuilder
	{
		public static IEnumerable<string> AllSupportedViews => new []{ "unassigned" , "assigned-ready" , "priority-1-unassigned" , "priority-2-unassigned", "priority-3-unassigned", "priority-4-unassigned" };

		public static ViewResult BuildResult(DataSet dataset, string viewName, string keyword)
		{
			IEnumerable<TestCaseDescription> result = null;

			switch (viewName)
			{
				case "unassigned":
					result = dataset.Unassigned.Except(dataset.Automated);
					break;
				case "assigned-ready":
					result = dataset.Assigned.Intersect(dataset.Ready).Except(dataset.Automated);
					break;
				case "priority-1-unassigned":
					result = Priority(dataset.TestCasePriority, 1).Intersect(dataset.Unassigned).Except(dataset.Automated);
					break;
				case "priority-2-unassigned":
					result = Priority(dataset.TestCasePriority, 2).Intersect(dataset.Unassigned).Except(dataset.Automated);
					break;
				case "priority-3-unassigned":
					result = Priority(dataset.TestCasePriority, 3).Intersect(dataset.Unassigned).Except(dataset.Automated);
					break;
				case "priority-4-unassigned":
					result = Priority(dataset.TestCasePriority, 4).Intersect(dataset.Unassigned).Except(dataset.Automated);
					break;
				default:
					throw new InvalidOperationException($"View {viewName} specified is not supported");
			}

			//Filter out by keyword
			if (!string.IsNullOrWhiteSpace(keyword)) {
				result = result.Where(description => Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(description.Title, keyword, CompareOptions.IgnoreCase) >= 0);
			}

			return new ViewResult{Name = viewName, Result = result};
		}

		private static ISet<TestCaseDescription> Priority(IDictionary<int, ISet<TestCaseDescription>> collection, int priority) {
			ISet<TestCaseDescription> result = collection.ContainsKey(priority) ? collection[priority] : new HashSet<TestCaseDescription>();
			return result;
		}

	}
}