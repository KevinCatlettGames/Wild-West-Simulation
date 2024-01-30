using UnityEngine;

/// <summary>
/// On entering the scene, this script add the gameObject it is on to the OreContainer list
/// of orepatches, so that <see cref="TownEntity"/> agents can used it as potential used ore patch.
/// </summary>
public class OrePatch : MonoBehaviour
{
    /// <summary>
    /// Adds this gameObject to the ore patches.
    /// </summary>
    private void Start()
    {
        if (OreContainer.Instance != null)
            OreContainer.Instance.AddToOrePatches(gameObject);
    }
}