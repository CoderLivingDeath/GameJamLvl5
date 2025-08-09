using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class InteractionBehaviour : MonoBehaviour
{
    // Radius to check for interactable objects
    public float Radius;
    
    // Maximum distance for interaction (used in CircleCast)
    public float Distance;

    // The currently selected interactable (the closest one)
    public InteractableBehaviour SelectedInteractable => Interactables.First();

    // Array of interactable objects currently detected
    public InteractableBehaviour[] Interactables;

    // LayerMask to filter which objects to detect
    public LayerMask Mask;

    // PlayerLoopTiming used for scheduling UniTask.Yield
    public PlayerLoopTiming Timing => _timing;

    // Coroutine wrapper for async monitoring task
    private UniTaskCoroutine _interactionMonitoringUniTaskCorutine;

    [SerializeField]
    private PlayerLoopTiming _timing;

    // Async method that continuously updates the list of interactables while not cancelled
    private async UniTask InteractableMonitoring(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // Update the array of interactables, ordered by distance
            Interactables = GetInteractables().ToArray();

            // Yield execution until the next frame or timing specified
            await UniTask.Yield(_timing, cancellationToken);
        }
    }

    // Finds interactables around the origin within given radius and distance on specified layer mask
    public IEnumerable<InteractableBehaviour> FindeInteractables(Vector2 origin, float radius, float distance, LayerMask mask)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,
            radius,
            Vector2.zero, // zero direction means a circle check around origin
            distance,
            mask
        );

        HashSet<InteractableBehaviour> result = new HashSet<InteractableBehaviour>();
        foreach (var hit in hits)
        {
            var interactable = hit.collider.GetComponent<InteractableBehaviour>();
            if (interactable != null)
            {
                result.Add(interactable);
            }
        }
        return result;
    }

    // Retrieves interactables near this object's position, sorted by distance
    public IEnumerable<InteractableBehaviour> GetInteractables()
    {
        return FindeInteractables(transform.position, Radius, Distance, Mask)
            .OrderBy(item => Vector2.Distance(transform.position, item.transform.position));
    }

    // Calls Interact method on the currently selected interactable object
    public void Interact()
    {
        SelectedInteractable.Interact(this);
    }

    #region Unity Methods

    // Initialize the UniTask coroutine with the monitoring method
    private void Awake()
    {
        _interactionMonitoringUniTaskCorutine = new(InteractableMonitoring);
    }

    // Start monitoring interactables when enabled
    private void OnEnable()
    {
        _interactionMonitoringUniTaskCorutine.Run();
    }

    // Stop monitoring interactables when disabled
    private void OnDisable()
    {
        _interactionMonitoringUniTaskCorutine.Stop();
    }

    // Dispose of the coroutine when this object is destroyed
    private void OnDestroy()
    {
        _interactionMonitoringUniTaskCorutine.Dispose();
    }

    // Draw a wire sphere in the editor representing the interaction radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    #endregion
}
