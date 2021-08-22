using System;
using System.Windows.Input;

namespace WpfApp1.Wpf
{
    public class Command : ICommand
    {
        private readonly Action<object?> action;

        public Command(Action<object?> action)
        {
            this.action = action;
        }

        public bool CanExecute(object? parameter)
            => true;

        public void Execute(object? parameter)
            => action(parameter);

        public event EventHandler? CanExecuteChanged;
    }
}