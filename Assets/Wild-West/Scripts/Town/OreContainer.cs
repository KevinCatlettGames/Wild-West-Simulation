using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Saves a list with all orepatches and lets <see cref="TownEntity"/> AI find the closest or patch to use.
/// </summary>
public class OreContainer : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// This class is a singleton.
    /// </summary>
    public static OreContainer Instance;

    [Tooltip("The building that should be placed at an used ore patch.")]
    [SerializeField] private GameObject orePatchBuilding;

    [Tooltip("The minimum distance from town a used ore patch can be.")]
    [SerializeField] private float distanceFromTown = 150;

    /// <summary>
    /// A list of all ore patches.
    /// </summary>
    private List<GameObject> orePatches;

    public List<GameObject> OrePatches
    { get { return orePatches; } set { orePatches = value; } }

    /// <summary>
    /// A list of all patches that are being used by TownEntities.
    /// </summary>
    private List<GameObject> orePatchesWithBuilding;

    public List<GameObject> OrePatchesWithBuilding
    { get { return orePatchesWithBuilding; } set { orePatchesWithBuilding = value; } }

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes this into a singleton and initializes the lists.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        orePatches = new List<GameObject>();
        orePatchesWithBuilding = new List<GameObject>();
    }

    /// <summary>
    /// Adds a ore patch to the <see cref="orePatches"/> list.
    /// </summary>
    /// <param name="objectToAdd"></param>
    public void AddToOrePatches(GameObject objectToAdd)
    {
        orePatches.Add(objectToAdd);
    }

    /// <summary>
    /// Instantiates a ore patch building at a selected ore patch and marks this ore patch as being used by adding it
    /// to the <see cref="orePatchesWithBuilding"/> list.
    /// </summary>
    /// <param name="orePatch"></param>
    public void SpawnOrePatchBuilding(GameObject orePatch)
    {
        if (!orePatchesWithBuilding.Contains(orePatch))
        {
            Instantiate(orePatchBuilding, orePatch.transform.position, Quaternion.identity);
            orePatchesWithBuilding.Add(orePatch);
        }
    }

    /// <summary>
    /// Checks the ore patches and finds one closest to the given transform, that is valid.
    /// </summary>
    /// <param name="transformToCheckFrom"></param> The transform from where to check from.
    /// <returns></returns> The ore patch that has been selected to be used.
    public GameObject FindClosestOrePatch(Transform transformToCheckFrom)
    {
        GameObject closestOrePatch = null;
        // Iterate through all ore patches.
        for (int i = 0; i < orePatches.Count; i++)
        {
            if (i == 0)
                // Set the first ore patch.
                closestOrePatch = orePatches[i];
            else
            {
                // Check if the distance to the currently checked ore patch is closer than the currently selected closest.
                float distanceToCurrentClosest = Vector3.Distance(transformToCheckFrom.position, closestOrePatch.transform.position);
                float distanceToCurrent = Vector3.Distance(transformToCheckFrom.position, orePatches[i].transform.position);
                // Check if it is a valid ore patch.
                if (distanceToCurrent < distanceToCurrentClosest && distanceToCurrent > distanceFromTown)
                    closestOrePatch = orePatches[i];
            }
        }
        // After iteration set the selected ore patch to be the used one.
        SpawnOrePatchBuilding(closestOrePatch);
        return closestOrePatch;
    }

    #endregion Methods
}