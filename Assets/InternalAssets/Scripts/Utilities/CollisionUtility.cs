using UnityEngine;

public static class CollisionUtility
{
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