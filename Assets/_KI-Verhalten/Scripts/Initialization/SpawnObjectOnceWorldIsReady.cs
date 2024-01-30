using System.Collections;
using UnityEngine;

/// <summary>
/// Waits until the world has been generated and objects / town has been placed, then instantiates a GameObject.
/// </summary>
public class SpawnObjectOnceWorldIsReady : MonoBehaviour
{
    #region Variables

    [Tooltip("The position where the GameObject should be spawned.")]
    [SerializeField] private GameObject spawnPosition;

    [Tooltip("The prefab that should be instantiated.")]
    [SerializeField] private GameObject prefab;

    [Tooltip("The duration until spawning occurs, once the scene is ready for it to happen.")]
    [SerializeField] private int spawnDelay = 10;

    /// <summary>
    /// Has spawning occured?
    /// </summary>
    private bool spawned;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes sure spawning occurs at the correct moment.
    /// </summary>
    private void Update()
    {
        CheckIfSpawningCanOccur();
    }

    /// <summary>
    /// Begins the spawning process once the town is placed and the world and the objects it entails are generated.
    /// </summary>
    private void CheckIfSpawningCanOccur()
    {
        if (spawned) return;

        if (SceneInitializer.Instance != null)
        {
            if (SceneInitializer.Instance.TownPlaced && SceneInitializer.Instance.WorldGenerated)
            {
                spawned = true;
                StartCoroutine(SpawnPrefabCoroutine());
            }
        }
    }

    /// <summary>
    /// Waits for a duration then spawn the GameObject.
    /// </summary>
    /// <returns></returns> The <see cref="spawnDelay"/> until the GameObject is spawned.
    private IEnumerator SpawnPrefabCoroutine()
    {
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(prefab, spawnPosition.transform.position, Quaternion.identity);
    }

    #endregion Methods
}