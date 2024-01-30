using System.Collections;
using UnityEngine;

/// <summary>
/// Handles initializing the different GameObjects placed in the world.
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    #region Variables

    [Tooltip("The biome data currently being represented in the scene.")]
    [SerializeField] private BiomeData biomeData;
    public BiomeData BiomeData
    { get { return biomeData; } set { biomeData = value; } }


    [Tooltip("The MapGenerator that is initialized in the scene, used for regenerating a random terrain.")]
    [SerializeField] private MapGenerator mapGenerator;

    [Tooltip("The meshFilter that should hold the newly generated mesh.")]
    [SerializeField] private MeshFilter terrainMeshFilter;

    [Tooltip("The position the activated GameObjects get positioned at at first.")]
    [SerializeField] private Vector3 initialSpawnPoint = new Vector3(0, 100, 0);

    /// <summary>
    /// Has the environment detail been placed?
    /// </summary>
    private bool spawned;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Generates a new terrain when the scene is first loaded.
    /// </summary>
    private void Awake()
    {
        DrawMap();
    }

    /// <summary>
    /// Once the town is placed makes sure the environment objects are placed.
    /// </summary>
    private void Update()
    {
        PlaceEnvironmentDetail();
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Calls the Spawning method once the town is placed.
    /// </summary>
    private void PlaceEnvironmentDetail()
    {
        if (!spawned && SceneInitializer.Instance.TownPlaced)
        {
            spawned = true;
            StartCoroutine(Spawning());
        }
    }

    /// <summary>
    /// Draws the terrain when the scene first loads to make a random world each time.
    /// </summary>
    private void DrawMap()
    {
        mapGenerator.DrawMapInEditor();
    }

    /// <summary>
    /// Takes each pool in the given world type and spawns GameObjects from those pools.
    /// Afterwards tell these GameObjects to find a random position on the terrain.
    /// </summary>
    /// <returns></returns> A short duration to make sure no inconsistency occurs.
    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(1f);
        // Take every pool
        foreach (Pool pool in biomeData.Pools)
        {
            // Take every tag in the pool
            foreach (string tag in pool.Tags)
            {
                int amount;
                // Check if a random amount of GameObjects from this pool should be activated.
                if (pool.RandomizeSpawnAmount)
                    amount = Random.Range(pool.MinPlaceAmount, pool.MaxPlaceAmount);
                else
                    amount = pool.MaxPlaceAmount;

                // Activate the GameObjects
                for (int i = 0; i < amount; i++)
                {
                    // The random value indicates from the current pool, the GameObjects with what tag should be activated.
                    int randomTag = Random.Range(0, pool.Tags.Length);

                    // Call to the Object Pool system
                    ObjectPool.Instance.SpawnFromPool(pool.Tags[randomTag], initialSpawnPoint, Quaternion.identity);
                }
            }
        }
        // Tell the newly activated GameObjects to find a random position.
        foreach (Transform t in ObjectPool.Instance.ActiveObjects)
        {
            t.GetComponent<PlaceOnGround>().FindRandomPosition();
            t.GetComponent<PlaceOnGround>().RaycastAndPlace();
        }

        // Tell the scene initializer that the objects have been activated.
        SceneInitializer.Instance.WorldGenerated = true;
    }

    #endregion Methods

}