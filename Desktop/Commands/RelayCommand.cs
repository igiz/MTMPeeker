using System;
using System.Windows.Input;

namespace Desktop.Commands
{
	/// <summary>Class for a command that can only be executed when the predicate condition is met.</summary>
	public class RelayCommand<T> : ICommand
	{
		#region Public Events

		/// <summary>The event handler for CanExecuteChanged event.</summary>
		public event EventHandler CanExecuteChanged;

		#endregion

		#region Non-Public Fields

		/// <summary>The command to execute.</summary>
		private readonly Action<T> command;

		/// <summary>The predicate which checks whether the command can be executed.</summary>
		private readonly Predicate<object> canExecute;

		#endregion

		#region Public Constructors

		/// <summary>The main constructor.</summary>
		/// <param name="command">The action to execute.</param>
		/// <param name="canExecute">The predicate to check whether this command can be executed.</param>
		public RelayCommand(Action<T> command, Predicate<object> canExecute)
		{
			this.command = command;
			this.canExecute = canExecute;
			CommandManager.RequerySuggested += OnCanExecuteChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>Checks whether this command can be executed.</summary>
		/// <param name="parameter">The object.</param>
		/// <returns>A flag indicating whether the command can be executed.</returns>
		public bool CanExecute(object parameter)
		{
			bool result = canExecute == null || canExecute(parameter);
			return result;
		}

		/// <summary>Executes the command.</summary>
		/// <param name="parameter">The object.</param>
		public void Execute(object parameter)
		{
			command((T)parameter);
		}

		#endregion

		#region Non-Public Methods

		/// <summary>The event which handler which raises the CanExecuteChange event handler.</summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnCanExecuteChanged(object sender, EventArgs eventArgs)
		{
			EventHandler handler = CanExecuteChanged;
			handler?.Invoke(this, eventArgs);
		}

		#endregion
	}
}