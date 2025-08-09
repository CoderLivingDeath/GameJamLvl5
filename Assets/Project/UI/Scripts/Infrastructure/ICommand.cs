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

    public CommandBase(Action<object> execute, Func<object, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object param) => _canExecute.Invoke(param);

    public void Execute(object param) => _execute.Invoke(param);
}

public class LambdaCommand : CommandBase
{
    public LambdaCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
    {
    }
}