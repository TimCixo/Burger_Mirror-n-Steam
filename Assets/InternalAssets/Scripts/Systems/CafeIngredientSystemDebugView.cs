using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores a serialized debug snapshot for the scene ingredient system.
/// </summary>
[Serializable]
public sealed class CafeIngredientSystemDebugView
{
    [Tooltip("Total count of ingredient objects tracked by the runtime ingredient system.")]
    [SerializeField] private int _ingredientCount;
    [Tooltip("Count of tracked ingredients that are currently active in the scene.")]
    [SerializeField] private int _activeIngredientCount;
    [Tooltip("Count of tracked ingredients currently parked in the inactive reuse pool.")]
    [SerializeField] private int _inactiveIngredientCount;
    [Tooltip("Debug snapshot of tracked ingredients with their type, active state and stack membership.")]
    [SerializeField] private List<string> _ingredients = new();

    [NonSerialized] private CafeIngredientSystem _system;

    /// <summary>
    /// Binds the debug view to the runtime ingredient system.
    /// </summary>
    /// <param name="system">Runtime ingredient system instance.</param>
    public void Bind(CafeIngredientSystem system)
    {
        _system = system;
        Refresh();
    }

    /// <summary>
    /// Clears the runtime ingredient system reference and resets the serialized snapshot.
    /// </summary>
    public void Unbind()
    {
        _system = null;
        _ingredientCount = 0;
        _activeIngredientCount = 0;
        _inactiveIngredientCount = 0;
        _ingredients.Clear();
    }

    /// <summary>
    /// Copies the current runtime ingredient data into serialized debug fields.
    /// </summary>
    public void Refresh()
    {
        if (_system == null) return;

        _ingredientCount = _system.IngredientCount;
        _activeIngredientCount = _system.ActiveIngredientCount;
        _inactiveIngredientCount = _system.InactiveIngredientCount;

        _ingredients.Clear();

        IReadOnlyList<IngredientData> ingredients = _system.Ingredients;
        for (int i = 0; i < ingredients.Count; i++)
        {
            IngredientData ingredient = ingredients[i];

            bool isInStack = ingredient.GetComponentInParent<RecipeData>() != null;
            string stackState = isInStack ? "stacked" : "free";
            string activeState = ingredient.gameObject.activeInHierarchy ? "active" : "inactive";
            _ingredients.Add($"{ingredient.name} [{ingredient.Type}] ({activeState}, {stackState})");
        }
    }
}
