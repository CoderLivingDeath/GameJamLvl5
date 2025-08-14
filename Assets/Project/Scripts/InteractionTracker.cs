using UnityEngine;

public class InteractionTracker : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<InteractableTracker>(out var component))
        {
            component.TrackEnter();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<InteractableTracker>(out var component))
        {
            component.TrackExit();
        }
    }
}
