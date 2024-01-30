using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The base class for all entities that holds the references that all entities have in common.
/// </summary>
public class Entity : MonoBehaviour
{
    #region Serialized Variables

    [Tooltip("The Transform that holds the mesh components.")]
    [SerializeField] protected Transform meshTransform;

    public Transform MeshTransform
    { get { return meshTransform; } }

    [Tooltip("The animator of the entity.")]
    [SerializeField] protected Animator entityAnimator;

    public Animator EntityAnimator
    { get { return entityAnimator; } }

    [Tooltip("The particle system that shows a sleeping effect.")]
    [SerializeField] protected GameObject sleepEffectGameobject;

    public GameObject SleepEffectGameobject
    { get { return sleepEffectGameobject; } }

    [Tooltip("The size of the mesh. X = Min value, Y = Max value.")]
    [SerializeField] protected Vector2 minMaxMeshSize;

    [Tooltip("The speed of the agent. X = Min value, Y = Max value.")]
    [SerializeField] protected Vector2 minMaxAgentSpeed;

    [Tooltip("The layermask of the ground navmesh.")]
    [SerializeField] protected LayerMask groundLayerMask;

    public LayerMask GroundLayer
    { get { return groundLayerMask; } }

    #endregion Serialized Variables

    #region Core References

    /// <summary>
    /// Manages and transitions from and to states.
    /// </summary>
    protected StateMachine stateMachine;

    public StateMachine StateMachine
    { get { return stateMachine; } }

    /// <summary>
    /// Manages the day and night cycle. The entity subscribes to an event to be informed when the current dayTime switches.
    /// </summary>
    protected WorldTime worldTime;

    public WorldTime GameTime
    { get { return worldTime; } }

    /// <summary>
    /// The NavMeshAgent on this gameObject.
    /// </summary>
    protected NavMeshAgent agent;

    public NavMeshAgent Agent
    { get { return agent; } }

    /// <summary>
    /// The state that this entity initially begins in.
    /// </summary>
    protected State initialState;

    #endregion Core References

    #region Unity Methods

    /// <summary>
    /// Calls the <see cref="Initialization"/> method.
    /// </summary>
    protected void Start()
    {
        Initialization();
    }

    /// <summary>
    /// Updates the state machine every frame.
    /// </summary>
    private void Update()
    {
        stateMachine.Update();
    }

    #endregion Unity Methods

    #region Initialization

    /// <summary>
    /// For all entities to function the <see cref="agent"/>, <see cref="worldTime"/> and <see cref="stateMachine"/>
    /// must be referenced or instanced.
    /// The speed and scale of the entity is also randomly set to simulate difference between entities.
    /// </summary>
    private void Initialization()
    {
        // Agent referencing and speed changing.
        if (GetComponent<NavMeshAgent>())
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = Random.Range(minMaxAgentSpeed.x, minMaxAgentSpeed.y);
        }

        // Scale changing.
        if (meshTransform)
        {
            float scaleAmount = Random.Range(minMaxMeshSize.x, minMaxMeshSize.y);
            meshTransform.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
        }

        // WorldTime referencing.
        if (WorldTime.Instance)
            worldTime = WorldTime.Instance;

        // State machine instancing.
        stateMachine = new StateMachine(initialState);
    }

    /// <summary>
    /// Finds a position on the navmesh.
    /// </summary>
    /// <param name="origin"></param> The position from which to search a position from.
    /// <param name="dist"></param> The maximum distance to use as search radius.
    /// <param name="layermask"></param> The layermask with which to search for a navmesh position.
    /// <returns></returns> A position on the navmesh, if one is found. If not Vector3.zero is returned.
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    #endregion Initialization
}