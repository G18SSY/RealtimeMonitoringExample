﻿using System;
using System.Windows.Input;

namespace RealtimeMonitoringExample.Wpf
{
    public class Command : ICommand
    {
        private readonly Action<object?> action;

        public Command(Action action) : this(_ => action()) { }

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