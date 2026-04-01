using UnityEngine;

[RequireComponent(typeof(HoldableObject))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HoldableStateController))]
/// <summary>
/// Marks a holdable object as stackable and exposes alignment points for recipe assembly.
/// </summary>
public class StackableObject : MonoBehaviour
{
    [Header("Alignment")]
    [Tooltip("Point exposed above this object and used as the next stack target.")]
    [SerializeField] private Transform _topPoint;
    [Tooltip("Point used to align this object onto the current stack point.")]
    [SerializeField] private Transform _bottomPoint;

    private HoldableObject _holdableObject;
    private HoldableStateController _stateController;

    /// <summary>
    /// Gets the point that becomes the next stack target after this object is attached.
    /// </summary>
    public Transform TopPoint => _topPoint;

    /// <summary>
    /// Gets the point used to align this object onto an existing stack point.
    /// </summary>
    public Transform BottomPoint => _bottomPoint;

    /// <summary>
    /// Gets whether the object may currently attach to a stack.
    /// </summary>
    public bool CanAttachToStack => _stateController.CanAttachToStack();

    /// <summary>
    /// Caches required component references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        _holdableObject = GetComponent<HoldableObject>();
        _stateController = GetComponent<HoldableStateController>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates the serialized alignment points and required runtime dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the component is configured correctly.</returns>
    public bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_holdableObject != null, "HoldableObject component is missing.", gameObject);
        ok &= Guard.Expect(_stateController != null, "HoldableStateController component is missing.", gameObject);
        ok &= Guard.Expect(_topPoint != null, "Top point is not assigned.", gameObject);
        ok &= Guard.Expect(_bottomPoint != null, "Bottom point is not assigned.", gameObject);

        return ok;
    }

    /// <summary>
    /// Snaps this object onto a stack hierarchy and removes it from the holdable runtime flow.
    /// </summary>
    /// <param name="parent">Parent transform that owns the stack.</param>
    /// <param name="stackPoint">World-space alignment target for the bottom point.</param>
    public void AttachTo(Transform parent, Transform stackPoint)
    {
        if (!CanAttachToStack) return;

        _holdableObject.EndHold();
        transform.SetParent(parent);
        transform.rotation = stackPoint.rotation;
        transform.position += stackPoint.position - _bottomPoint.position;
        _stateController.ApplyState(HoldableState.Stacked);
    }

    /// <summary>
    /// Removes this object from its current stack hierarchy and restores the common holdable state.
    /// </summary>
    public void Detach()
    {
        transform.SetParent(null);
        _stateController.ApplyState(HoldableState.Common);
    }

    /// <summary>
    /// Registers this object in the provided recipe data container.
    /// </summary>
    /// <param name="recipeData">Recipe data that should contain this object.</param>
    public void AddTo(RecipeData recipeData)
    {
        recipeData.Add(this);
    }
}
