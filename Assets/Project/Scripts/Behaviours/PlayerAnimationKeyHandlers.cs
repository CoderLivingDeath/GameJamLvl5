using UnityEngine;
using Zenject;

public class PlayerAnimationKeyHandlers : MonoBehaviour
{
    [Inject]
    private PlayerBehaviour playerBehaviour;

    public void PlayStepSound()
    {
        playerBehaviour.StepsAudio.Play();
    }

}
