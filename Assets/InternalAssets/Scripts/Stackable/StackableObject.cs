using UnityEngine;

[RequireComponent(typeof(HoldableObject))]
[RequireComponent(typeof(Rigidbody))]
public class StackableObject : MonoBehaviour
{
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _bottomPoint;

    private HoldableObject _holdableObject;
    private Rigidbody _rigidbody;

    public Transform TopPoint => _topPoint;
    public Transform BottomPoint => _bottomPoint;

    private void Awake()
    {
        _holdableObject = GetComponent<HoldableObject>();
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    public bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_holdableObject != null, "HoldableObject component is missing.", this);
        ok &= Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
        ok &= Guard.Expect(_topPoint != null, "Top point is not assigned.", this);
        ok &= Guard.Expect(_bottomPoint != null, "Bottom point is not assigned.", this);

        return ok;
    }

    public void AttachTo(Transform parent, Transform stackPoint)
    {
        transform.SetParent(parent);
        transform.rotation = stackPoint.rotation;
        transform.position += stackPoint.position - _bottomPoint.position;
        _rigidbody.isKinematic = true;
        _holdableObject.enabled = false;
    }

    public void AddTo(BurgerData burgerData)
    {
        burgerData.Add(this);
    }
}
