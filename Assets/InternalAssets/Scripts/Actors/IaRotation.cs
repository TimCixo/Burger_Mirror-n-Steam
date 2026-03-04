using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class IaRotation : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _lookAction;

    [Header("Yaw")]
    [SerializeField] private float _yawSpeed = 180f;

    [Header("Pitch")]
    [SerializeField] private Transform _pitchPivot;
    [SerializeField] private float _pitchSpeed = 120f;
    [SerializeField] private float _minPitch = -80f;
    [SerializeField] private float _maxPitch = 80f;

    private Rigidbody _rigidbody;
    private Vector2 _lookInput;
    private float _pitch;
    private Quaternion _pitchBaseLocalRotation;

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

    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_lookAction != null, "Look action reference is not assigned.", this);
        ok &= Guard.Expect(_lookAction.action != null, "Look action is not properly set up.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_pitchPivot != null, "Pitch pivot is not assigned.", this);

        return ok;
    }

    private void OnEnable()
    {
        _lookAction?.action?.Enable();
    }

    private void OnDisable()
    {
        _lookAction?.action?.Disable();
        _lookInput = Vector2.zero;
    }

    private void Update()
    {
        _lookInput = _lookAction.action.ReadValue<Vector2>();
    }

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
