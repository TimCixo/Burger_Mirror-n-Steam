using UnityEngine;

[RequireComponent(typeof(BurgerData))]
public class StackHolder : MonoBehaviour
{
    [SerializeField] private GameObject _stackPoint;

    private BurgerData _burgerData;
    private Collider _triggerCollider;

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

    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_burgerData != null, "BurgerData component is missing.", this);
        ok &= Guard.Expect(_stackPoint != null, "Trigger object is not assigned.", this);
        ok &= Guard.Expect(_triggerCollider != null, "Trigger object must contain a Collider component.", this);
        ok &= Guard.Expect(_triggerCollider != null && _triggerCollider.isTrigger, "Trigger object collider must be a trigger.", this);

        return ok;
    }

    private void OnTriggerEnter(Collider other)
    {
        StackableObject stackableObject = other.GetComponentInParent<StackableObject>();

        if (stackableObject == null) return;
        if (stackableObject.transform.IsChildOf(transform)) return;

        stackableObject.AttachTo(transform, _stackPoint.transform);
        MoveStackPoint(stackableObject.TopPoint.position);
        stackableObject.AddTo(_burgerData);
    }

    private void MoveStackPoint(Vector3 position)
    {
        _stackPoint.transform.position = position;
    }
}
