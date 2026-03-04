using UnityEngine;

public static class Guard
{
    [HideInCallstack]
    public static bool Expect(bool condition, string message, Object context = null)
    {
        if (condition) return true;

        Debug.LogError(message, context);
        return false;
    }
}