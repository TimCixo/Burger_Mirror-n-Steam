using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks ingredient objects available in the cafe scene and manages their reuse lifecycle.
/// </summary>
public sealed class CafeIngredientSystem
{
    private readonly List<IngredientData> _ingredients = new();

    /// <summary>
    /// Gets the registered ingredient objects tracked by the runtime system.
    /// </summary>
    public IReadOnlyList<IngredientData> Ingredients => _ingredients;

    /// <summary>
    /// Gets the total number of registered ingredient objects.
    /// </summary>
    public int IngredientCount => _ingredients.Count;

    /// <summary>
    /// Gets the number of registered ingredients that are currently active in the scene.
    /// </summary>
    public int ActiveIngredientCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < _ingredients.Count; i++)
            {
                IngredientData ingredient = _ingredients[i];
                if (ingredient.gameObject.activeSelf)
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <summary>
    /// Gets the number of registered ingredients that are currently inactive and available for reuse.
    /// </summary>
    public int InactiveIngredientCount => IngredientCount - ActiveIngredientCount;

    /// <summary>
    /// Registers an ingredient object in the runtime system.
    /// </summary>
    /// <param name="ingredient">Ingredient object to track.</param>
    /// <returns><see langword="true"/> when the ingredient was added; otherwise <see langword="false"/>.</returns>
    public bool Register(IngredientData ingredient)
    {
        if (ingredient == null) return false;
        if (_ingredients.Contains(ingredient)) return false;

        _ingredients.Add(ingredient);
        return true;
    }

    /// <summary>
    /// Removes an ingredient from its current stack, refreshes stack state and disables the object for later reuse.
    /// </summary>
    /// <param name="ingredient">Registered ingredient that should be deactivated.</param>
    /// <returns><see langword="true"/> when the ingredient is registered and ends up inactive; otherwise <see langword="false"/>.</returns>
    public bool TryDeactivateIngredient(IngredientData ingredient)
    {
        if (!_ingredients.Contains(ingredient)) return false;
        if (!ingredient.gameObject.activeSelf) return true;

        RecipeData recipeData = ingredient.GetComponentInParent<RecipeData>();
        StackHolder stackHolder = ingredient.GetComponentInParent<StackHolder>();
        StackableObject stackableObject = ingredient.GetComponent<StackableObject>();

        recipeData?.Remove(stackableObject);
        stackableObject?.Detach();
        stackHolder?.RefreshStackPoint();
        ingredient.gameObject.SetActive(false);
        return true;
    }

    /// <summary>
    /// Reactivates a registered ingredient and places it at the provided transform state.
    /// </summary>
    /// <param name="ingredient">Registered ingredient that should be reactivated.</param>
    /// <param name="position">World position applied to the ingredient.</param>
    /// <param name="rotation">World rotation applied to the ingredient.</param>
    /// <param name="parent">Optional parent assigned before activation.</param>
    /// <returns><see langword="true"/> when the ingredient is registered; otherwise <see langword="false"/>.</returns>
    public bool TryActivateIngredient(IngredientData ingredient, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!_ingredients.Contains(ingredient)) return false;

        Transform ingredientTransform = ingredient.transform;
        ingredientTransform.SetParent(parent);
        ingredientTransform.SetPositionAndRotation(position, rotation);

        if (!ingredient.gameObject.activeSelf)
        {
            ingredient.gameObject.SetActive(true);
        }

        HoldableStateController stateController = ingredient.GetComponent<HoldableStateController>();
        stateController?.ApplyState(HoldableState.Common);
        return true;
    }

    /// <summary>
    /// Finds the first inactive registered ingredient with the requested type.
    /// </summary>
    /// <param name="type">Ingredient type to search for.</param>
    /// <param name="ingredient">Resolved inactive ingredient when one is found.</param>
    /// <returns><see langword="true"/> when a matching inactive ingredient exists; otherwise <see langword="false"/>.</returns>
    public bool TryGetInactiveIngredient(IngredientType type, out IngredientData ingredient)
    {
        for (int i = 0; i < _ingredients.Count; i++)
        {
            IngredientData registeredIngredient = _ingredients[i];

            if (registeredIngredient.Type != type) continue;
            if (registeredIngredient.gameObject.activeSelf) continue;

            ingredient = registeredIngredient;
            return true;
        }

        ingredient = null;
        return false;
    }
}
