using System.Windows.Input;

namespace funya1_wpf
{
    public class ActionCommand(Action<object?> action) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => action(parameter);
    }
}
