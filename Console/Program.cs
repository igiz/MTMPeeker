using TFSPeeker;
using TFSPeeker.Interrogation;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationContext context = AppConfig.CreateContext(args);
			using (IApplication application = new ConsoleApplication(context)) {
				application.Run(new TfsApiInterrogator(context));
			}
		}
	}
}
