using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Throws the currently held object in the camera forward direction.
/// </summary>
public class IaThrowObject : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input action used to throw the currently held object.")]
    [SerializeField] private InputActionReference _throwAction;

    [Header("References")]
    [Tooltip("Hold component that provides the currently held object.")]
    [SerializeField] private IaHoldObject _holdObject;
    [Tooltip("Camera used to determine the throw direction.")]
    [SerializeField] private Camera _camera;

    [Header("Throw")]
    [Min(0.01f)]
    [Tooltip("Force magnitude applied when the held object is thrown.")]
    [SerializeField] private float _throwForce = 8f;
    [Tooltip("Force mode used when applying the throw impulse.")]
    [SerializeField] private ForceMode _forceMode = ForceMode.Impulse;

    /// <summary>
    /// Resolves default references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates the input action and throw dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the throw component is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_throwAction != null, "Throw action reference is not assigned.", this);
        ok &= Guard.Expect(_throwAction.action != null, "Throw action is not properly set up.", this);
        ok &= Guard.Expect(_holdObject != null, "IaHoldObject reference is not assigned.", this);
        ok &= Guard.Expect(_camera != null, "Camera is not assigned and Camera.main was not found.", this);
        ok &= Guard.Expect(_throwForce > 0f, "Throw force must be greater than 0.", this);

        return ok;
    }

    /// <summary>
    /// Enables the throw input action and subscribes to performed events.
    /// </summary>
    private void OnEnable()
    {
        _throwAction?.action?.Enable();
        _throwAction.action.performed += OnThrowPerformed;
    }

    /// <summary>
    /// Unsubscribes from throw input callbacks and disables the action.
    /// </summary>
    private void OnDisable()
    {
        _throwAction.action.performed -= OnThrowPerformed;
        _throwAction?.action?.Disable();
    }

    /// <summary>
    /// Tries to throw the currently held object when throw input is performed.
    /// </summary>
    /// <param name="context">Input callback context for the throw action.</param>
    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        TryThrowHeldObject();
    }

    /// <summary>
    /// Drops the current held object and applies an impulse in the camera forward direction.
    /// </summary>
    /// <returns><see langword="true"/> when an object was successfully thrown.</returns>
    public bool TryThrowHeldObject()
    {
        HoldableObject heldObject = _holdObject.HeldObject;

        if (!_holdObject.TryDrop()) return false;

        Rigidbody holdableRigidbody = heldObject.GetComponent<Rigidbody>();

        if (holdableRigidbody == null) return false;

        heldObject.ClearAngularVelocity();

        Vector3 throwDirection = _camera.transform.forward.normalized;
        holdableRigidbody.AddForce(throwDirection * _throwForce, _forceMode);

        return true;
    }
}
