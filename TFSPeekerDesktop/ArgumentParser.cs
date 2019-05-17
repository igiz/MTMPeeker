using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TFSPeeker
{
	public class ArgumentParser
	{
		#region Public Methods

		public static IDictionary<string, string> Parse(string[] args, string configFile)
		{

			IDictionary<string, string> consoleArguments = ExtractArguments(args);
			IDictionary<string, string> storedConfigurationArguments = ExtractConfigurationArguments(configFile);

			IDictionary<string, string> result = consoleArguments
				.Concat(storedConfigurationArguments
					.Where(kvp => !consoleArguments
						.ContainsKey(kvp.Key))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			return result;
		}

		#endregion

		#region Private Methods

		private static Dictionary<string, string> ExtractArguments(string[] args)
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

		private static Dictionary<string, string> ExtractConfigurationArguments(string configFile)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			string configurationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
			if (File.Exists(configurationFilePath)) {
				string jsonContent;
				var fileStream = new FileStream(configFile, FileMode.Open, FileAccess.Read);
				using (var streamReader = new StreamReader(fileStream, Encoding.UTF8)) {
					jsonContent = streamReader.ReadToEnd();
				}

				result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
			}

			return result;
		}

		#endregion
	}
}