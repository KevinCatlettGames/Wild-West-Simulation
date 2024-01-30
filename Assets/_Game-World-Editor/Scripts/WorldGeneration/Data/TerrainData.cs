using UnityEngine;

/// <summary>
/// Settings from this scriptable object are used for terrain generation.
/// Having a scriptable object hold these values allows for creation of different terrain information and with it terrain types.
/// </summary>
[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    [Tooltip("Should the terrain be flat shaded? ")]
    [SerializeField] private bool useFlatShading;
    public bool UseFlatShading
    { get { return useFlatShading; } }

    [Tooltip("Should a falloff map be used? ")]
    [SerializeField] private bool useFalloff;
    public bool UseFalloff
    { get { return useFalloff; } }

    [Tooltip("Manipulates the height of the terrain by multiplying onto the height. ")]
    [SerializeField] private float meshHeightMultiplier;
    public float MeshHeightMultiplier
    { get { return meshHeightMultiplier; } }


    [Tooltip("Influences the look of the terrain, where the time declares at what point the influence should occur and the value declares how the terrain should form.")]
    [SerializeField] private AnimationCurve meshHeightCurve;
    public AnimationCurve MeshHeightCurve
    { get { return meshHeightCurve; } }


    [Tooltip("If the terrain uses a colormap to generate its color, this is used to calculate and fill the color array with colors for different regions of the terrain.")]
    [SerializeField] private Region[] regions;
    public Region[] Regions
    { get { return regions; } set { regions = value; } }
}