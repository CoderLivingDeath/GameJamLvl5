using System;

public interface ICommand
{
    void Execute(object param);
    bool CanExecute(object param);
}

public abstract class CommandBase : ICommand
{
    private protected Action<object> _execute;
    private protected Func<object, bool> _canExecute;

    public CommandBase(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object param) => _execute.Invoke(param);
}

public class LambdaCommand : CommandBase
{
    public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
    {
    }
}