using System;
using Cysharp.Threading.Tasks;

public abstract class UIAnimation<Context>
{
    public abstract UniTask RunAsync(Context context);
}
