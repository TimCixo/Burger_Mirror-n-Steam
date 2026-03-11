using UnityEngine;

public class CollisionCache : MonoBehaviour
{
    private Collider[] _colliders;
    public Collider[] Colliders => _colliders;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>(true);

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    private bool Validate()
    {
        return Guard.Expect(_colliders != null && _colliders.Length > 0, "No colliders found.", this);
    }
}