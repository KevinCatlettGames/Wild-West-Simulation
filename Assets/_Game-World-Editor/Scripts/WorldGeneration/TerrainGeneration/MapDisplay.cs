using UnityEngine;

/// <summary>
/// Updates the texture and / or mesh, when its methods are called, to represent changes made on the values of the mesh, perlin noise map or colors.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    [Tooltip("The Texture Renderer that will display the newly made height or color map.")]
    [SerializeField] private Renderer textureRenderer;

    [Tooltip("The MeshFilter that will be used to hold the newly generated mesh.")]
    [SerializeField] private MeshFilter meshFilter;

    [Tooltip("The MeshRenderer that will display the newly generated mesh with its material.")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Tooltip("The collider component of the generated terrain. The collider bounds are regenerated at runtime.")]
    [SerializeField] private MeshCollider meshCollider;

    /// <summary>
    /// Sets the textureRenderers texture to the given one and updates the size of the rendered texture.
    /// </summary>
    /// <param name="texture"></param> The texture that should be represented.
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    /// <summary>
    /// Sets the mesh to the give one and applies a texture onto it.
    /// </summary>
    /// <param name="meshData"></param> The data of the mesh that should be displayed.
    /// <param name="texture"></param> The texture of the mesh that should be displayed.
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        if (meshFilter)
            meshFilter.sharedMesh = meshData.CreateMesh();

        if (meshRenderer)
            meshRenderer.sharedMaterial.mainTexture = texture;

        if (meshCollider)
            meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
}