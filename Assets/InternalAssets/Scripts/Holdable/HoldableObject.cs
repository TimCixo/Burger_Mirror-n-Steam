using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Controls the runtime behaviour of a holdable rigidbody while it is carried by an actor.
/// </summary>
public class HoldableObject : MonoBehaviour
{
    [Header("Hold Follow")]
    [Min(0.01f)]
    [Tooltip("Base movement speed used while the object follows the hold target.")]
    [SerializeField] private float _followStrength = 12f;
    [Tooltip("Curve that shapes follow speed based on current distance to the hold target.")]
    [SerializeField] private AnimationCurve _followCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Min(0.01f)]
    [Tooltip("Blend speed used to rotate the object toward the hold target rotation.")]
    [SerializeField] private float _rotationStrength = 12f;
    [Min(0.01f)]
    [Tooltip("Distance threshold after which the object snaps back to the hold target instead of smoothly following.")]
    [SerializeField] private float _snapDistance = 4f;

    private Rigidbody _rigidbody;
    private Transform _holdTarget;
    private bool _isHeld;

    /// <summary>
    /// Gets a value indicating whether the object is currently held.
    /// </summary>
    public bool IsHeld => _isHeld;

    /// <summary>
    /// Caches required references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Updates the held object's position and rotation in the physics loop.
    /// </summary>
    private void FixedUpdate()
    {
        if (!_isHeld || _holdTarget == null) return;

        UpdateHoldPosition();
        UpdateHoldRotation();
    }

    /// <summary>
    /// Ensures hold state is cleared when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        EndHold();
    }

    /// <summary>
    /// Validates required references and serialized hold settings.
    /// </summary>
    /// <returns><see langword="true"/> when the object can be used as a holdable item.</returns>
    public bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_followStrength > 0f, "Follow strength must be greater than 0.", this);
        ok &= Guard.Expect(_followCurve != null, "Follow curve is not assigned.", this);
        ok &= Guard.Expect(_rotationStrength > 0f, "Rotation strength must be greater than 0.", this);
        ok &= Guard.Expect(_snapDistance > 0f, "Snap distance must be greater than 0.", this);

        return ok;
    }

    /// <summary>
    /// Starts following the provided hold target and disables gravity while held.
    /// </summary>
    /// <param name="target">Transform that defines the desired hold position and rotation.</param>
    public void BeginHold(Transform target)
    {
        _holdTarget = target;
        _rigidbody.useGravity = false;

        _isHeld = true;
    }

    /// <summary>
    /// Stops the hold follow behaviour and restores the object to free physics motion.
    /// </summary>
    public void EndHold()
    {
        if (!_isHeld && _holdTarget == null) return;

        _isHeld = false;
        _holdTarget = null;
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Clears any residual angular velocity on the held rigidbody.
    /// </summary>
    public void ClearAngularVelocity()
    {
        _rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Moves the held rigidbody toward the target position using a curved follow response.
    /// </summary>
    private void UpdateHoldPosition()
    {
        Vector3 targetPosition = _holdTarget.position;
        Vector3 toTarget = targetPosition - _rigidbody.position;
        float distance = toTarget.magnitude;

        if (toTarget.sqrMagnitude > _snapDistance * _snapDistance)
        {
            _rigidbody.position = targetPosition;
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        if (distance <= Mathf.Epsilon)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        float normalizedDistance = Mathf.Clamp01(distance / _snapDistance);
        float curveFactor = _followCurve.Evaluate(normalizedDistance);
        float moveStep = _followStrength * curveFactor * Time.fixedDeltaTime;
        Vector3 nextPosition = Vector3.MoveTowards(_rigidbody.position, targetPosition, moveStep);

        _rigidbody.MovePosition(nextPosition);
        _rigidbody.linearVelocity = Vector3.zero;
    }

    /// <summary>
    /// Rotates the held rigidbody toward the target rotation.
    /// </summary>
    private void UpdateHoldRotation()
    {
        Quaternion targetRotation = _holdTarget.rotation;
        float rotationBlend = 1f - Mathf.Exp(-_rotationStrength * Time.fixedDeltaTime);
        Quaternion nextRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, rotationBlend);

        _rigidbody.MoveRotation(nextRotation);
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
