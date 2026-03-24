using UnityEngine;

[RequireComponent(typeof(BurgerData))]
/// <summary>
/// Accepts stackable objects through a trigger zone and builds a burger stack from them.
/// </summary>
public class StackHolder : MonoBehaviour
{
    [Tooltip("Game object whose transform and trigger define the next stack insertion point.")]
    [SerializeField] private GameObject _stackPoint;

    private BurgerData _burgerData;
    private Collider _triggerCollider;

    /// <summary>
    /// Caches required references and disables the behaviour when setup is invalid.
    /// </summary>
    private void Awake()
    {
        _burgerData = GetComponent<BurgerData>();
        _triggerCollider = _stackPoint.GetComponent<Collider>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates the stack holder setup and trigger configuration.
    /// </summary>
    /// <returns><see langword="true"/> when the holder can accept stackable objects.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_burgerData != null, "BurgerData component is missing.", this);
        ok &= Guard.Expect(_stackPoint != null, "Trigger object is not assigned.", this);
        ok &= Guard.Expect(_triggerCollider != null, "Trigger object must contain a Collider component.", this);
        ok &= Guard.Expect(_triggerCollider != null && _triggerCollider.isTrigger, "Trigger object collider must be a trigger.", this);

        return ok;
    }

    /// <summary>
    /// Attaches incoming stackable objects to the current stack and advances the stack point.
    /// </summary>
    /// <param name="other">Collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        StackableObject stackableObject = other.GetComponentInParent<StackableObject>();

        if (stackableObject == null) return;
        if (!stackableObject.CanAttachToStack) return;
        if (stackableObject.transform.IsChildOf(transform)) return;

        stackableObject.AttachTo(transform, _stackPoint.transform);
        MoveStackPoint(stackableObject.TopPoint.position);
        stackableObject.AddTo(_burgerData);
    }

    /// <summary>
    /// Moves the trigger stack point to the provided world-space position.
    /// </summary>
    /// <param name="position">Next position for the stack point.</param>
    private void MoveStackPoint(Vector3 position)
    {
        _stackPoint.transform.position = position;
    }
}
