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
    }

    private void OnEnable()
    {
        _moveAction?.action?.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.action?.Disable();
    }

    private void Update()
    {
        if (_moveAction == null || _moveAction.action == null)
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = _moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (_rigidbody == null)
        {
            return;
        }

        Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);

        direction = transform.TransformDirection(direction);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0f)
        {
            direction.Normalize();
        }

        Vector3 delta = direction * _moveSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + delta);
    }
}
