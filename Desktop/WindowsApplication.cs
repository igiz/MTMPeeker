using System;
using System.Threading;
using System.Windows.Threading;
using Desktop.Commands;
using Desktop.WPF;
using Desktop.WPF.ViewModels;
using TFSPeeker;
using TFSPeeker.Interrogation;

namespace Desktop
{
	public class WindowsApplication : IApplication
	{
		private ApplicationContext context;

		private IDisposable subscription;

		private DataSet currentDataSet;

		private MainViewModel viewModel;

		private Thread uiThread;

		#region Constructor

		public WindowsApplication(ApplicationContext context)
		{
			this.currentDataSet = new DataSet();
			this.context = context;
		}

		#endregion

		#region Public Methods

		public void Run(IInterrogator<DataSet> interrogator)
		{
			subscription = interrogator.DataProvider.Subscribe(this);

			Action<object> changeContext = (args) => {
				ApplicationContext newContext = new ApplicationContext(context)
				{
					PlanId = viewModel.PlanId,
					SuiteId = viewModel.SuiteId,
					Keyword = viewModel.Keyword
				};
				this.context = newContext;
				interrogator.SwitchContext(newContext);
				ShowSelectedView();
			};

			RelayCommand<object> changeContextCommand = new RelayCommand<object>(changeContext, args => true);
			RelayCommand<object> changeViewCommand = new RelayCommand<object>(args => ShowSelectedView(), args => true);
			RelayCommand<object> closeCommand = new RelayCommand<object>(args => Closing(), args => true);
				
			uiThread = new Thread(() => {
				viewModel = new MainViewModel(context, changeContextCommand, changeContextCommand, changeContextCommand, changeViewCommand, closeCommand);
				MainForm mainForm = new MainForm { DataContext = viewModel };
				mainForm.Show();
				Dispatcher.Run();
			}) {IsBackground = true};

			
			uiThread.SetApartmentState(ApartmentState.STA);
			uiThread.Start();
			interrogator.Start();
			//Do not return until the app finishes executing.
			uiThread.Join();
		}

		public void OnNext(DataSet value)
		{
			currentDataSet = value;
			ShowSelectedView();
		}

		public void OnError(Exception error)
		{
			throw error;
		}

		public void OnCompleted()
		{
			Dispose();
		}

		public void Dispose()
		{
			subscription.Dispose();
			subscription = null;
			uiThread = null;
			viewModel = null;
		}

		#endregion

		#region Private Methods

		private void Closing()
		{
			uiThread.Abort();
		}

		private void ShowSelectedView()
		{
			viewModel.SetViewResult(ViewResultBuilder.BuildResult(currentDataSet, viewModel.CurrentViewType, context.Keyword));
		}

		#endregion
	}
}