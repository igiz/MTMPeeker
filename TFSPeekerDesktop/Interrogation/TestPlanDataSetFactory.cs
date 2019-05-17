using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TFSPeeker.Interrogation
{
	public class TestPlanDataSetFactory : IObservable<DataSet>
	{
		#region Private Fields

		private readonly IList<IObserver<DataSet>> subscribers = new List<IObserver<DataSet>>();

		private readonly string TestCaseUrlBase = "{0}/{1}/_testmanagement?planId={2}&suiteId={3}";

		private DataSet dataSet = new DataSet();

		#endregion

		#region Public Methods

		public void Initialize(ITestPlan testPlan, ITestManagementTeamProject testProject, string tfsUrl, string project)
		{
			ITestPointCollection testPlanPoints = testPlan.QueryTestPoints("SELECT * FROM TestPoint");
			dataSet = new DataSet();

			foreach (ITestPoint item in testPlanPoints) {
				int priority = item.TestCaseWorkItem.Priority;
				string testCaseUrl = string.Format(TestCaseUrlBase, tfsUrl, project, item.Plan.Id, item.SuiteId);
				TestCaseDescription testCaseDescription = new TestCaseDescription(item, testCaseUrl);

				dataSet.AddCasePriority(priority, testCaseDescription);

				if (item.IsTestCaseAutomated) {
					dataSet.Automated.Add(testCaseDescription);
				}

				switch (item.State) {
					case TestPointState.InProgress:
						dataSet.InProgress.Add(testCaseDescription);
						break;
					case TestPointState.Ready:
						dataSet.Ready.Add(testCaseDescription);
						break;
					case TestPointState.Completed:
						dataSet.Complete.Add(testCaseDescription);
						break;
				}

				switch (item.AssignedToName) {
					case "Unassigned":
						dataSet.Unassigned.Add(testCaseDescription);
						break;
					case "Automation User":
						dataSet.Automated.Add(testCaseDescription);
						break;
					default:
						dataSet.Assigned.Add(testCaseDescription);
						break;
				}
			}

			DatasetChanged();
		}
		
		public IDisposable Subscribe(IObserver<DataSet> subscriber)
		{
			if (!subscribers.Contains(subscriber)) {
				subscribers.Add(subscriber);
			}
			return new SubscriptionManager<DataSet>(subscribers, subscriber);
		}

		#endregion

		#region Non-Public Methods

		private void DatasetChanged()
		{
			foreach (IObserver<DataSet> subscriber in subscribers) {
				subscriber.OnNext(dataSet);
			}
		}

		#endregion
	}
}