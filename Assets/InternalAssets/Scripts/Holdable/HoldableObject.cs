using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : MonoBehaviour
{
    [Header("Hold Follow")]
    [SerializeField] private float _followStrength = 12f;
    [SerializeField] private AnimationCurve _followCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _rotationStrength = 12f;
    [SerializeField] private float _snapDistance = 4f;

    private Rigidbody _rigidbody;
    private Transform _holdTarget;
    private bool _isHeld;

    public bool IsHeld => _isHeld;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!_isHeld || _holdTarget == null) return;

        UpdateHoldPosition();
        UpdateHoldRotation();
    }

    private void OnDisable()
    {
        EndHold();
    }

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

    public void BeginHold(Transform target)
    {
        _holdTarget = target;
        _rigidbody.useGravity = false;

        _isHeld = true;
    }

    public void EndHold()
    {
        if (!_isHeld && _holdTarget == null) return;

        _isHeld = false;
        _holdTarget = null;
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void ClearAngularVelocity()
    {
        _rigidbody.angularVelocity = Vector3.zero;
    }

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

    private void UpdateHoldRotation()
    {
        Quaternion targetRotation = _holdTarget.rotation;
        float rotationBlend = 1f - Mathf.Exp(-_rotationStrength * Time.fixedDeltaTime);
        Quaternion nextRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, rotationBlend);

        _rigidbody.MoveRotation(nextRotation);
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
