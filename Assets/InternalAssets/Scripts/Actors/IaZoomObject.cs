using UnityEngine;
using UnityEngine.InputSystem;

public class IaZoomObject : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _zoomAction;

    [Header("Hold Point")]
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _minDistance = 0.75f;
    [SerializeField] private float _maxDistance = 3f;

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

        ok &= Guard.Expect(_zoomAction != null, "Zoom action reference is not assigned.", this);
        ok &= Guard.Expect(_zoomAction.action != null, "Zoom action is not properly set up.", this);
        ok &= Guard.Expect(_holdPoint != null, "Hold point transform is not assigned.", this);
        ok &= Guard.Expect(_zoomSpeed > 0f, "Zoom speed must be greater than 0.", this);
        ok &= Guard.Expect(_minDistance >= 0f, "Min distance must be greater than or equal to 0.", this);
        ok &= Guard.Expect(_maxDistance > _minDistance, "Max distance must be greater than min distance.", this);

        return ok;
    }

    private void OnEnable()
    {
        _zoomAction?.action?.Enable();
    }

    private void OnDisable()
    {
        _zoomAction?.action?.Disable();
    }

    private void Update()
    {
        float zoomInput = _zoomAction.action.ReadValue<float>();

        if (Mathf.Approximately(zoomInput, 0f)) return;

        Vector3 localPosition = _holdPoint.localPosition;

        localPosition.z += zoomInput * _zoomSpeed * Time.deltaTime;
        localPosition.z = Mathf.Clamp(localPosition.z, _minDistance, _maxDistance);
        _holdPoint.localPosition = localPosition;
    }
}
