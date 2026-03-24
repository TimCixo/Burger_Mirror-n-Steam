using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Queues and applies jump impulses while the actor has valid ground trigger contacts.
/// </summary>
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
    /// Validates the input action, rigidbody and ground mask dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the jump component is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_jumpAction != null, "Jump action reference is not assigned.", this);
        ok &= Guard.Expect(_jumpAction.action != null, "Jump action is not properly set up.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_groundMask.value != 0, "Ground mask is empty. Assign a ground layer.", this);

        return ok;
    }

    /// <summary>
    /// Enables the jump action and subscribes to performed events.
    /// </summary>
    private void OnEnable()
    {
        _jumpAction?.action?.Enable();
        _jumpAction.action.performed += OnJumpPerformed;
    }

    /// <summary>
    /// Unsubscribes from input events and clears cached jump state.
    /// </summary>
    private void OnDisable()
    {
        _jumpAction.action.performed -= OnJumpPerformed;
        _jumpAction?.action?.Disable();
        _jumpQueued = false;
        _groundContacts = 0;
    }

    /// <summary>
    /// Applies the queued jump if the actor is grounded during the current physics step.
    /// </summary>
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

    /// <summary>
    /// Queues a jump for the next physics step.
    /// </summary>
    /// <param name="_">Unused jump callback context.</param>
    private void OnJumpPerformed(InputAction.CallbackContext _)
    {
        _jumpQueued = true;
    }

    /// <summary>
    /// Counts valid ground trigger contacts when the actor enters a ground collider.
    /// </summary>
    /// <param name="other">Collider entered by the ground-check trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || !IsInGroundMask(other.gameObject.layer))
        {
            return;
        }

        _groundContacts++;
    }

    /// <summary>
    /// Removes valid ground trigger contacts when the actor leaves a ground collider.
    /// </summary>
    /// <param name="other">Collider exited by the ground-check trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger || !IsInGroundMask(other.gameObject.layer))
        {
            return;
        }

        _groundContacts = Mathf.Max(0, _groundContacts - 1);
    }

    /// <summary>
    /// Determines whether a layer is included in the configured ground mask.
    /// </summary>
    /// <param name="layer">Layer index to test.</param>
    /// <returns><see langword="true"/> when the layer belongs to the ground mask.</returns>
    private bool IsInGroundMask(int layer)
    {
        int layerBit = 1 << layer;
        return (_groundMask.value & layerBit) != 0;
    }
}
