using Microsoft.TeamFoundation.TestManagement.Client;

namespace TFSPeekerDesktop
{
	public struct TestCaseDescription
	{
		public ITestPoint Info { get; }

		public string Url { get; }

		public TestCaseDescription(ITestPoint info, string url)
		{
			Info = info;
			Url = url;
		}
	}
}