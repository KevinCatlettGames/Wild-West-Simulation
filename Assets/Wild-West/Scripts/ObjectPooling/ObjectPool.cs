using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes many different pools of GameObjects and keeps the stored inside of an Dictionary.
/// This ObjectPool allows for activation of GameObjects from many different pools and still keeps
/// all the functionality a objectpool should have.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// This class as singleton.
    /// </summary>
    public static ObjectPool Instance;

    [Tooltip("The Biome Data scriptable object to take the Pools from and create instanced of GameObjects with.")]
    [SerializeField] private BiomeData biomeData;

    public BiomeData CurrentBiomeData
    { get { return biomeData; } set { biomeData = value; } }

    /// <summary>
    /// The dictionary storing the tags of the pools as keys and the different pools as values.
    /// </summary>
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    /// <summary>
    /// A list of currently active objects.
    /// </summary>
    private List<Transform> activeObjects;

    public List<Transform> ActiveObjects
    { get { return activeObjects; } }

    #endregion Variables

    #region Methods

    /// <summary>
    /// Creates the singleton and initialized the activeObjects list.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        activeObjects = new List<Transform>();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        InitializeEnvironmentPools();
    }

    /// <summary>
    /// Takes a GameObject from a pool and actives it by putting it at the give position.
    /// </summary>
    /// <param name="tag"></param> The Tag of the pool of which a GameObject should be taken.
    /// <param name="position"></param> The position where the GameObject should be placed at.
    /// <param name="rotation"></param> The rotation of the GameObject.
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        // Gets the object that should be spawned.
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Initialize it.
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        activeObjects.Add(objectToSpawn.transform);
        poolDictionary[tag].Enqueue(objectToSpawn);

        // Return the initialized object.
        return objectToSpawn;
    }

    /// <summary>
    /// Deactivates the given object and makes this gameObject its parent, essentially resetting it and
    /// adding it back to the deactivated pool items.
    /// </summary>
    /// <param name="transformToReuse"></param> The transform to deactivate and reset.
    public void ReuseObject(Transform transformToReuse)
    {
        if (transformToReuse != null)
        {
            transformToReuse.SetParent(this.transform);
            transformToReuse.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Quickly deactivate all objects that are currently active.
    /// </summary>
    public void ResetAllObjects()
    {
        if (activeObjects.Count > 0)
        {
            foreach (Transform t in activeObjects)
                ReuseObject(t);

            activeObjects = new List<Transform>();
        }
    }

    /// <summary>
    /// Takes every pool in the <see cref="BiomeData.pools"/> list and creates a ObjectPool for it
    /// with its tag and GameObject/s that are declared in the pools list.
    /// </summary>
    public void InitializeEnvironmentPools()
    {
        // Iterate through all pools.
        foreach (Pool pool in biomeData.Pools)
        {
            // Check if the pool already has a pool with the same tag.
            if (!poolDictionary.ContainsKey(pool.Tags[0]))
            {
                // For each tag in the pool
                for (int j = 0; j < pool.Tags.Length; j++)
                {
                    // Initialize a new queue
                    Queue<GameObject> objectPool = new Queue<GameObject>();

                    // Iterate as often that there should be GameObjects loaded in.
                    for (int i = 0; i < pool.LoadAmount; i++)
                    {
                        // Instantiate the currently looked at GameObject.
                        GameObject obj = Instantiate(pool.Objects[j]);

                        // Set the necessary values to make the GameObject idle.
                        obj.SetActive(false);
                        obj.transform.SetParent(this.gameObject.transform);
                        obj.GetComponent<PoolTag>().Tag = pool.Tags[j];
                        objectPool.Enqueue(obj);
                    }

                    // Add the newly created pool to the dictionary with the tag and the queue pool.
                    poolDictionary.Add(pool.Tags[j], objectPool);
                }
            }
        }
    }

    #endregion Methods
}