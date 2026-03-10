using UnityEngine;
using UnityEngine.InputSystem;

public class IaThrowObject : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _throwAction;

    [Header("References")]
    [SerializeField] private IaHoldObject _holdObject;
    [SerializeField] private Camera _camera;

    [Header("Throw")]
    [SerializeField] private float _throwForce = 8f;
    [SerializeField] private ForceMode _forceMode = ForceMode.Impulse;

    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

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

    private void OnEnable()
    {
        _throwAction?.action?.Enable();
        _throwAction.action.performed += OnThrowPerformed;
    }

    private void OnDisable()
    {
        _throwAction.action.performed -= OnThrowPerformed;
        _throwAction?.action?.Disable();
    }

    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        TryThrowHeldObject();
    }

    public bool TryThrowHeldObject()
    {
        HoldableObject heldObject = _holdObject.HeldObject;

        if (!_holdObject.TryDrop()) return false;

        Rigidbody holdableRigidbody = heldObject.GetComponent<Rigidbody>();

        if (holdableRigidbody == null) return false;

        Vector3 throwDirection = _camera.transform.forward.normalized;
        holdableRigidbody.AddForce(throwDirection * _throwForce, _forceMode);

        return true;
    }
}
