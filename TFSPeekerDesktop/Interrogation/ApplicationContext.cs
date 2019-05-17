using System.Collections.Generic;

namespace TFSPeeker.Interrogation
{
	public struct ApplicationContext
	{
		public IEnumerable<string> SelectedViews { get; set; }

		public string Project { get; set; }

		public string ApiUrl { get; set; }

		public int SuiteId { get; set; }

		public int PlanId { get; set; }

		public int RefreshTime { get; set; }

		public string Keyword { get; set; }

		public ApplicationContext(ApplicationContext another)
		{
			Project = another.Project;
			ApiUrl = another.ApiUrl;
			SuiteId = another.SuiteId;
			PlanId = another.PlanId;
			RefreshTime = another.RefreshTime;
			SelectedViews = another.SelectedViews;
			Keyword = another.Keyword;
		}
	}
}