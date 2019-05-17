using System.Collections.Generic;
using TFSPeeker.Interrogation;

namespace TFSPeeker
{
	public class AppConfig
	{
		private static string configFile = "config.json";

		public static ApplicationContext CreateContext(string[] args)
		{
			IDictionary<string, string> arguments = ArgumentParser.Parse(args, configFile);

			ApplicationContext context = new ApplicationContext {
				Project = arguments[ConfigurationConstants.Project],
				ApiUrl = arguments[ConfigurationConstants.TfsUrl],
				PlanId = int.Parse(arguments[ConfigurationConstants.TestPlanId]),
				SuiteId = int.Parse(arguments[ConfigurationConstants.TestSuiteId]),
				RefreshTime = int.Parse(arguments[ConfigurationConstants.PollTimeInMinutes]),
				SelectedViews = arguments[ConfigurationConstants.Views].Split(',')
			};

			return context;
		}
	}
}
