using System;
using UnityEngine;

/// <summary>
/// Stores a serialized debug snapshot for the scene actor system.
/// </summary>
[Serializable]
public sealed class CafeActorSystemDebugView
{
    [SerializeField] private string _playerName;

    [NonSerialized] private CafeActorSystem _system;

    /// <summary>
    /// Binds the debug view to the runtime actor system.
    /// </summary>
    /// <param name="system">Runtime actor system instance.</param>
    public void Bind(CafeActorSystem system)
    {
        _system = system;

        Refresh();
    }

    /// <summary>
    /// Clears the runtime actor system reference.
    /// </summary>
    public void Unbind()
    {
        _system = null;
    }

    /// <summary>
    /// Copies the current runtime actor data into serialized debug fields.
    /// </summary>
    public void Refresh()
    {
        if (_system == null) return;

        _playerName = _system.Player != null ? _system.Player.name : "None";
    }
}
