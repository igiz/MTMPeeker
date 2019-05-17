using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace Desktop.WPF.ViewModels
{
	/// <summary>Base class for all ViewModels.</summary>
	public class ViewModelBase : INotifyPropertyChanged
	{
		#region Public Events

		/// <summary>Event handler called when notyfing PropertyChanged event.</summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public Properties

		/// <summary>Gets or sets a dispatcher. Should be not null if a binding form is going to be used from other threads.</summary>
		public Dispatcher Dispatcher { get; set; }

		#endregion

		#region Public Constructors

		/// <summary>Initializes a new instance of the <see cref="ViewModelBase"/> class.</summary>
		public ViewModelBase()
		{
			Dispatcher = Dispatcher.CurrentDispatcher;
		}

		#endregion

		#region Non-Public Methods

		/// <summary> Called when some of the properties is changed. </summary>
		/// <param name="propertyName">The name of the property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				Invoke(() => handler(this, new PropertyChangedEventArgs(propertyName)));
			}
		}

		/// <summary>
		/// Called when some of the properties is changed.
		/// </summary>
		/// <typeparam name="TProperty">Property type</typeparam>
		/// <param name="projection">Lambda expression</param>
		protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			MemberExpression memberExpression = (MemberExpression)projection.Body;
			OnPropertyChanged(memberExpression.Member.Name);
		}

		/// <summary>Invokes the specified handler.</summary>
		/// <param name="handler">The handler.</param>
		protected void Invoke(Action handler)
		{
			if (Dispatcher != null && !Dispatcher.CheckAccess()) {
				Dispatcher.Invoke(handler, DispatcherPriority.Normal);
			} else {
				handler();
			}
		}

		#endregion
	}
}