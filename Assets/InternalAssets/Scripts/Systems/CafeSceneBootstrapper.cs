using UnityEngine;

/// <summary>
/// Acts as the scene composition root for runtime systems used in the cafe scene.
/// </summary>
public class CafeSceneBootstrapper : MonoBehaviour
{
    [Header("Actor System")]
    [Tooltip("Player object passed into the cafe actor system at scene startup.")]
    [SerializeField] private GameObject _player;

    [Header("Debug")]
    [Tooltip("Serialized debug snapshot refreshed from the runtime cafe actor system during play mode.")]
    [SerializeField] private CafeActorSystemDebugView _actorSystemDebug = new();
    [Tooltip("Serialized debug snapshot refreshed from the runtime cafe ingredient system during play mode.")]
    [SerializeField] private CafeIngredientSystemDebugView _ingredientSystemDebug = new();

    private CafeActorSystem _actorSystem;
    private CafeIngredientSystem _ingredientSystem;

    /// <summary>
    /// Validates serialized scene dependencies and creates runtime systems.
    /// </summary>
    private void Awake()
    {
        if (!Validate())
        {
            enabled = false;
            return;
        }

        IngredientData[] ingredients = FindObjectsByType<IngredientData>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        _actorSystem = new CafeActorSystem(_player);
        _ingredientSystem = new CafeIngredientSystem();

        for (int i = 0; i < ingredients.Length; i++)
        {
            _ingredientSystem.Register(ingredients[i]);
        }

        _actorSystemDebug.Bind(_actorSystem);
        _ingredientSystemDebug.Bind(_ingredientSystem);
    }

    /// <summary>
    /// Validates scene references required by the bootstrapper.
    /// </summary>
    /// <returns><see langword="true"/> when the bootstrapper is configured correctly.</returns>
    private bool Validate()
    {
        bool ok = true;

        ok &= Guard.Expect(_player != null, "Player is not assigned.", gameObject);

        return ok;
    }

    /// <summary>
    /// Refreshes serialized debug data during play mode.
    /// </summary>
    private void Update()
    {
        _actorSystemDebug.Refresh();
        _ingredientSystemDebug.Refresh();
    }

    /// <summary>
    /// Releases runtime bindings used by serialized debug views.
    /// </summary>
    private void OnDestroy()
    {
        _actorSystemDebug.Unbind();
        _ingredientSystemDebug.Unbind();
    }
}
