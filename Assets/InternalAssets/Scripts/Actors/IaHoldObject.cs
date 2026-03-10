using UnityEngine;
using UnityEngine.InputSystem;

public class IaHoldObject : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _holdAction;

    [Header("Hold")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private LayerMask _holdableMask = ~0;

    private HoldableObject _heldObject;
    public HoldableObject HeldObject => _heldObject;

    private void Awake()
    {
        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_holdAction != null, "Hold action reference is not assigned.", this);
        ok &= Guard.Expect(_holdAction.action != null, "Hold action is not properly set up.", this);
        ok &= Guard.Expect(_camera != null, "Camera is not assigned and Camera.main was not found.", this);
        ok &= Guard.Expect(_holdPoint != null, "Hold point transform is not assigned.", this);
        ok &= Guard.Expect(_rayDistance > 0f, "Ray distance must be greater than 0.", this);
        ok &= Guard.Expect(_holdableMask.value != 0, "Holdable mask is empty. Assign at least one layer.", this);

        return ok;
    }

    private void OnEnable()
    {
        _holdAction?.action?.Enable();
        _holdAction.action.performed += OnHoldPerformed;
        _holdAction.action.canceled += OnHoldCanceled;
    }

    private void OnDisable()
    {
        _holdAction.action.performed -= OnHoldPerformed;
        _holdAction.action.canceled -= OnHoldCanceled;
        _holdAction?.action?.Disable();
    }

    private void Update()
    {
        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * _rayDistance, Color.red);
    }

    private void OnHoldPerformed(InputAction.CallbackContext context)
    {
        if (TryFindHoldable(out HoldableObject holdable))
        {
            PickUp(holdable);
        }
    }

    private bool TryFindHoldable(out HoldableObject holdable)
    {
        holdable = null;

        Vector3 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = _camera.ScreenPointToRay(screenCenter);

        RaycastHit[] hits = Physics.RaycastAll(ray, _rayDistance, _holdableMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            HoldableObject foundHoldable = hit.collider.GetComponentInParent<HoldableObject>();
            if (foundHoldable == null) continue;

            holdable = foundHoldable;
            return true;
        }

        return false;
    }

    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        TryDrop();
    }

    public void PickUp(HoldableObject holdable)
    {
        if (_heldObject != null) TryDrop();

        _heldObject = holdable;
        _heldObject.OnPickUp(_holdPoint.transform);
    }

    public bool TryDrop()
    {
        if (_heldObject == null) return false;

        _heldObject.OnDrop();
        _heldObject = null;
        return true;
    }
}
