using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SoundManager
{
    private const string PARAM_MUSIC = "MusicVolume"; // Имя параметра, заданное при экспорте
    private const string PARAM_SOUND = "SoundVolume"; // Имя параметра, заданное при экспорте

    [Inject]
    private GameplaySceneAssets _assets;

    private AudioMixer mixer;

    [Inject]
    private void Construct()
    {
        mixer = _assets.AudioMixer;
        SoundVolume = 1f;
        Debug.Log($"cound volume set to: {SoundVolume}");
    }

    // Свойство для громкости музыки (0..1)
    public float MusicVolume
    {
        get
        {
            mixer.GetFloat(PARAM_MUSIC, out float value);
            return Mathf.InverseLerp(-80f, 20f, value);
        }
        set
        {
            float volume = Mathf.Lerp(-80f, 20f, Mathf.Clamp01(value));
            mixer.SetFloat(PARAM_MUSIC, volume);
        }
    }

    // Свойство для громкости звуков (0..1)
    public float SoundVolume
    {
        get
        {
            mixer.GetFloat(PARAM_SOUND, out float value);
            return Mathf.InverseLerp(-80f, 0f, value);
        }
        set
        {
            float volume = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(value));
            mixer.SetFloat(PARAM_SOUND, volume);
        }
    }
}
