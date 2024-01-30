using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Finds a random position on the navmesh in a certain range from this entity and makes the entity walk to that position.
/// Switches state once the walk duration or the destination has been reached.
/// </summary>
public class RandomWalkState : State
{
    #region Variables

    /// <summary>
    /// Distance to destination when reached.
    /// </summary>
    private const float DistanceWhenDestinationReached = 2;

    /// <summary>
    /// The time that has surpassed while walking.
    /// </summary>
    private float currentWalkTime;

    /// <summary>
    /// The maximum time the entity can be walking.
    /// </summary>
    private float walkTime;

    /// <summary>
    /// The maximum radius in which a position to walk to is chosen.
    /// </summary>
    private float walkRadius;

    /// <summary>
    /// How fast the entity rotates itself to look in the moving direction. 
    /// </summary>
    private const float ROTATIONSPEED = 1f;

    /// <summary>
    /// How fast the entity rotates aligns itself to the ground normal.
    /// </summary>
    private const float ALIGNMENTSPEED = .7f;

    /// <summary>
    /// The position on the navmesh that the entity will walk to.
    /// </summary>
    private Vector3 walkPosition;

    /// <summary>
    /// The name of the animation that will be played while the entity walks.
    /// </summary>
    private string animationName;

    #endregion Variables

    #region Constructor

    public RandomWalkState(Entity entity, float totalRandomWalkTime, float totalRandomWalkRadius, string animationName) : base(entity)
    {
        walkTime = totalRandomWalkTime;
        walkRadius = totalRandomWalkRadius;
        this.animationName = animationName;
    }

    #endregion Constructor

    #region State Methods

    public override void EnterState()
    {
        if (entity.Agent.isOnNavMesh)
            entity.Agent.isStopped = false;

        Initialize();
    }

    public override void UpdateState()
    {
        AlignToGround();

        if (currentWalkTime < walkTime)
            currentWalkTime += Time.deltaTime;

        if (currentWalkTime >= walkTime || Vector3.Distance(entity.transform.position, walkPosition) <= DistanceWhenDestinationReached)
        {
            CheckSwitchState();
        }
    }

    #endregion State Methods

    #region Methods

    /// <summary>
    /// Sets the required variables to their initial values.
    /// Finds a position through a static <see cref="Utility"/> method.
    /// </summary>
    private void Initialize()
    {
        walkPosition = entity.RandomNavSphere(entity.transform.position, walkRadius, -1);
        if (entity.Agent.isOnNavMesh)
            entity.Agent.SetDestination(walkPosition);

        if (entity.EntityAnimator)
            entity.EntityAnimator.Play(animationName);

        currentWalkTime = 0;
    }

    /// <summary>
    /// Gets the ground normal from a static <see cref="Utility"/> method.
    /// Changes the eulerAngles of the entity so its upVector matches the ground normal.
    /// </summary>
    private void AlignToGround()
    {
        Ray ray = new Ray(entity.transform.position + Vector3.up, -entity.transform.up * 5);
        RaycastHit info = new RaycastHit();
        if (Physics.Raycast(ray, out info, entity.GroundLayer))
        {
            // entity.transform.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
            entity.transform.eulerAngles = Vector3.Lerp(entity.transform.rotation.eulerAngles, info.normal, ALIGNMENTSPEED * Time.deltaTime);

            if (entity.MeshTransform && entity.Agent.velocity.magnitude != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(entity.Agent.velocity.normalized);

                entity.MeshTransform.rotation = Quaternion.Slerp(entity.MeshTransform.rotation, targetRotation, ROTATIONSPEED * Time.deltaTime);
            }
        }
    }

    #endregion Methods
}