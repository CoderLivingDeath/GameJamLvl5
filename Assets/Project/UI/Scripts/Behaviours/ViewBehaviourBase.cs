using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Obsolete]
public abstract class ViewBehaviourBase : MonoBehaviour
{

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual async UniTask ShowAsync()
    {
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual async UniTask CloseAsync()
    {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }
}