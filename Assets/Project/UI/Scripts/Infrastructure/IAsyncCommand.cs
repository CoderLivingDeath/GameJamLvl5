using System;
using Cysharp.Threading.Tasks;

public interface IAsyncCommand
{
    UniTask ExecuteAsync(object param);
    bool CanExecute(object param);
}

public abstract class AsyncCommandBase : IAsyncCommand
{
    private protected Func<object, UniTask> _executeAsync;
    private protected Func<object, bool> _canExecute;

    protected AsyncCommandBase(Func<object, UniTask> executeAsync, Func<object, bool> canExecute = null)
    {
        _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        _canExecute = canExecute;
    }

    public bool CanExecute(object param) => _canExecute?.Invoke(param) ?? true;

    public UniTask ExecuteAsync(object param) => _executeAsync.Invoke(param);
}

public class LambdaAsyncCommand : AsyncCommandBase
{
    public LambdaAsyncCommand(Func<object, UniTask> executeAsync, Func<object, bool> canExecute = null)
        : base(executeAsync, canExecute)
    {
    }
}