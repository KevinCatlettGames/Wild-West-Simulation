using UnityEngine;

/// <summary>
/// Settings from this scriptable object are used for terrain generation.
/// Having a scriptable object hold these values allows for creation of differen noise values and with it terrain types.
/// </summary>
[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    #region Variables

    [Tooltip("Should the noise generation take the endless terrain system into account (Global), or not (Local).")]
    [SerializeField] private Noise.MinMaxConsideration minMaxConsideration;
    public Noise.MinMaxConsideration MinMaxConsideration
    { get { return minMaxConsideration; } }


    [Tooltip("How large the individuel pixels of the noise is.")]
    [SerializeField] private float noiseScale;
    public float NoiseScale
    { get { return noiseScale; } }


    [Tooltip("Should a random seed be generated?")]
    [SerializeField] private bool randomSeed;
    public bool RandomSeed
    { get { return randomSeed; } }


    [Tooltip("The generation will use this seed to generate random values, which allows for retaining a terrain by memorizing the seed.")]
    [SerializeField] private int seed;
    public int Seed
    { get { return seed; } set { seed = value; } }


    [Tooltip("How many octave layers should be used.")]
    [SerializeField] private int octaves;
    public int Octaves
    { get { return octaves; } }


    [Tooltip("Decreases the amplitude of the octaves.")]
    [Range(0, 1)]
    [SerializeField] private float persistance;
    public float Persistance
    { get { return persistance; } }


    [Tooltip("Increases the amplitude of the octaves.")]
    [SerializeField] private float lacunarity;
    public float Lacunarity
    { get { return lacunarity; } }


    [Tooltip("Manipulates how far apart octaves are from eachother.")]
    [SerializeField] private Vector2 offset;
    public Vector2 Offset
    { get { return offset; } }

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes sure the lacunarity and octave values are not under the minimum possible value.
    /// </summary>
    protected override void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;

        base.OnValidate();
    }

    #endregion Methods
}