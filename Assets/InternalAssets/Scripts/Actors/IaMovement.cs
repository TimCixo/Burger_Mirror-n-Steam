using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class IaMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _moveAction;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _collisionSkin = 0.02f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_moveAction != null, "Move action reference is not assigned.", this);
        ok &= Guard.Expect(_moveAction.action != null, "Move action is not properly set up.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);

        return ok;
    }

    private void OnEnable()
    {
        _moveAction?.action?.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.action?.Disable();
        _moveInput = Vector2.zero;
    }

    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
        Vector3 delta;
        float distance;
        float safeDistance;

        direction = transform.TransformDirection(direction);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0f)
        {
            direction.Normalize();
        }

        delta = direction * _moveSpeed * Time.fixedDeltaTime;
        distance = delta.magnitude;

        if (distance > 0f && _rigidbody.SweepTest(direction, out RaycastHit hit, distance + _collisionSkin, QueryTriggerInteraction.Ignore))
        {
            safeDistance = Mathf.Max(0f, hit.distance - _collisionSkin);
            delta = direction * safeDistance;
        }

        _rigidbody.MovePosition(_rigidbody.position + delta);
    }
}
