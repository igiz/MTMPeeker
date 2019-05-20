using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TFSPeeker.Interrogation;

namespace Desktop.WPF.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		#region Private Fields

		private int suiteId;

		private int planId;

		private string currentViewType;

		private string keyword;

		private bool autoRefresh;

		private DateTime lastRefresh;

		private ObservableCollection<string> viewTypes;

		private ObservableCollection<TestCaseDescription> testCases;

		#endregion

		#region Public Properties

		public int SuiteId
		{
			get => suiteId;

			set
			{
				if (suiteId != value) {
					suiteId = value;
					OnPropertyChanged(() => SuiteId);
				}
			}
		}

		public int PlanId
		{
			get => planId;

			set
			{
				if (planId != value) {
					planId = value;
					OnPropertyChanged(() => PlanId);
				}
			}
		}

		public string Keyword
		{
			get => keyword;

			set
			{
				if (keyword != value) {
					keyword = value;
					OnPropertyChanged(() => Keyword);
				}
			}
		}

		public string CurrentViewType
		{
			get => currentViewType;

			set
			{
				if (currentViewType != value) {
					currentViewType = value;
					OnPropertyChanged(() => CurrentViewType);
					if (ChangeView.CanExecute(value)) {
						ChangeView.Execute(value);
					}
				}
			}
		}

		public bool AutoRefresh
		{
			get => autoRefresh;

			set
			{
				if (autoRefresh != value) {
					autoRefresh = value;
					OnPropertyChanged(() => AutoRefresh);
				}
			}
		}

		public ObservableCollection<string> ViewTypes
		{
			get => viewTypes;

			set
			{
				if (viewTypes != value) {
					viewTypes = value;
					OnPropertyChanged(() => ViewTypes);
				}
			}
		}

		public int Total
		{
			get => TestCases.Count;
		}

		public string LastRefresh
		{
			get => lastRefresh.ToShortTimeString();
		}

		public string CurrentUser
		{
			get => System.Security.Principal.WindowsIdentity.GetCurrent().Name;
		}

		public ObservableCollection<TestCaseDescription> TestCases
		{
			get => testCases;

			set {
				if (testCases != value) {
					testCases = value;
					OnPropertyChanged(() => TestCases);
					OnPropertyChanged(() => Total);
				}
			}
		}

		#endregion

		#region Public Commands

		public ICommand Filter { get; }

		public ICommand ChangeSuite { get; }

		public ICommand ChangePlan { get; }

		public ICommand ChangeView { get; }

		public ICommand Close { get; }

		#endregion

		#region Constructors

		public MainViewModel(ApplicationContext context, ICommand filter, ICommand changeSuite, ICommand changePlan , ICommand changeView, ICommand close)
		{
			this.testCases = new ObservableCollection<TestCaseDescription>();
			this.viewTypes = new ObservableCollection<string>(context.SelectedViews);
			this.planId = context.PlanId;
			this.suiteId = context.SuiteId;
			this.autoRefresh = context.RefreshTime > 0;
			this.currentViewType = context.SelectedViews.First();

			this.Filter = filter;
			this.ChangeSuite = changeSuite;
			this.ChangePlan = changePlan;
			this.ChangeView = changeView;
			this.Close = close;
		}

		#endregion

		#region Public Methods

		public void SetViewResult(ViewResult viewResult)
		{
			Dispatch(() => {
				currentViewType = viewResult.Name;
				testCases.Clear();		
				foreach (var testCaseDescription in viewResult.Result) {
					testCases.Add(testCaseDescription);
				}
				lastRefresh = DateTime.Now;
				OnPropertyChanged(() => Total);
				OnPropertyChanged(() => LastRefresh);
			});
		}

		#endregion

		#region Private Methods

		private void Dispatch(Action action)
		{
			Dispatcher.Invoke(action);
		}

		#endregion
	}
}