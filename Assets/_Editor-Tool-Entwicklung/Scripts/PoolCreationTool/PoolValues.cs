using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the input of the user and is given from the <see cref="PoolWindow"/> class
/// to the <see cref="PoolGenerator"/> class for creating a pool with the values in this struct.
/// </summary>
public struct PoolValues
{
    /// <summary>
    /// The name of the pool.
    /// </summary>
    private string name;

    public string Name { get { return name; } set { name = value; } }

    /// <summary>
    /// The tags in this pool.
    /// </summary>
    private List<string> tags;

    public List<string> Tags { get { return tags; } set { tags = value; } }

    /// <summary>
    /// The objects in this pool.
    /// </summary>
    private List<GameObject> objects;

    public List<GameObject> Objects { get { return objects; } set { objects = value; } }

    /// <summary>
    /// The amount of instanced objects.
    /// </summary>
    private int amount;

    public int Amount { get { return amount; } set { amount = value; } }

    /// <summary>
    /// Should the amount of spawned objects be random? If not the maxAmount is taken.
    /// </summary>
    private bool randomizeSpawnAmount;

    public bool RandomizeSpawnAmount { get { return randomizeSpawnAmount; } set { randomizeSpawnAmount = value; } }

    /// <summary>
    /// The minimum amount of objects that are spawned, if randomizeSpawnAmount is true.
    /// </summary>
    private int minAmount;

    public int MinAmount { get { return minAmount; } set { minAmount = value; } }

    /// <summary>
    /// The maximum amount of objects that are spawned, if randomizeSpawnAmount is true. If it is false, this value is taken as object spawn amount.
    /// </summary>
    private int maxAmount;

    public int MaxAmount { get { return maxAmount; } set { maxAmount = value; } }
}