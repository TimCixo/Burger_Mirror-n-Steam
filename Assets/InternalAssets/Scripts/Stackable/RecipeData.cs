using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the ordered list of stackable objects that currently form a recipe stack.
/// </summary>
public class RecipeData : MonoBehaviour
{
    [Tooltip("Ordered list of stackable objects currently registered as part of the recipe.")]
    [SerializeField] private List<StackableObject> _stackableObjects = new();

    /// <summary>
    /// Gets the current stack contents in insertion order.
    /// </summary>
    public List<StackableObject> StackableObjects => _stackableObjects;

    /// <summary>
    /// Adds a stackable object to the recipe if it is not already present.
    /// </summary>
    /// <param name="stackableObject">Object to add.</param>
    public void Add(StackableObject stackableObject)
    {
        if (_stackableObjects.Contains(stackableObject)) return;

        _stackableObjects.Add(stackableObject);
    }

    /// <summary>
    /// Removes a stackable object from the recipe.
    /// </summary>
    /// <param name="stackableObject">Object to remove.</param>
    public void Remove(StackableObject stackableObject)
    {
        _stackableObjects.Remove(stackableObject);
    }
}
