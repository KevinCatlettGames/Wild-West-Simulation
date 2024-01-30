using UnityEngine;

/// <summary>
/// A singleton holding booleans that state if a certain task has been completed for initialization.
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The singleton.
    /// </summary>
    public static SceneInitializer Instance;

    [Tooltip("Has the town been placed?")]
    [SerializeField] private bool townPlaced;
    public bool TownPlaced
    { get { return townPlaced; } set { townPlaced = value; } }


    [Tooltip("Has the world been generated?")]
    [SerializeField] private bool worldGenerated;
    public bool WorldGenerated
    { get { return worldGenerated; } set { worldGenerated = value; } }


    [Tooltip("Have the bisons been placed?")]
    [SerializeField] private bool bisonsPlaced;
    public bool BisonsPlaced
    { get { return bisonsPlaced; } set { bisonsPlaced = value; } }

    #endregion Variables

    /// <summary>
    /// Sets this as a singleton.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}