using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Waits until the town has been placed and the world has been generated, then builds the navmesh in order to take the changes into account.
/// </summary>
public class NavmeshUpdater : MonoBehaviour
{
    #region Variables

    [Tooltip("The navmeshsurface that should be baked from.")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    /// <summary>
    /// Has the navmesh been backed again?
    /// </summary>
    private bool navmeshUpdated;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes sure the navmesh is regenerated at the correct moment.
    /// </summary>
    private void Update()
    {
        RegenerateNavmeshWhenSceneIsReady();
    }

    /// <summary>
    /// Regenerates the navmesh once the town has been placed and the world has been generated and filled with objects.
    /// </summary>
    private void RegenerateNavmeshWhenSceneIsReady()
    {
        if (SceneInitializer.Instance.TownPlaced && SceneInitializer.Instance.WorldGenerated && !navmeshUpdated)
        {
            navmeshUpdated = true;
            navMeshSurface.BuildNavMesh();
        }
    }

    #endregion Methods
}