using UnityEngine;

/// <summary>
/// Holds the input of the user and is given from the <see cref="PrefabWindow"/> class
/// to the <see cref="PrefabGenerator"/> class for creating a prefab with the values in this struct.
/// </summary>
public struct PrefabValues
{
    /// <summary>
    /// The name of the prefab.
    /// </summary>
    private string name;
    public string Name { get { return name; } set { name = value; } }


    /// <summary>
    /// The mesh on the MeshFilter of this prefab.
    /// </summary>
    private Mesh mesh;
    public Mesh Mesh { get { return mesh; } set { mesh = value; } }


    /// <summary>
    /// The material on the MeshRenderer of this prefab.
    /// </summary>
    private Material material;
    public Material Material { get { return material; } set { material = value; } }


    /// <summary>
    /// Should this prefab cast shadows?
    /// </summary>
    private bool castShadow;
    public bool CastShadow { get { return castShadow; } set { castShadow = value; } }


    /// <summary>
    /// Should the scale be randomized when the scene is generated?
    /// </summary>
    private bool randomScale;
    public bool RandomScale { get { return randomScale; } set { randomScale = value; } }

    /// <summary>
    /// Should rotation be randomized when the scene is generated?
    /// </summary>
    private bool randomRotation;
    public bool RandomRotation { get { return randomRotation; } set { randomRotation = value; } }


    /// <summary>
    /// The maximum distance in which this object will spawn from the center of the scene.
    /// </summary>
    private Vector2 maxDistanceFromCenter;
    public Vector2 MaxDistanceFromCenter { get { return maxDistanceFromCenter; } set { maxDistanceFromCenter = value; } }
}