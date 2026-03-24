using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// Rotates the actor body on yaw and a separate pivot on pitch using look input.
/// </summary>
public class IaRotation : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input action that provides yaw and pitch look input.")]
    [SerializeField] private InputActionReference _lookAction;

    [Header("Yaw")]
    [Min(0.01f)]
    [Tooltip("Yaw rotation speed for turning the actor body left and right.")]
    [SerializeField] private float _yawSpeed = 180f;

    [Header("Pitch")]
    [Tooltip("Transform rotated locally to represent camera pitch.")]
    [SerializeField] private Transform _pitchPivot;
    [Min(0.01f)]
    [Tooltip("Pitch rotation speed for looking up and down.")]
    [SerializeField] private float _pitchSpeed = 120f;
    [Tooltip("Minimum vertical look angle in degrees.")]
    [SerializeField] private float _minPitch = -80f;
    [Tooltip("Maximum vertical look angle in degrees.")]
    [SerializeField] private float _maxPitch = 80f;

    private Rigidbody _rigidbody;
    private Vector2 _lookInput;
    private float _pitch;
    private Quaternion _pitchBaseLocalRotation;

    /// <summary>
    /// Caches required references and initializes the pitch baseline.
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }

        _pitchBaseLocalRotation = _pitchPivot.localRotation;
        _pitch = 0f;
    }

    /// <summary>
    /// Validates the input action, rigidbody and pitch pivot dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the rotation component is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_lookAction != null, "Look action reference is not assigned.", this);
        ok &= Guard.Expect(_lookAction.action != null, "Look action is not properly set up.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_pitchPivot != null, "Pitch pivot is not assigned.", this);

        return ok;
    }

    /// <summary>
    /// Enables the look input action when the component becomes active.
    /// </summary>
    private void OnEnable()
    {
        _lookAction?.action?.Enable();
    }

    /// <summary>
    /// Disables the look input action and clears the cached look input.
    /// </summary>
    private void OnDisable()
    {
        _lookAction?.action?.Disable();
        _lookInput = Vector2.zero;
    }

    /// <summary>
    /// Reads the latest look input vector from the input action.
    /// </summary>
    private void Update()
    {
        _lookInput = _lookAction.action.ReadValue<Vector2>();
    }

    /// <summary>
    /// Applies yaw to the actor rigidbody and pitch to the configured pivot.
    /// </summary>
    private void FixedUpdate()
    {
        float yawDelta = _lookInput.x * _yawSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, yawDelta, 0f);

        _rigidbody.MoveRotation(_rigidbody.rotation * yawRotation);

        _pitch -= _lookInput.y * _pitchSpeed * Time.fixedDeltaTime;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        _pitchPivot.localRotation = _pitchBaseLocalRotation * Quaternion.Euler(_pitch, 0f, 0f);
    }
}
