using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Newtonsoft.Json;
using TFSPeekerDesktop.Views;

namespace TFSPeekerDesktop
{
	class Program
	{
		private static string configFile = "config.json";

		public static Dictionary<string, string> ExtractArguments(string[] args)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (string argument in args) {
				string[] kvp = argument.Split('=');
				if (kvp.Length == 2) {
					result[kvp[0]] = kvp[1];
				} else {
					throw new InvalidOperationException("Invalid argument specification");
				}
			}
			return result;
		}

		public static Dictionary<string, string> ExtractConfigurationArguments(string configFilePath)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			if (File.Exists(configFilePath)) {
				string jsonContent;
				var fileStream = new FileStream(configFile, FileMode.Open, FileAccess.Read);
				using (var streamReader = new StreamReader(fileStream, Encoding.UTF8)) {
					jsonContent = streamReader.ReadToEnd();
				}

				result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
			}

			return result;
		}

		public static void Main(string[] args)
		{
			string configurationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

			IDictionary<string, string> consoleArguments = ExtractArguments(args);
			IDictionary<string, string> storedConfigurationArguments = ExtractConfigurationArguments(configurationFilePath);

			IDictionary<string, string> arguments = consoleArguments
				.Concat(storedConfigurationArguments
					.Where(kvp => !consoleArguments
						.ContainsKey(kvp.Key))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
				.ToDictionary(kvp => kvp.Key , kvp => kvp.Value);

			string tfsUrl = arguments[ConfigurationConstants.TfsUrl];
			string project = arguments[ConfigurationConstants.Project];
			int testSuiteId = Int32.Parse(arguments[ConfigurationConstants.TestSuiteId]);
			int testPlanId = Int32.Parse(arguments[ConfigurationConstants.TestPlanId]);
			string[] views = arguments[ConfigurationConstants.Views].Split(',');
			int pollTimeInMinutes = Int32.Parse(arguments[ConfigurationConstants.PollTimeInMinutes]);

			TimeSpan pollTime = TimeSpan.FromMinutes(pollTimeInMinutes);

			Thread taskThread = new Thread(() => {
				while (true) {
					try {
						using (new ConsoleFormatter(ConsoleColor.DarkGreen, ConsoleColor.White)){
							Console.WriteLine($"Refreshing views at: {DateTime.Now.ToShortTimeString()}");
						}

						using (TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(tfsUrl))) {

							ITestManagementService service =
								tfs.GetService(typeof(ITestManagementService)) as ITestManagementService;
							ITestManagementTeamProject testProject = service.GetTeamProject(project);

							//Can extract information about the test suite from here
							ITestSuiteBase testSuite = testProject.TestSuites.Find(testSuiteId);

							//This is a given instance of the test suite , I.E the test plan (suites can be re-used)
							ITestPlan testPlan = testProject.TestPlans.Find(testPlanId);

							Console.WriteLine(
								$"Test Suite: {testSuite.Title} \nDescription: {testPlan.Description} \nLast Updated: {testPlan.LastUpdated}"
							);

							Console.WriteLine();

							TestCaseViewFactory viewFactory = new TestCaseViewFactory();
							viewFactory.Init(testPlan, testProject, tfsUrl, project);
							IEnumerable<IView> viewsToDisplay = viewFactory.GetViews(views);

							foreach (var viewToDisplay in viewsToDisplay) {
								viewToDisplay.ConsoleOut();
								Console.WriteLine();
							}
						}

						Thread.Sleep(pollTime);
						Console.Clear();
					} catch (ThreadInterruptedException) {
						// This is used as a way to wake up sleeping thread.
					}
				}
			});

			taskThread.Start();
			string command;
			do {
				using (new ConsoleFormatter(ConsoleColor.DarkRed, ConsoleColor.White)) {
					Console.WriteLine("Type 'quit' to exit the application");
				}
				using (new ConsoleFormatter(ConsoleColor.Gray, ConsoleColor.White)){
					Console.WriteLine("Type 'refresh' to refresh views");
				}
				command = Console.ReadLine();
				if (command == "refresh") {
					taskThread.Interrupt();
				}
			} while (command!="quit");

			taskThread.Abort();
		}
	}
}
