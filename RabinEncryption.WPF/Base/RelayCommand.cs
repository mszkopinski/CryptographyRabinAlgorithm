using System;
using System.Windows.Input;

namespace RabinEncryption.WPF.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameters)
        {
            return canExecute?.Invoke(parameters) ?? true;
        }

        public void Execute(object parameters)
        {
            execute(parameters);
        }

        public event EventHandler CanExecuteChanged;
    }
}