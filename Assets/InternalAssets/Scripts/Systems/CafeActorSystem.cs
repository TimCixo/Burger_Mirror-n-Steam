using UnityEngine;

/// <summary>
/// Coordinates actor-related scene dependencies for the cafe scene.
/// </summary>
public sealed class CafeActorSystem
{
    private readonly GameObject _player;

    /// <summary>
    /// Gets the player object registered in the actor system.
    /// </summary>
    public GameObject Player => _player;

    /// <summary>
    /// Creates the actor system for the current scene.
    /// </summary>
    /// <param name="player">Player object used by the scene actor flow.</param>
    public CafeActorSystem(GameObject player)
    {
        _player = player;
    }
}
