using System.Collections;
using UnityEngine;

/// <summary>
/// Makes sure the positions of all buildings are valid, and makes sure the town repositions itself
/// until this is the case.
/// </summary>
public class TownPlacementSystem : MonoBehaviour
{
    #region Variables

    [Tooltip("The buildings to take into account.")]
    [SerializeField] private TownBuildingPlacement[] townBuildingPlacements;

    [Tooltip("The range in which the town can be replaced in in the xAxis.")]
    [SerializeField] private float xPlacementRange;

    [Tooltip("The range in which the town can be replaced in in the zAxis.")]
    [SerializeField] private float yPlacementRange;

    /// <summary>
    /// The height this object is at when Awake is called. Used for when this is reset.
    /// </summary>
    private float yStartPosition;

    /// <summary>
    /// The number of positions that must be valid at once.
    /// </summary>
    private int numberOfNeededValidPositions;

    /// <summary>
    /// The number of positions that are valid while checking this time.
    /// </summary>
    private int validPositionCount;

    /// <summary>
    /// The number of positions that have been checked.
    /// </summary>
    private int buildingSpotsChecked;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes it so that the Town Placement can begin and reset if needed.
    /// </summary>
    private void Awake()
    {
        yStartPosition = transform.position.y;
        numberOfNeededValidPositions = townBuildingPlacements.Length;

        transform.position = new Vector3(Random.Range(-xPlacementRange, xPlacementRange), yStartPosition, Random.Range(-yPlacementRange, yPlacementRange));
    }

    /// <summary>
    /// Checks if enough buildings are placed to check if they are all valid.
    /// If so checks if the positions are are valid.
    /// If they are not valid, resets the Town Placement system.
    /// </summary>
    /// <param name="validPosition"></param> Wether the building that is currently calling this method is at a valid position.
    public void SpotChecking(bool validPosition)
    {
        // Add to the total building spots checked this round.
        buildingSpotsChecked++;

        // If the position is valid, add to the valid position counter.
        if (validPosition)
            validPositionCount++;

        // Check if the checked buildings amount is the same or more as the valid positions that are needed.
        if (buildingSpotsChecked >= numberOfNeededValidPositions)
        {
            buildingSpotsChecked = 0;
            // Check if all poitions are valid.
            if (validPositionCount == numberOfNeededValidPositions)
            {
                // If so initialize the town at the positions.
                StopCoroutine(BuildingResetCoroutine());
                SceneInitializer.Instance.TownPlaced = true;
            }
            else
                // Since the number of valid positions is not the same as the valid positions that are needed, retry the positioning.
                ResetSpotChecking();
        }
    }

    /// <summary>
    /// Reset all values so that a new position can be validated for the town.
    /// </summary>
    public void ResetSpotChecking()
    {
        transform.position = new Vector3(Random.Range(-xPlacementRange, xPlacementRange), transform.position.y, Random.Range(-yPlacementRange, yPlacementRange));
        validPositionCount = 0;
        StartCoroutine(BuildingResetCoroutine());
    }

    /// <summary>
    /// Retry placing each building at the newly positions spots.
    /// Wait a short period between placements so that the system has time to evaluate the information
    /// and the <see cref="SpotChecking"/> method is not called simultaniously.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BuildingResetCoroutine()
    {
        for (int i = 0; i < numberOfNeededValidPositions; i++)
        {
            yield return new WaitForSeconds(.00001f);
            townBuildingPlacements[i].RetrySpawn();
        }
    }

    #endregion Methods
}