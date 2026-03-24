using UnityEngine;

[RequireComponent(typeof(StackableObject))]
/// <summary>
/// Stores the ingredient type assigned to a stackable ingredient object.
/// </summary>
public class IngredientsData : MonoBehaviour
{
    [SerializeField] private IngredientType _type = IngredientType.BottomBun;

    /// <summary>
    /// Gets the configured ingredient type.
    /// </summary>
    public IngredientType Type => _type;
}

/// <summary>
/// Defines the supported ingredient types used by burger assembly.
/// </summary>
public enum IngredientType
{
    BottomBun,
    Patty,
    Cheese,
    TopBun
}
