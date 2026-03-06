using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class IaJump : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _jumpAction;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 6f;

    [Header("Ground Check (Trigger)")]
    [SerializeField] private LayerMask _groundMask;

    private Rigidbody _rigidbody;
    private bool _jumpQueued;
    private int _groundContacts;

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

        ok &= Guard.Expect(_jumpAction != null, "Jump action reference is not assigned.", this);
        ok &= Guard.Expect(_jumpAction.action != null, "Jump action is not properly set up.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_groundMask.value != 0, "Ground mask is empty. Assign a ground layer.", this);

        return ok;
    }

    private void OnEnable()
    {
        _jumpAction?.action?.Enable();
        _jumpAction.action.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        _jumpAction.action.performed -= OnJumpPerformed;
        _jumpAction?.action?.Disable();
        _jumpQueued = false;
        _groundContacts = 0;
    }

    private void FixedUpdate()
    {
        if (!_jumpQueued || _groundContacts <= 0)
        {
            _jumpQueued = false;
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = 0f;
        _rigidbody.linearVelocity = velocity;
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        _jumpQueued = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext _)
    {
        _jumpQueued = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || !IsInGroundMask(other.gameObject.layer))
        {
            return;
        }

        _groundContacts++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger || !IsInGroundMask(other.gameObject.layer))
        {
            return;
        }

        _groundContacts = Mathf.Max(0, _groundContacts - 1);
    }

    private bool IsInGroundMask(int layer)
    {
        int layerBit = 1 << layer;
        return (_groundMask.value & layerBit) != 0;
    }
}
