using System.Windows.Input;

namespace funya1_wpf
{
    public class ActionCommand(Action<object?> action) : ICommand
    {
#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => action(parameter);
    }
}
