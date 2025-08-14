using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class ScrimmerSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform Point;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(prefab, Point.position, quaternion.identity);
        Destroy(gameObject);
    }
}
