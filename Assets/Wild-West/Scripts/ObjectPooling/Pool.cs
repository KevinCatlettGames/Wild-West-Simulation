using UnityEngine;

/// <summary>
/// Holds all information that the object pooling system needs to initialize a pool.
/// The Object pool uses many pools to take GameObjects from, which can be accessed by the given tags.
/// </summary>
[CreateAssetMenu()]
public class Pool : ScriptableObject
{
    [Tooltip("The name of this pool.")]
    [SerializeField] private string poolName;

    public string PoolName
    { get { return poolName; } set { poolName = value; } }

    [Tooltip("The tags of this pool")]
    [SerializeField] private string[] tags;

    public string[] Tags
    { get { return tags; } set { tags = value; } }

    [Tooltip("The objects inside of this pool. Having multiple objects in one pool allows for more randomization of actually placed objects in the scene.")]
    [SerializeField] private GameObject[] objects;

    public GameObject[] Objects
    { get { return objects; } set { objects = value; } }

    [Tooltip("The amount og GameObjects that are instantiated from this pool.")]
    [SerializeField] private int loadAmount;

    public int LoadAmount
    { get { return loadAmount; } set { loadAmount = value; } }

    [Tooltip("Should the amount of active and placed GameObjects be random? If not the maxPlaceAmount value is taken as the placed amount.")]
    [SerializeField] private bool randomizeSpawnAmount;

    public bool RandomizeSpawnAmount
    { get { return randomizeSpawnAmount; } set { randomizeSpawnAmount = value; } }

    [Tooltip("If the amount of active and placed GameObjects should be random, how many should it be at minimum?")]
    [SerializeField] private int minPlaceAmount;

    public int MinPlaceAmount
    { get { return minPlaceAmount; } set { minPlaceAmount = value; } }

    [Tooltip("If the amount of active and placed GameObjects should be random, how many should it be at maximum?")]
    [SerializeField] private int maxPlaceAmount;

    public int MaxPlaceAmount
    { get { return maxPlaceAmount; } set { maxPlaceAmount = value; } }
}