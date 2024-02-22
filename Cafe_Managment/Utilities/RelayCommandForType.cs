using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cafe_Managment.Utilities
{
    internal class RelayCommandForType : ICommand
    {
        private readonly Action<Type> _execute;
        private readonly Func<Type, bool> _canExecute;

        public RelayCommandForType(Action<Type> execute, Func<Type, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((Type)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((Type)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
