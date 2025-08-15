using UnityEngine;
using UnityEngine.Audio;

public class GameplaySceneAssets : MonoBehaviour
{
    public Transform L1SpawnPoint;
    public Transform L2SpawnPoint;
    public Transform[] L3SpawnPoints;

    public PolygonCollider2D L1CameraBounds;
    public PolygonCollider2D L2CameraBounds;
    public PolygonCollider2D[] L3CameraBounds;

    public AudioMixer AudioMixer;

    public AudioSource MusicSource;

    public AudioResource Ktulhu;
    public AudioResource doctor;
    public AudioResource island;
}
