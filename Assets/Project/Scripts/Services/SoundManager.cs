using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SoundManager
{
    public AudioSource MusicSource;

    [Inject]
    private void Construct()
    {
        
    }
}
