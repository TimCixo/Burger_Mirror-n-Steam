using UnityEngine;

/// <summary>
/// Provides helpers for batch collision configuration.
/// </summary>
public static class CollisionUtility
{
    /// <summary>
    /// Toggles collision ignoring for every collider pair from two collider sets.
    /// </summary>
    /// <param name="a">First collider set.</param>
    /// <param name="b">Second collider set.</param>
    /// <param name="ignore"><see langword="true"/> to ignore collisions; otherwise <see langword="false"/>.</param>
    public static void SetIgnore(Collider[] a, Collider[] b, bool ignore)
    {
        if (a == null || b == null) return;

        foreach (var colliderA in a)
        {
            if (colliderA == null) continue;

            foreach (var colliderB in b)
            {
                if (colliderB == null) continue;

                Physics.IgnoreCollision(colliderA, colliderB, ignore);
            }
        }
    }
}
