using Microsoft.TeamFoundation.TestManagement.Client;

namespace TFSPeeker.Interrogation
{
	public struct TestCaseDescription
	{
		public int Id { get; }

		public string Title { get; }

		public string State { get; }

		public string Url { get; }

		public TestCaseDescription(ITestPoint info, string url)
		{
			Id = info.TestCaseId;
			Title = info.TestCaseWorkItem.Title;
			State = info.State.ToString();
			Url = url;
		}
	}
}