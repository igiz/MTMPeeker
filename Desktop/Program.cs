using TFSPeeker;
using TFSPeeker.Interrogation;

namespace Desktop
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationContext context = AppConfig.CreateContext(args);

			//We want to show all the supported views for now.
			context.SelectedViews = ViewResultBuilder.AllSupportedViews;
			using (IApplication application = new WindowsApplication(context)) {
				application.Run(new TfsApiInterrogator(context));
			}
		}
	}
}
