using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
/// <summary>
/// After the <see cref="PrefabValidator"/> has verified the input of the user this script creates a new instance of a prefab preset,
/// and saves all information from the <see cref="PrefabValues"/> instance to it.
/// </summary>
public class PrefabGenerator : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The GameObject being worked on that will be saved as a prefab.
    /// </summary>
    private GameObject prefab;

    /// <summary>
    /// The values that should be applied onto the prefab.
    /// </summary>
    private PrefabValues values;

    /// <summary>
    /// The layer the generated Prefab automatically is assigned to. 
    /// </summary>
    private int objectLayerIndex = 1;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Stores the users input since other methods need it and calls the other methods for prefab creation.
    /// </summary>
    /// <param name="values"></param>
    public void InitializePrefabCreation(PrefabValues values)
    {
        this.values = values;

        BuildPrefab();
        GenerateNewPrefab();
    }

    /// <summary>
    /// A new GamObject is instanced into the scene, the values are applied onto it, a prefab is generated from it and then the instanced object is destroyed.
    /// This is done because a prefab can only be saved from a instanced object.
    /// </summary>
    private void BuildPrefab()
    {
        // Create parent base object.
        prefab = new GameObject();

        // Set the name and its layer.
        prefab.name = values.Name;
        prefab.layer = objectLayerIndex;

        // Add a collider to the parent and set the trigger to true.
        BoxCollider parentCollider = prefab.AddComponent<BoxCollider>();
        parentCollider.isTrigger = true;

        // Add a rigidbody to the parent.
        Rigidbody rigidBody = prefab.AddComponent<Rigidbody>();
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        // Add a placement script and set its values, which allows for random placement around the scene.
        PlaceOnGround placeOnGround = prefab.AddComponent<PlaceOnGround>();
        placeOnGround.LayerMask = 1 << 6;
        placeOnGround.MinXPosition = -values.MaxDistanceFromCenter.x;
        placeOnGround.MaxXPosition = values.MaxDistanceFromCenter.x;
        placeOnGround.MinZPosition = -values.MaxDistanceFromCenter.y;
        placeOnGround.MaxZPosition = values.MaxDistanceFromCenter.y;

        // Add the PoolTag component.
        prefab.AddComponent<PoolTag>();

        // Add the randomizer component and set its values.
        if (values.RandomScale || values.RandomRotation)
        {
            RandomScaleAndRotation randomizer = prefab.AddComponent<RandomScaleAndRotation>();
            randomizer.RandomScale = values.RandomScale;
            randomizer.RandomRot = values.RandomRotation;
        }


        // Add the DisableOnTrigger component, that disables this gameObject if it hits a other object with the specified layer.
        DisableOnTrigger disableOnTrigger = prefab.AddComponent<DisableOnTrigger>();
        disableOnTrigger.LayerMask = 1;

        // Create child object and name it Mesh.
        GameObject childObject = new GameObject("Mesh");
        childObject.transform.parent = prefab.transform;

        // Add a MeshFilter component to the child object and set its Mesh value to the input of the user.
        MeshFilter meshFilter = childObject.AddComponent<MeshFilter>();
        meshFilter.mesh = values.Mesh;

        // Add a MeshRenderer component to the child object and set its Material value to the input of the user.
        MeshRenderer meshRenderer = childObject.AddComponent<MeshRenderer>();
        meshRenderer.material = values.Material;

        // Add a Collider to the child. This will make the child collider be as big as the mesh.
        BoxCollider childCollider = childObject.AddComponent<BoxCollider>();

        // Set the size of the parent collider so that the mesh fits inside of it.
        parentCollider.size = childCollider.size;

        // Since the parent collider is now set correctly corresponding to the Mesh, the child collider is deleted.
        DestroyImmediate(childCollider);
    }

    /// <summary>
    /// Saves the newly generated GameObject as a prefab in the folder specified by <<see cref="PathHolder"/> and deletes the prefab
    /// and this gameObject from the scene so the scene stays clean.
    /// </summary>
    private void GenerateNewPrefab()
    {
        // Save the transform's GameObject as a prefab asset.
        PrefabUtility.SaveAsPrefabAsset(prefab, PathHolder.ENVIRONMENTTOOLPREFABFOLDER + prefab.transform.name + ".prefab");

        // Destroy the objects in the scene that were created in order to make a new prefab.
        DestroyImmediate(prefab);
        DestroyImmediate(gameObject);
    }

    #endregion Methods
}
#endif 