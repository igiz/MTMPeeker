using System;
using System.Threading;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TFSPeeker.Interrogation
{
	public class TfsApiInterrogator : IInterrogator<DataSet>
	{
		private Thread taskThread;

		private ApplicationContext context;

		private readonly object handle = new object();

		private readonly TestPlanDataSetFactory testPlanDataSetFactory;

		#region Public Properties

		public IObservable<DataSet> DataProvider => testPlanDataSetFactory;

		#endregion

		#region Constructors

		public TfsApiInterrogator(ApplicationContext context)
		{
			this.context = context;
			this.testPlanDataSetFactory = new TestPlanDataSetFactory();
		}

		#endregion

		#region Public Methods

		public void Start()
		{
			if (taskThread == null) {
				taskThread = CreateTaskThread();
				taskThread.Start();
			}
		}

		public void Stop()
		{
			if (taskThread != null) {
				taskThread.Abort();
				taskThread = null;
			}
		}

		public void Refresh()
		{
			taskThread?.Interrupt();
		}

		public void SwitchContext(ApplicationContext newContext)
		{
			lock (handle) {
				this.context = newContext;
			}
			Refresh();
		}

		#endregion

		#region Private Methods

		private Thread CreateTaskThread()
		{
			return new Thread(() => {
				while (true) {
					try {
						using (new ConsoleFormatter(ConsoleColor.DarkGreen, ConsoleColor.White)) {
							Console.WriteLine($"Refreshing data at: {DateTime.Now.ToShortTimeString()}");
						}

						lock (handle) {
							using (TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(context.ApiUrl))) {
								ITestManagementService service =
									tfs.GetService(typeof(ITestManagementService)) as ITestManagementService;
								ITestManagementTeamProject testProject = service.GetTeamProject(context.Project);

								//Can extract information about the test suite from here
								ITestSuiteBase testSuite = testProject.TestSuites.Find(context.SuiteId);

								//This is a given instance of the test suite , I.E the test plan (suites can be re-used)
								ITestPlan testPlan = testProject.TestPlans.Find(context.PlanId);

								Console.WriteLine($"Test Suite: {testSuite.Title} \n Description: {testPlan.Description} \n Last Updated: {testPlan.LastUpdated}");

								testPlanDataSetFactory.Initialize(testPlan, testProject, context.ApiUrl, context.Project);
							}
						}

						Thread.Sleep(TimeSpan.FromMinutes(context.RefreshTime));
					} catch (ThreadInterruptedException) {
						// This is used as a way to wake up sleeping thread.
					}
				}
			});
		}

		#endregion
	}
}