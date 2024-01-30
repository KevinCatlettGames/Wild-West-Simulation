using UnityEngine;

/// <summary>
/// Deriving from the PlaceOnGround class, this class makes sure the building this is attached
/// to is placed correctly for a town building, taking the angle of the ground into account.
/// </summary>
public class TownBuildingPlacement : PlaceOnGround
{
    #region Variables

    [Tooltip("The class that makes sure that all buildings are at a valid position before placing the town.")]
    [SerializeField] private TownPlacementSystem townPlacementSystem;

    [Tooltip("The maxing angle allowed for the ground to be so that the current position is valid.")]
    [SerializeField] private float maxAngle;

    /// <summary>
    /// Has the current position bee checked.
    /// </summary>
    private bool checkedPosition;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calls the CheckGround method.
    /// </summary>
    private void Update()
    {
        CheckGround();
    }

    /// <summary>
    /// Makes sure the ground angle is below the maxAngle and building is placed high enough.
    /// </summary>
    private void CheckGround()
    {
        if (set && !checkedPosition)
        {
            if (groundAngle < maxAngle && transform.position.y > minYPosition)
            {
                townPlacementSystem.SpotChecking(true);
            }
            else
            {
                townPlacementSystem.SpotChecking(false);
            }

            checkedPosition = true;
        }
    }

    /// <summary>
    /// Reverts the necessary values so that repositioning can occur and restarts the process.
    /// </summary>
    public void RetrySpawn()
    {
        transform.position = new Vector3(transform.position.x, transform.parent.position.y, transform.position.z);
        set = false;
        checkedPosition = false;
        RaycastAndPlace();
    }

    #endregion Methods
}