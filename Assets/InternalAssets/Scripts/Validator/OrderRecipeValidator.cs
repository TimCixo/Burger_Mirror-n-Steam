using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Validates burger objects that enter the order-check trigger and logs their current composition.
/// </summary>
public class OrderRecipeValidator : MonoBehaviour
{
    private readonly Dictionary<int, int> _activeEntries = new();

    /// <summary>
    /// Starts burger validation when a tracked object enters the trigger for the first time.
    /// </summary>
    /// <param name="other">Collider that entered the trigger volume.</param>
    private void OnTriggerEnter(Collider other)
    {
        int trackedObjectId = GetTrackedObjectId(other);

        if (_activeEntries.TryGetValue(trackedObjectId, out int contacts))
        {
            _activeEntries[trackedObjectId] = contacts + 1;
            return;
        }

        _activeEntries.Add(trackedObjectId, 1);
        ValidateBurger(other);
    }

    /// <summary>
    /// Clears tracked burger entries when the last collider of a tracked object leaves the trigger.
    /// </summary>
    /// <param name="other">Collider that exited the trigger volume.</param>
    private void OnTriggerExit(Collider other)
    {
        int trackedObjectId = GetTrackedObjectId(other);

        if (!_activeEntries.TryGetValue(trackedObjectId, out int contacts)) return;
        if (contacts > 1)
        {
            _activeEntries[trackedObjectId] = contacts - 1;
            return;
        }

        _activeEntries.Remove(trackedObjectId);
    }

    /// <summary>
    /// Returns the identifier used to treat multiple colliders from the same burger hierarchy as one entry.
    /// </summary>
    /// <param name="other">Collider that belongs to the tracked object.</param>
    /// <returns>Instance identifier of the burger data or root object used for trigger deduplication.</returns>
    private int GetTrackedObjectId(Collider other)
    {
        BurgerData burgerData = other.GetComponentInParent<BurgerData>();
        return burgerData != null ? burgerData.GetInstanceID() : other.transform.root.GetInstanceID();
    }

    /// <summary>
    /// Validates the entered object as a burger and logs either its composition or an error.
    /// </summary>
    /// <param name="other">Collider that triggered validation.</param>
    private void ValidateBurger(Collider other)
    {
        BurgerData burgerData = other.GetComponentInParent<BurgerData>();

        if (burgerData == null)
        {
            Debug.LogError($"Object '{other.name}' entered '{name}' without BurgerData.", this);
            return;
        }

        Debug.Log($"Burger composition: {BuildBurgerComposition(burgerData)}", burgerData);
    }

    /// <summary>
    /// Builds a printable ingredient list for the provided burger.
    /// </summary>
    /// <param name="burgerData">Burger whose stack contents should be formatted.</param>
    /// <returns>Comma-separated stackable object names or <c>&lt;empty&gt;</c> when the burger has no contents.</returns>
    private string BuildBurgerComposition(BurgerData burgerData)
    {
        if (burgerData.StackableObjects.Count == 0) return "<empty>";

        StringBuilder builder = new();

        for (int i = 0; i < burgerData.StackableObjects.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            StackableObject stackableObject = burgerData.StackableObjects[i];
            builder.Append(stackableObject != null ? stackableObject.name : "null");
        }

        return builder.ToString();
    }
}
