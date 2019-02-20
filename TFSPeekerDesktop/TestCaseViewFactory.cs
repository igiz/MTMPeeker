using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.TestManagement.Client;
using TFSPeekerDesktop.Views;

namespace TFSPeekerDesktop
{
	public class TestCaseViewFactory
	{
		#region Private Fields

		private readonly string TestCaseUrlBase = "{0}/{1}/_testmanagement?planId={2}&suiteId={3}";

		private readonly IDictionary<int, ITestCaseResult> previousResults = new Dictionary<int, ITestCaseResult>();
		private readonly IDictionary<int, ISet<TestCaseDescription>> testCasePriority = new Dictionary<int, ISet<TestCaseDescription>>();

		private readonly ISet<TestCaseDescription> unassigned = new HashSet<TestCaseDescription>();
		private readonly ISet<TestCaseDescription> assigned = new HashSet<TestCaseDescription>();
		private readonly ISet<TestCaseDescription> automated = new HashSet<TestCaseDescription>();

		private readonly ISet<TestCaseDescription> ready = new HashSet<TestCaseDescription>();
		private readonly ISet<TestCaseDescription> inprogress = new HashSet<TestCaseDescription>();
		private readonly ISet<TestCaseDescription> complete = new HashSet<TestCaseDescription>();

		#endregion

		#region Initialize current test plan

		public void Init(ITestPlan testPlan, ITestManagementTeamProject testProject, string tfsUrl, string project)
		{
			ITestPointCollection testPlanPoints = testPlan.QueryTestPoints("SELECT * FROM TestPoint");

			foreach (ITestPoint item in testPlanPoints)
			{
				int testCaseId = item.TestCaseId;
				int priority = item.TestCaseWorkItem.Priority;
				string testCaseUrl = string.Format(TestCaseUrlBase, tfsUrl, project, item.Plan.Id, item.SuiteId);
				TestCaseDescription testCaseDescription = new TestCaseDescription(item, testCaseUrl);

				ITestCaseResult previousResult = testProject.TestResults.ByTestId(testCaseId)
					.FirstOrDefault();

				if (previousResult != null) {
					previousResults[testCaseId] = previousResult;
				}

				if (!testCasePriority.ContainsKey(priority)) {
					testCasePriority[priority] = new HashSet<TestCaseDescription>();
				}

				testCasePriority[priority].Add(testCaseDescription);

				if (item.IsTestCaseAutomated) {
					automated.Add(testCaseDescription);
				}

				switch (item.State)
				{
					case TestPointState.InProgress:
						inprogress.Add(testCaseDescription);
						break;
					case TestPointState.Ready:
						ready.Add(testCaseDescription);
						break;
					case TestPointState.Completed:
						complete.Add(testCaseDescription);
						break;
				}

				switch (item.AssignedToName)
				{
					case "Unassigned":
						unassigned.Add(testCaseDescription);
						break;
					case "Automation User":
						automated.Add(testCaseDescription);
						break;
					default:
						assigned.Add(testCaseDescription);
						break;
				}
			}
		}

		#endregion

		public IEnumerable<IView> GetViews(IEnumerable<string> views)
		{
			List<IView> result = views.Select(GetView)
				.ToList();

			return result;
		}

		public IView GetView(string view)
		{
			IView result = null;

			switch (view) {
				case "unassigned":
					result = new View(unassigned.Except(automated)) {Foreground = ConsoleColor.DarkRed , Background = ConsoleColor.Black};
					break;
				case "assigned-ready":
					result = new View(assigned.Intersect(ready).Except(automated)) {Foreground = ConsoleColor.DarkYellow, Background = ConsoleColor.Black};
					break;
				case "complete":
					result = new View(complete.Except(automated)) {Foreground = ConsoleColor.Green, Background = ConsoleColor.Black};
					break;
				case "priority-1-unassigned":
					result = new View(testCasePriority[1].Intersect(unassigned).Except(automated)) { Foreground = ConsoleColor.Red, Background = ConsoleColor.Black };
					break;
				case "priority-2-unassigned":
					result = new View(testCasePriority[2].Intersect(unassigned).Except(automated)) { Foreground = ConsoleColor.Red, Background = ConsoleColor.Black };
					break;
				default:
					throw new InvalidOperationException($"View {view} specified is not supported");
			}

			return result;
		}
	}
}