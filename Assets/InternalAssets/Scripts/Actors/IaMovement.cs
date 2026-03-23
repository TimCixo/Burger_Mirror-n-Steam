using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class IaMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _moveAction;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;

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
        Vector3 velocity = _rigidbody.linearVelocity;

        direction = transform.TransformDirection(direction);
        direction.y = 0f;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        velocity.x = direction.x * _moveSpeed;
        velocity.z = direction.z * _moveSpeed;

        _rigidbody.linearVelocity = velocity;
    }
}
