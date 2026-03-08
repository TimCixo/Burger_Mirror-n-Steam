using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    public bool Validate()
    {
        return Guard.Expect(_rigidbody != null, "Rigidbody component is missing.", this);
    }

    public void OnPickUp(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _rigidbody.isKinematic = true;
    }
    public void OnDrop()
    {
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
    }
}