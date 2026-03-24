using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adjusts the hold point distance used by the hold system.
/// </summary>
public class IaZoomObject : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _zoomAction;

    [Header("Hold Point")]
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _minDistance = 0.75f;
    [SerializeField] private float _maxDistance = 3f;

    /// <summary>
    /// Disables the behaviour when the zoom setup is invalid.
    /// </summary>
    private void Awake()
    {
        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates the zoom input action and hold point configuration.
    /// </summary>
    /// <returns><see langword="true"/> when the zoom component is configured correctly.</returns>
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

    /// <summary>
    /// Enables the zoom input action when the component becomes active.
    /// </summary>
    private void OnEnable()
    {
        _zoomAction?.action?.Enable();
    }

    /// <summary>
    /// Disables the zoom input action when the component becomes inactive.
    /// </summary>
    private void OnDisable()
    {
        _zoomAction?.action?.Disable();
    }

    /// <summary>
    /// Moves the hold point along its local forward axis based on zoom input.
    /// </summary>
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
