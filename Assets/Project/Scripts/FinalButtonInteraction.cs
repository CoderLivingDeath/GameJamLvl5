using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FinalButtonInteraction : MonoBehaviour
{
    [Inject] private ProgressionController progressionController;
    public string FinalKey;
    public void Handle()
    {
        progressionController.HandleFinal(FinalKey).Forget();
    }
}
