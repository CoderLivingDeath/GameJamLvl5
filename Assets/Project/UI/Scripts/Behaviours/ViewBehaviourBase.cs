using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ViewBehaviourBase : MonoBehaviour
{
    public virtual async UniTask ShowAsync()
    {
        await UniTask.CompletedTask;
    }
    public virtual async UniTask CloseAsync()
    {
        await UniTask.CompletedTask;
    }
}
