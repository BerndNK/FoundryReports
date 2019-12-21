using System;
using System.Windows.Input;

namespace FoundryReports.Utils
{
    internal class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action();
        }

#pragma warning disable 67 // never used warning. Event is necessary for the interface which is why it cannot be removed
        public event EventHandler? CanExecuteChanged;
#pragma warning restore 67

        public DelegateCommand(Action action)
        {
            _action = action;
        }
    }

    internal class DelegateCommand<T> : ICommand where T : class
    {
        private readonly Action<T?> _action;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action.Invoke(parameter as T);
        }

#pragma warning disable 67 // never used warning. Event is necessary for the interface which is why it cannot be removed
        public event EventHandler? CanExecuteChanged;
#pragma warning restore 67

        public DelegateCommand(Action<T?> action)
        {
            _action = action;
        }
    }
}