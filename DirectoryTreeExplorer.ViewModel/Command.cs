using System;
using System.Windows.Input;

namespace DirectoryTreeExplorer.ViewModel
{
    /// <summary>
	/// Represents the implementation of ICommand interface.
	/// </summary>
	public sealed class Command : ICommand
    {
        public delegate void ActionDelegate();

        public ActionDelegate ExecuteDelegate { get; set; }
        public Predicate<object> CanExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteDelegate == null || this.CanExecuteDelegate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Command(ActionDelegate action)
        {
            this.ExecuteDelegate = action;
        }

        public void Execute(object parameter)
        {
            this.ExecuteDelegate?.Invoke();
        }
    }
}
