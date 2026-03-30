using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Applies horizontal locomotion to the actor from the configured move input action.
/// </summary>
public class IaMovement : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input action that provides the 2D movement vector.")]
    [SerializeField] private InputActionReference _moveAction;

    [Header("Movement")]
    [Min(0.01f)]
    [Tooltip("Horizontal movement speed applied to the actor rigidbody.")]
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

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
    /// Validates the input action and rigidbody dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the movement component is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_moveAction != null, "Move action reference is not assigned.", gameObject);
        ok &= Guard.Expect(_moveAction.action != null, "Move action is not properly set up.", gameObject);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", gameObject);

        return ok;
    }

    /// <summary>
    /// Enables the move input action when the component becomes active.
    /// </summary>
    private void OnEnable()
    {
        _moveAction?.action?.Enable();
    }

    /// <summary>
    /// Disables the move input action and clears the cached input state.
    /// </summary>
    private void OnDisable()
    {
        _moveAction?.action?.Disable();
        _moveInput = Vector2.zero;
    }

    /// <summary>
    /// Reads the latest move input vector from the input action.
    /// </summary>
    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();
    }

    /// <summary>
    /// Applies the actor's horizontal velocity in local-space movement directions.
    /// </summary>
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
