using System.Collections.Generic;

namespace TFSPeeker.Interrogation
{
	public class DataSet
	{

		#region Public Properties

		public ISet<TestCaseDescription> Unassigned { get; set; }

		public ISet<TestCaseDescription> Assigned { get; set; }

		public ISet<TestCaseDescription> Automated { get; set; }

		public ISet<TestCaseDescription> Ready { get; set; }

		public ISet<TestCaseDescription> InProgress { get; set; }

		public ISet<TestCaseDescription> Complete { get; set; }

		public IDictionary<int, ISet<TestCaseDescription>> TestCasePriority { get; set; }

		#endregion

		public DataSet()
		{
			Unassigned = new HashSet<TestCaseDescription>();
			Assigned = new HashSet<TestCaseDescription>();
			Automated = new HashSet<TestCaseDescription>();
			Ready = new HashSet<TestCaseDescription>();
			InProgress = new HashSet<TestCaseDescription>();
			Complete = new HashSet<TestCaseDescription>();
			TestCasePriority = new Dictionary<int, ISet<TestCaseDescription>>();
		}

		public void AddCasePriority(int priority, TestCaseDescription testCase)
		{
			if (!TestCasePriority.ContainsKey(priority)){
				TestCasePriority[priority] = new HashSet<TestCaseDescription>();
			}

			TestCasePriority[priority].Add(testCase);
		}
	}
}