using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object combining <see cref="NoiseData"/> and <see cref="TerrainData"/> together and declare what <see cref="Pool"/>'s of GameObject should exist in this world type.
/// This allows making a presets for worldtypes, from different biomes with uniquely placed objects to different kinds of landmasses / terrain.
/// </summary>
[CreateAssetMenu()]
public class BiomeData : ScriptableObject
{
    [Tooltip("The NoisData this world type should use to generate the noisemap the terrain uses.")]
    [SerializeField] private NoiseData noiseData;

    public NoiseData NoiseData
    { get { return noiseData; } set { noiseData = value; } }

    [Tooltip("The TerrainData this world type should use to generate the terrain.")]
    [SerializeField] private TerrainData terrainData;

    public TerrainData TerrainData
    { get { return terrainData; } set { terrainData = value; } }

    [Tooltip("The Pool scriptable objects this world type should use to generate placed GameObjects for the environment.")]
    [SerializeField] private List<Pool> pools;

    public List<Pool> Pools
    { get { return pools; } }
}