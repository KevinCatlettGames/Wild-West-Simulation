using UnityEngine;

/// <summary>
/// Sets the agents destination to a Vector recieved by <see cref="PositionGenerator"/>.
/// Plays a animation
/// </summary>
public class WanderState : State
{
    #region Variables

    /// <summary>
    /// Generates a new position on the navmesh when a event is invoked (on daybreak).
    /// </summary>
    private PositionGenerator targetPositionGenerator;

    /// <summary>
    /// How fast the entity rotates itself to look in the moving direction. 
    /// </summary>
    private const float ROTATIONSPEED = .8f;

    /// <summary>
    /// How fast the entity rotates aligns itself to the ground normal.
    /// </summary>
    private const float ALIGNMENTSPEED = .7f;

    /// <summary>
    /// The name of the animation that will be played.
    /// </summary>
    private string animationName;

    #endregion Variables

    #region Constructor

    public WanderState(Entity entity, string animationName, PositionGenerator targetPositionGenerator) : base(entity)
    {
        this.targetPositionGenerator = targetPositionGenerator;
        this.animationName = animationName;
    }

    #endregion Constructor

    #region State Methods

    public override void EnterState()
    {
        Initialize();
    }

    public override void UpdateState()
    {
        AlignToGround();
        CheckSwitchState();

        // Debug.DrawLine(entity.transform.position, targetPositionGenerator.CurrentWanderPosition);
    }

    #endregion State Methods

    #region Methods

    /// <summary>
    /// Sets the required variables to their initial values.
    /// Plays a animation
    /// </summary>
    private void Initialize()
    {
        entity.Agent.SetDestination(targetPositionGenerator.CurrentWanderPosition);
        entity.Agent.isStopped = false;

        entity.EntityAnimator.Play(animationName);
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