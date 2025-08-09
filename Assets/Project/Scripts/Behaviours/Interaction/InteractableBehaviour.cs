using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class InteractableBehaviour : InteractableBehaviourBase
{
    // UnityEvent invoked when interaction occurs
    public UnityEvent OnInteractEvent;

    [SerializeField]
    private Func<bool> _canInteract; // Function to determine if interaction is allowed (not currently used)

    [SerializeField]
    [SelectionPopup(nameof(strs), callbackName: nameof(OnItemSelected), placeholder: "{select}")]
    private string Interaction; // Selected interaction type from a dropdown UI

    // Property returning selectable items to show in dropdown for interaction selection
    public SelectItem[] strs => GetSelectedItems().ToArray();

    // Returns the assembly in which this class type is defined
    public Assembly GetAssembly()
    {
        return this.GetType().Assembly;
    }

    /// <summary>
    /// Finds all MonoBehaviour-derived types marked with a specific attribute within a given assembly.
    /// </summary>
    /// <param name="attributeType">Type of attribute to look for (e.g. typeof(MyAttribute))</param>
    /// <param name="assembly">Assembly to search in</param>
    private IEnumerable<Type> GetBehavioursByAttribute(Type attributeType, Assembly assembly)
    {
        if (assembly == null)
            yield break;

        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // In case some types failed to load, ignore nulls and proceed with loaded types
            types = ex.Types.Where(t => t != null).ToArray();
        }

        foreach (Type type in types)
        {
            if (type == null)
                continue;

            // Return types that are non-abstract subclasses of MonoBehaviour and have the specified attribute
            if (type.IsSubclassOf(typeof(MonoBehaviour)) &&
                !type.IsAbstract &&
                Attribute.IsDefined(type, attributeType, false))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Finds a MonoBehaviour type by name and attribute in a given assembly.
    /// </summary>
    /// <param name="name">Type name without namespace</param>
    /// <param name="attributeType">Attribute type to check for</param>
    /// <param name="assembly">Assembly to search</param>
    /// <returns>Found Type or null if not found</returns>
    private Type GetBehaviourByNameAndAttribute(string name, Type attributeType, Assembly assembly)
    {
        if (assembly == null || string.IsNullOrEmpty(name) || attributeType == null)
            return null;

        try
        {
            return assembly.GetTypes()
                .Where(t => t != null
                            && t.IsClass
                            && !t.IsAbstract
                            && t.IsSubclassOf(typeof(MonoBehaviour))
                            && t.Name == name
                            && Attribute.IsDefined(t, attributeType, false))
                .FirstOrDefault();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Handle types loaded partially
            return ex.Types
                .Where(t => t != null
                            && t.IsClass
                            && !t.IsAbstract
                            && t.IsSubclassOf(typeof(MonoBehaviour))
                            && t.Name == name
                            && Attribute.IsDefined(t, attributeType, false))
                .FirstOrDefault();
        }
    }

    // Get all interactable types marked with the InteractableComponentAttribute
    private IEnumerable<Type> GetInteractableTypes()
    {
        foreach (var item in GetBehavioursByAttribute(typeof(InteractableComponentAttribute), GetAssembly()))
        {
            yield return item;
        }
    }

    // Build the list of selectable items for UI based on interactable types present on this GameObject
    private IEnumerable<SelectItem> GetSelectedItems()
    {
        foreach (var type in GetInteractableTypes())
        {
            // Check if the GameObject already has this component
            var exists = gameObject.GetComponent(type) != null;
            var name = type.Name;
            var displayName = type.Name;

            bool isSelected = false;  // Currently unused, can be used to mark active selection
            bool isActive = !exists;  // Active if component is not on GameObject

            yield return new SelectItem(name, displayName, isSelected, isActive);
        }
    }

    // Helper to get interactable behaviour Type by its name
    private Type GetInteractableBehaviourByName(string value)
    {
        return GetBehaviourByNameAndAttribute(value, typeof(InteractableComponentAttribute), GetAssembly());
    }

    // Called when a new interaction type is selected from the UI popup
    private void OnItemSelected(string value)
    {
        Type behaviour = GetInteractableBehaviourByName(value);

        if (behaviour == null)
        {
            Debug.LogWarning($"Component with name {value} was not found.");
            return;
        }

        // Add the component of the selected behaviour type if not already present
        if (gameObject.GetComponent(behaviour) == null)
        {
            gameObject.AddComponent(behaviour);
        }
    }

    // Check if the interaction can be performed (stub, returns true always)
    public bool CanIneract()
    {
        return true;
    }

    // Override of Interact method from base class, invoked by interactor
    public override void Interact(InteractionBehaviour sender)
    {
        if (!CanIneract())
            return;

        // Get all InteractableHandlerBehaviourBase components attached, representing handlers for interaction
        var interactables = GetComponents<InteractableHandlerBehaviourBase>();

        var context = new InteractionContext(this, sender);

        // Pass the interaction context to each handler
        foreach (var interactable in interactables)
        {
            interactable.HandleInteract(context);
        }

        // Invoke the Unity event for external subscribers
        OnInteractEvent?.Invoke();
    }

    // Clean up the event listeners when this GameObject is destroyed
    private void OnDestroy()
    {
        OnInteractEvent.RemoveAllListeners();
    }
}

// Context containing references to the interactable object and the interactor
public readonly struct InteractionContext
{
    public readonly InteractableBehaviour Interactable;
    public readonly InteractionBehaviour Interactor;

    public InteractionContext(InteractableBehaviour interactable, InteractionBehaviour interactor)
    {
        Interactable = interactable;
        Interactor = interactor;
    }
}

// Base class for interactable behaviours that can be extended
public abstract class InteractableBehaviourBase : MonoBehaviour
{
    // Virtual method representing an interaction event, to be overridden by subclasses
    public virtual void Interact(InteractionBehaviour sender)
    {
    }
}
