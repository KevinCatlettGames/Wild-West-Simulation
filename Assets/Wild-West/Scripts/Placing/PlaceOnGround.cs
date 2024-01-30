using UnityEngine;

/// <summary>
/// Finds a position on a navmesh by sending a raycast down towards it and does this until a
/// position is found. Then places the object at that position.
/// </summary>
public class PlaceOnGround : MonoBehaviour
{
    #region Variables

    [Tooltip("The layermask that should be searched for by the raycast as valid position.")]
    [SerializeField] protected LayerMask layerMask;

    public LayerMask LayerMask
    { get { return layerMask; } set { layerMask = value; } }

    [Tooltip("The minimum position in the xaxis.")]
    [SerializeField] protected float minXPosition = -250;

    public float MinXPosition
    { get { return minXPosition; } set { minXPosition = value; } }

    [Tooltip("The maximum position in the xaxis.")]
    [SerializeField] protected float maxXPosition = 250;

    public float MaxXPosition
    { get { return maxXPosition; } set { maxXPosition = value; } }

    [Tooltip("The minimum position in the z axis.")]
    [SerializeField] protected float minZPosition = -250;

    public float MinZPosition
    { get { return minZPosition; } set { minZPosition = value; } }

    [Tooltip("The maximum position in the z axis.")]
    [SerializeField] protected float maxZPosition = 250;

    public float MaxZPosition
    { get { return maxZPosition; } set { maxZPosition = value; } }

    [Tooltip("The minimum y position this object should have when placed.")]
    [SerializeField] protected float minYPosition = 0;

    [Tooltip("The maximum y position this object should have when placed")]
    [SerializeField] protected float maxYPosition = 20;

    [Tooltip("The Transform at which to start the transform from. If null this gameObjects transform is used.")]
    [SerializeField] protected Transform raycastStartTransform;

    [Tooltip("Should this Gameobjects rotation be changes so that its up vector is the same as the point at which the raycasthit occurs at?")]
    [SerializeField] protected bool alignToGround = true;

    [Tooltip("Is this object a town object?")]
    [SerializeField] private bool isTownObject;

    public bool IsTownObject
    { get { return isTownObject; } set { isTownObject = value; } }

    /// <summary>
    /// Has this object been placed?
    /// </summary>
    protected bool set;

    /// <summary>
    /// The current angle of the ground.
    /// </summary>
    protected float groundAngle;

    /// <summary>
    /// When placed, the object will take this value in seconds to align itself to the ground, then stop the aligning.
    /// </summary>
    protected float alignmentDuration = 2;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Perform the positioning on Start if this is a townobject. If not the object pool system will
    /// handle the calling of the positioning logic.
    /// </summary>
    private void Start()
    {
        if (isTownObject)
            RaycastAndPlace();
    }

    /// <summary>
    /// Handles alignment to ground if the alignment should still occur.
    /// </summary>
    private void Update()
    {
        if (alignmentDuration > 0)
            AlignToGround();
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Performs a raycast from the raycastStartPosition and checks if a object is hit and if the hit objects layer
    /// is in the layermask.
    /// </summary>
    public void RaycastAndPlace()
    {
        Vector3 raycastStartPosition;

        if (raycastStartTransform != null)
            raycastStartPosition = raycastStartTransform.position;
        else
            raycastStartPosition = transform.position;

        RaycastHit hit;

        // If true a object has been hit
        if (Physics.Raycast(raycastStartPosition, Vector3.down, out hit, 300, layerMask))
        {
            // If true the hit layer is entailed in the layermask
            if ((layerMask & (1 << hit.transform.gameObject.layer)) != 0)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                groundAngle = angle;

                if (transform.position.y < minYPosition && !isTownObject || transform.position.y > maxYPosition && !isTownObject)
                    RetryPlacement();
                else
                    set = true;
            }
            else
            {
                RetryPlacement();
            }
        }
        // If false no object has been hit and the process should happen again.
        else
            RetryPlacement();
    }

    /// <summary>
    /// Generates a random position and sets the transforms position to the newly found position.
    /// </summary
    public void FindRandomPosition()
    {
        float randomXPosition = Random.Range(minXPosition, maxXPosition);
        float randomZPosition = Random.Range(minZPosition, maxZPosition);
        Vector3 spawnPosition = new Vector3(randomXPosition, 100, randomZPosition);
        transform.position = spawnPosition;
    }

    /// <summary>
    /// Calls all methods needed to perform the placement process.
    /// </summary>
    private void RetryPlacement()
    {
        FindRandomPosition();
        RaycastAndPlace();
    }

    /// <summary>
    /// Gets the normal of a Raycast hit variable and rotates this gameObject so the up vector shows in the direction
    /// of the ground normal.
    /// </summary>
    private void AlignToGround()
    {
        if (alignToGround && alignmentDuration > 0)
        {
            alignmentDuration -= Time.deltaTime;

            Ray ray = new Ray(transform.position + Vector3.up, -transform.up * 5);
            RaycastHit info = new RaycastHit();
            if (Physics.Raycast(ray, out info, layerMask))
                transform.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
        }
    }

    #endregion Methods
}