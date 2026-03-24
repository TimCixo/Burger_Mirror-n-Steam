using UnityEngine;

[RequireComponent(typeof(HoldableObject))]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Applies shared runtime states for holdable and stackable objects.
/// </summary>
public class HoldableStateController : MonoBehaviour
{
    [Header("States")]
    [Tooltip("Base physics state used when the object is free in the world.")]
    [SerializeField] private RigidbodyState _commonState = new(false, true, RigidbodyInterpolation.None);
    [Tooltip("Physics state used while the object is actively held.")]
    [SerializeField] private RigidbodyState _heldState = new(false, false, RigidbodyInterpolation.Interpolate);
    [Tooltip("Physics state used while the object is attached to a stack.")]
    [SerializeField] private RigidbodyState _stackedState = new(true, false, RigidbodyInterpolation.None);

    private HoldableObject _holdableObject;
    private Rigidbody _rigidbody;

    /// <summary>
    /// Gets the object's current runtime state.
    /// </summary>
    public HoldableState CurrentState { get; private set; } = HoldableState.Common;

    /// <summary>
    /// Caches required component references and applies the default common state.
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

        ApplyState(HoldableState.Common);
    }

    /// <summary>
    /// Validates required runtime dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the state controller can operate.</returns>
    public bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_holdableObject != null, "HoldableObject component is missing.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);

        return ok;
    }

    /// <summary>
    /// Applies the provided runtime state to the rigidbody and holdable behaviour.
    /// </summary>
    /// <param name="state">State to apply.</param>
    public void ApplyState(HoldableState state)
    {
        CurrentState = state;

        RigidbodyState rigidbodyState = GetState(state);
        _rigidbody.isKinematic = rigidbodyState.IsKinematic;
        _rigidbody.useGravity = rigidbodyState.UseGravity;
        _rigidbody.interpolation = rigidbodyState.Interpolation;
        _rigidbody.collisionDetectionMode = rigidbodyState.CollisionDetectionMode;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        bool enableHoldable = state != HoldableState.Stacked;
        if (_holdableObject.enabled != enableHoldable)
        {
            _holdableObject.enabled = enableHoldable;
        }
    }

    /// <summary>
    /// Returns whether the object may currently attach to a stack.
    /// </summary>
    public bool CanAttachToStack()
    {
        return CurrentState == HoldableState.Common;
    }

    /// <summary>
    /// Resolves the rigidbody settings for the requested runtime state.
    /// </summary>
    /// <param name="state">State to resolve.</param>
    /// <returns>Configured rigidbody state for the given runtime mode.</returns>
    private RigidbodyState GetState(HoldableState state)
    {
        switch (state)
        {
            case HoldableState.Held:
                return _heldState;

            case HoldableState.Stacked:
                return _stackedState;

            default:
                return _commonState;
        }
    }
}

/// <summary>
/// Defines the shared runtime modes used by holdable and stackable objects.
/// </summary>
public enum HoldableState
{
    Common,
    Held,
    Stacked
}

/// <summary>
/// Serializable rigidbody settings used by runtime state transitions.
/// </summary>
[System.Serializable]
public struct RigidbodyState
{
    [SerializeField] private bool _isKinematic;
    [SerializeField] private bool _useGravity;
    [SerializeField] private RigidbodyInterpolation _interpolation;
    [SerializeField] private CollisionDetectionMode _collisionDetectionMode;

    /// <summary>
    /// Gets whether the rigidbody should be kinematic in this state.
    /// </summary>
    public bool IsKinematic => _isKinematic;

    /// <summary>
    /// Gets whether gravity should affect the rigidbody in this state.
    /// </summary>
    public bool UseGravity => _useGravity;

    /// <summary>
    /// Gets the rigidbody interpolation mode applied in this state.
    /// </summary>
    public RigidbodyInterpolation Interpolation => _interpolation;

    /// <summary>
    /// Gets the collision detection mode applied in this state.
    /// </summary>
    public CollisionDetectionMode CollisionDetectionMode => _collisionDetectionMode;

    /// <summary>
    /// Creates a serializable rigidbody state description.
    /// </summary>
    /// <param name="isKinematic">Whether the rigidbody should be kinematic.</param>
    /// <param name="useGravity">Whether the rigidbody should use gravity.</param>
    /// <param name="interpolation">Interpolation mode applied to the rigidbody.</param>
    /// <param name="collisionDetectionMode">Collision detection mode applied to the rigidbody.</param>
    public RigidbodyState(bool isKinematic, bool useGravity, RigidbodyInterpolation interpolation, CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Discrete)
    {
        _isKinematic = isKinematic;
        _useGravity = useGravity;
        _interpolation = interpolation;
        _collisionDetectionMode = collisionDetectionMode;
    }
}
