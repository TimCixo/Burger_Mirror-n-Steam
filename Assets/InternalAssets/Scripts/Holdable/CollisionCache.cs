using UnityEngine;

/// <summary>
/// Caches all colliders in a hierarchy for repeated collision ignore operations.
/// </summary>
public class CollisionCache : MonoBehaviour
{
    private Collider[] _colliders;

    /// <summary>
    /// Gets the cached colliders from this object hierarchy.
    /// </summary>
    public Collider[] Colliders => _colliders;

    /// <summary>
    /// Collects colliders from the hierarchy and disables the behaviour when none are found.
    /// </summary>
    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>(true);

        if (!Validate())
        {
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Validates that at least one collider was cached.
    /// </summary>
    /// <returns><see langword="true"/> when the cache contains colliders.</returns>
    private bool Validate()
    {
        return Guard.Expect(_colliders != null && _colliders.Length > 0, "No colliders found.", this);
    }
}
