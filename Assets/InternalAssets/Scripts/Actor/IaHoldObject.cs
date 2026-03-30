using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CollisionCache))]
/// <summary>
/// Finds holdable objects in front of the camera and manages the actor's active held item.
/// </summary>
public class IaHoldObject : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input action used to pick up an object on press and drop it on release.")]
    [SerializeField] private InputActionReference _holdAction;

    [Header("References")]
    [Tooltip("Camera used to raycast from the screen center when searching for holdable objects.")]
    [SerializeField] private Camera _camera;
    [Tooltip("Target transform that defines where the held object should follow.")]
    [SerializeField] private Transform _holdPoint;

    [Header("Pickup")]
    [Min(0.01f)]
    [Tooltip("Maximum raycast distance for finding a holdable object in front of the player.")]
    [SerializeField] private float _rayDistance = 3f;
    [Tooltip("Layers considered valid for holdable object pickup raycasts.")]
    [SerializeField] private LayerMask _holdableMask = ~0;

    private HoldableObject _heldObject;
    private CollisionCache _playerCollisionCache;
    private CollisionCache _heldCollisionCache;

    /// <summary>
    /// Gets the holdable object currently carried by the actor.
    /// </summary>
    public HoldableObject HeldObject => _heldObject;

    /// <summary>
    /// Caches required references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        _playerCollisionCache = GetComponentInParent<CollisionCache>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates the configured input action, camera, hold point and collision dependencies.
    /// </summary>
    /// <returns><see langword="true"/> when the hold component is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_holdAction != null, "Hold action reference is not assigned.", gameObject);
        ok &= Guard.Expect(_holdAction.action != null, "Hold action is not properly set up.", gameObject);
        ok &= Guard.Expect(_camera != null, "Camera is not assigned and Camera.main was not found.", gameObject);
        ok &= Guard.Expect(_holdPoint != null, "Hold point transform is not assigned.", gameObject);
        ok &= Guard.Expect(_playerCollisionCache != null, "Player collision cache is not assigned.", gameObject);
        ok &= Guard.Expect(_rayDistance > 0f, "Ray distance must be greater than 0.", gameObject);
        ok &= Guard.Expect(_holdableMask.value != 0, "Holdable mask is empty. Assign at least one layer.", gameObject);

        return ok;
    }

    /// <summary>
    /// Enables the hold action and subscribes to hold input callbacks.
    /// </summary>
    private void OnEnable()
    {
        _holdAction?.action?.Enable();
        _holdAction.action.performed += OnHoldPerformed;
        _holdAction.action.canceled += OnHoldCanceled;
    }

    /// <summary>
    /// Unsubscribes from hold input callbacks and disables the action.
    /// </summary>
    private void OnDisable()
    {
        _holdAction.action.performed -= OnHoldPerformed;
        _holdAction.action.canceled -= OnHoldCanceled;
        _holdAction?.action?.Disable();
    }

    /// <summary>
    /// Cleans stale held-object references and draws the debug pickup ray.
    /// </summary>
    private void Update()
    {
        if (_heldObject != null && (!_heldObject.enabled || !_heldObject.IsHeld))
        {
            ReleaseHeldObjectReference();
        }

        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * _rayDistance, Color.red);
    }

    /// <summary>
    /// Attempts to pick up the nearest valid holdable when hold input is performed.
    /// </summary>
    /// <param name="context">Input callback context for the hold action.</param>
    private void OnHoldPerformed(InputAction.CallbackContext context)
    {
        if (TryFindHoldable(out HoldableObject holdable))
        {
            PickUp(holdable);
        }
    }

    /// <summary>
    /// Searches for the closest enabled holdable hit by the center-screen raycast.
    /// </summary>
    /// <param name="holdable">Resolved holdable object when one is found.</param>
    /// <returns><see langword="true"/> when a valid holdable object is found.</returns>
    private bool TryFindHoldable(out HoldableObject holdable)
    {
        holdable = null;

        Vector3 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = _camera.ScreenPointToRay(screenCenter);

        RaycastHit[] hits = Physics.RaycastAll(ray, _rayDistance, _holdableMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            HoldableObject foundHoldable = FindEnabledHoldable(hit.collider.transform);
            if (foundHoldable == null) continue;

            holdable = foundHoldable;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Walks up a transform hierarchy to find an enabled, currently unheld holdable component.
    /// </summary>
    /// <param name="current">Transform where the search should start.</param>
    /// <returns>The first matching <see cref="HoldableObject"/> in the hierarchy, or <see langword="null"/>.</returns>
    private HoldableObject FindEnabledHoldable(Transform current)
    {
        while (current != null)
        {
            HoldableObject holdable = current.GetComponent<HoldableObject>();
            if (holdable != null && holdable.enabled && !holdable.IsHeld)
            {
                return holdable;
            }

            current = current.parent;
        }

        return null;
    }

    /// <summary>
    /// Drops the currently held object when the hold input is released.
    /// </summary>
    /// <param name="context">Input callback context for the hold action.</param>
    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        TryDrop();
    }

    /// <summary>
    /// Starts holding the provided object and ignores collisions between the actor and that object.
    /// </summary>
    /// <param name="holdable">Object to pick up.</param>
    public void PickUp(HoldableObject holdable)
    {
        if (_heldObject != null) TryDrop();

        if (holdable == null || !holdable.enabled || holdable.IsHeld) return;

        _heldCollisionCache = holdable.GetComponentInParent<CollisionCache>();

        CollisionUtility.SetIgnore(_playerCollisionCache.Colliders, _heldCollisionCache?.Colliders, true);

        _heldObject = holdable;
        _heldObject.BeginHold(_holdPoint);
    }

    /// <summary>
    /// Stops holding the current object and restores actor-object collisions.
    /// </summary>
    /// <returns><see langword="true"/> when an object was dropped; otherwise <see langword="false"/>.</returns>
    public bool TryDrop()
    {
        if (_heldObject == null) return false;

        _heldObject.EndHold();
        ReleaseHeldObjectReference();
        return true;
    }

    /// <summary>
    /// Clears the held-object reference and re-enables actor collisions for it.
    /// </summary>
    private void ReleaseHeldObjectReference()
    {
        CollisionUtility.SetIgnore(_playerCollisionCache.Colliders, _heldCollisionCache?.Colliders, false);
        _heldObject = null;
        _heldCollisionCache = null;
    }
}
