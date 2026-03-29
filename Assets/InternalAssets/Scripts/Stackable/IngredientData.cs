using UnityEngine;

/// <summary>
/// Stores the recipe ingredient type assigned to a stackable object.
/// </summary>
[RequireComponent(typeof(StackableObject))]
public class IngredientData : MonoBehaviour
{
    [SerializeField] private IngredientType _type = IngredientType.BottomBun;

    /// <summary>
    /// Gets the configured ingredient type.
    /// </summary>
    /// <value>
    /// One of the supported ingredient types used during recipe assembly.
    /// </value>
    public IngredientType Type => _type;
}

/// <summary>
/// Defines the supported ingredient types used by recipe assembly.
/// </summary>
public enum IngredientType
{
    /// <summary>
    /// The bottom bun that starts the burger stack.
    /// </summary>
    BottomBun,

    /// <summary>
    /// The meat patty placed between buns.
    /// </summary>
    Patty,

    /// <summary>
    /// A cheese layer added to the burger stack.
    /// </summary>
    Cheese,

    /// <summary>
    /// The top bun that finishes the burger stack.
    /// </summary>
    TopBun
}
