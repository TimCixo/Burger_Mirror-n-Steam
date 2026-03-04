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

        if (_pitchPivot == null) return;

        _pitchBaseLocalRotation = _pitchPivot.localRotation;
        _pitch = 0f;
    }

    private void OnEnable()
    {
        _lookAction?.action?.Enable();
    }

    private void OnDisable()
    {
        _lookAction?.action?.Disable();
    }

    private void Update()
    {
        if (_lookAction == null || _lookAction.action == null)
        {
            _lookInput = Vector2.zero;
            return;
        }

        _lookInput = _lookAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        float yawDelta = _lookInput.x * _yawSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, yawDelta, 0f);

        _rigidbody.MoveRotation(_rigidbody.rotation * yawRotation);

        if (_pitchPivot == null) return;

        _pitch -= _lookInput.y * _pitchSpeed * Time.fixedDeltaTime;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        _pitchPivot.localRotation = _pitchBaseLocalRotation * Quaternion.Euler(_pitch, 0f, 0f);
    }
}
