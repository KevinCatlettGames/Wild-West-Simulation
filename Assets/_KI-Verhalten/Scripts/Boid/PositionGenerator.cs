using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// On Daybreak and at start this class finds a random position on the navmesh for entities to get and react on.
/// </summary>
public class PositionGenerator : MonoBehaviour
{
    #region Variables

    [Tooltip("The minimum position a target position should be searched for in the x and z axis.")]
    [SerializeField] private float minPosition;

    [Tooltip("The maximum position a target position should be searched for in the x and z axis.")]
    [SerializeField] private float maxPosition;

    [Tooltip("The distance from the last known position that the new position should be.")]
    [SerializeField] private float distanceFromOrigin;

    /// <summary>
    /// The position currently generated.
    /// </summary>
    private Vector3 currentWanderPosition;

    public Vector3 CurrentWanderPosition
    { get { return currentWanderPosition; } }

    /// <summary>
    /// The initial wait time before finding a new position for the first time.
    /// </summary>
    private const float initialWaitTime = 10f;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Subscribes to the dayTimeChanged event.
    /// </summary>
    private void Start()
    {
        WorldTime.Instance.dayTimeChanged += UpdateWanderPosition;
    }

    /// <summary>
    /// Unsubscribes from the <see cref="WorldTime.dayTimeChanged"/> event.
    /// </summary>
    private void OnDisable()
    {
        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged -= UpdateWanderPosition;
    }

    /// <summary>
    /// Gets called when the <see cref="WorldTime.dayTimeChanged"/> event is invoked and finds a new random position on the navmesh.
    /// </summary>
    private void UpdateWanderPosition()
    {
        if (WorldTime.Instance.isDay)
        {
            currentWanderPosition = new Vector3(Random.Range(minPosition, maxPosition), transform.position.y, Random.Range(minPosition, maxPosition));
            currentWanderPosition = RandomNavSphere(currentWanderPosition, distanceFromOrigin);
        }
    }

    /// <summary>
    /// Finds a position on the navmesh.
    /// </summary>
    /// <param name="origin"></param> The position from which to search a position from.
    /// <param name="dist"></param> The maximum distance to use as search radius.
    /// <param name="layermask"></param> The layermask with which to search for a navmesh position.
    /// <returns></returns> A position on the navmesh, if one is found. If not Vector3.zero is returned.
    private Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, -1);

        return navHit.position;
    }

    #endregion Methods
}