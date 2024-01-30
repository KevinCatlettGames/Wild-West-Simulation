using System.IO;
using UnityEngine;

/// <summary>
/// Makes sure the input of the user is valid and returns boolean values verifying the input.
/// Methods are called by the <see cref="PrefabWindow"/> class.
/// </summary>
public static class PrefabValidator
{
    #region Variables

    /// <summary>
    /// The range in which a placeable object can be placed, based from the center.
    /// </summary>
    private static Vector2 placeRange = new Vector2(0, 175);
    public static Vector2 PlaceRange
    { get { return placeRange; } set { placeRange = value; } }

    #endregion Variables

    #region Methods

    /// <summary>
    /// The length of the name should not be zero and the file name should not exist yet.
    /// </summary>
    /// <param name="name"></param> The name that should be verified.
    /// <returns></returns> If the name is able to be used.
    public static bool ValidateName(string name)
    {
        // name <= 0
        if (string.IsNullOrEmpty(name)) return false;

        // prefab with name already exists at path
        if (File.Exists("Assets/Prefabs/EnvironmentDetails/" + name + ".prefab")) return false;
        return true;
    }

    /// <summary>
    /// The mesh should not be null.
    /// </summary>
    /// <param name="mesh"></param> The mesh that should be verified.
    /// <returns></returns> If the Mesh is able to be used.
    public static bool ValidateMesh(Mesh mesh)
    {
        // mesh != null
        if (mesh == null) return false;
        return true;
    }

    /// <summary>
    /// The material should not be null.
    /// </summary>
    /// <param name="material"></param> The material that should be verified.
    /// <returns></returns> If the Material is able to be used.
    public static bool ValidateMaterial(Material material)
    {
        // material != null
        if (material == null) return false;
        return true;
    }

    #endregion Methods
}