using UnityEngine;

[RequireComponent(typeof(HoldableObject))]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Marks a holdable object as stackable and exposes alignment points for burger assembly.
/// </summary>
public class StackableObject : MonoBehaviour
{
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _bottomPoint;

    private HoldableObject _holdableObject;
    private Rigidbody _rigidbody;

    /// <summary>
    /// Gets the point that becomes the next stack target after this object is attached.
    /// </summary>
    public Transform TopPoint => _topPoint;

    /// <summary>
    /// Gets the point used to align this object onto an existing stack point.
    /// </summary>
    public Transform BottomPoint => _bottomPoint;

    /// <summary>
    /// Caches required component references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        _holdableObject = GetComponent<HoldableObject>();
        _rigidbody = GetComponent<Rigidbody>();

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

        ok &= Guard.Expect(_holdableObject != null, "HoldableObject component is missing.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_topPoint != null, "Top point is not assigned.", this);
        ok &= Guard.Expect(_bottomPoint != null, "Bottom point is not assigned.", this);

        return ok;
    }

    /// <summary>
    /// Snaps this object onto a stack hierarchy and removes it from the holdable runtime flow.
    /// </summary>
    /// <param name="parent">Parent transform that owns the stack.</param>
    /// <param name="stackPoint">World-space alignment target for the bottom point.</param>
    public void AttachTo(Transform parent, Transform stackPoint)
    {
        _holdableObject.EndHold();
        transform.SetParent(parent);
        transform.rotation = stackPoint.rotation;
        transform.position += stackPoint.position - _bottomPoint.position;
        _rigidbody.isKinematic = true;
        _holdableObject.enabled = false;
    }

    /// <summary>
    /// Registers this object in the provided burger data container.
    /// </summary>
    /// <param name="burgerData">Burger data that should contain this object.</param>
    public void AddTo(BurgerData burgerData)
    {
        burgerData.Add(this);
    }
}
