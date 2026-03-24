using UnityEngine;

/// <summary>
/// Provides lightweight runtime assertions for component configuration.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Verifies that a condition is true and logs an error when it is not.
    /// </summary>
    /// <param name="condition">Condition that is expected to be true.</param>
    /// <param name="message">Error message to log when the condition fails.</param>
    /// <param name="context">Unity object used as the log context.</param>
    /// <returns><see langword="true"/> when the condition passes; otherwise <see langword="false"/>.</returns>
    [HideInCallstack]
    public static bool Expect(bool condition, string message, Object context = null)
    {
        if (condition) return true;

        Debug.LogError(message, context);
        return false;
    }
}
