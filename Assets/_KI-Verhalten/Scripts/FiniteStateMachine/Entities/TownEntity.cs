using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entity that is part of a town.. This entity collect gold during the day and sleeps at the town at night.
/// </summary>
public class TownEntity : Entity
{
    #region Variables

    #region States

    private State idleState;
    private State workState;
    private State sleepState;
    private State walkToOreState;
    private State walkToTownState;
    private State randomWalkState;

    #endregion States

    #region State Variables

    [Tooltip("The mesh renderer of this entity.")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Tooltip("The Pickaxe gameobject the worker townspeople should be holding, the other townspeoples pickaxe will be disabled.")]
    [SerializeField] private GameObject pickaxe;

    [Header("WalkToDestination State")]
    [Tooltip("How close should the entity be to its destination, for it to be reached.")]
    [SerializeField] private float destinationReachedDistance;

    [Header("RandomWalk State")]
    [Tooltip("The time the agent should walk at maximum.")]
    [SerializeField] private float maxWalkTime = 10f;

    [Tooltip("The radius in which the agent should walk.")]
    [SerializeField] private float maxWalkRadius = 50f;

    /// <summary>
    /// The closest ore path when this entity is first initialized and the ore patch this entity will go to during the day.
    /// </summary>
    private GameObject closestOrePatch;

    /// <summary>
    /// The position where this entity will go to at night to sleep.
    /// </summary>
    private Vector3 sleepPosition;

    #endregion State Variables

    #region Animation

    [Header("Animation Names")]
    [Tooltip("The name of the idle animation inside of the animator.")]
    [SerializeField] private string idleAnimationName;

    [Tooltip("The nameo of the work animation inside of the animator.")]
    [SerializeField] private string workAnimationName;

    [Tooltip("The name of the sleep animation inside of the animator.")]
    [SerializeField] private string sleepAnimationName;

    [Tooltip("The name of the walk animation inside of the animator.")]
    [SerializeField] private string walkAnimationName;

    #endregion Animation

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Call the <see cref="Initialize"/> method and the base class Start method.
    /// </summary>
    private new void Start()
    {
        Initialize();
        base.Start();
    }

    #endregion Unity Methods

    #region Initialization

    /// <summary>
    /// Gets the required references and calls the <see cref="CreateStates"/> and <see cref="DefineTownEntityType"/> methods.
    /// </summary>
    private void Initialize()
    {
        closestOrePatch = OreContainer.Instance.FindClosestOrePatch(transform);
        sleepPosition = transform.position;
        CreateStates();
        DefineTownEntityType();
    }

    /// <summary>
    /// Creates an instance of each state this entity could potentially transition to.
    /// Gives their constructors the required variables.
    /// </summary>
    private void CreateStates()
    {
        // Idle states
        idleState = new IdleState(this, idleAnimationName);
        workState = new IdleState(this, workAnimationName);

        // WalkToDestination states
        walkToOreState = new WalkToDestinationState(this, closestOrePatch.transform.position, walkAnimationName);
        walkToTownState = new WalkToDestinationState(this, sleepPosition, walkAnimationName);

        // Sleep state
        sleepState = new SleepState(this, true, sleepAnimationName);

        // RandomWalk state
        randomWalkState = new RandomWalkState(this, maxWalkTime, maxWalkRadius, walkAnimationName);
    }

    /// <summary>
    /// By defining different transitions, multiple Townpeople types can be randomly created.
    /// This method defines a random value which is used in a switch statement to call
    /// appropriate logic that either creates a worker or a explorer.
    /// </summary>
    private void DefineTownEntityType()
    {
        int randomValue = Random.Range(0, 2);
        switch (randomValue)
        {
            case 0: // Worker
                Vector3 workerOrePatchPosition = new Vector3(Random.Range(closestOrePatch.transform.position.x - 2, closestOrePatch.transform.position.x + 2), closestOrePatch.transform.position.y, UnityEngine.Random.Range(closestOrePatch.transform.position.z - 2, closestOrePatch.transform.position.z + 2));
                CreateWorkerTransitions(workerOrePatchPosition);
                pickaxe.SetActive(true);
                break;

            case 1: // Explorer
                CreateExplorerTransitions();
                break;

            default: // Default
                Vector3 defaultOrePatchPosition = new Vector3(Random.Range(closestOrePatch.transform.position.x - 2, closestOrePatch.transform.position.x + 2), closestOrePatch.transform.position.y, UnityEngine.Random.Range(closestOrePatch.transform.position.z - 2, closestOrePatch.transform.position.z + 2));
                CreateWorkerTransitions(defaultOrePatchPosition);
                pickaxe.SetActive(true);
                break;
        }
        initialState = randomWalkState;
    }

    /// <summary>
    /// Creates instances of the transition class and fills its List with possible <see cref="Transition"/>'s for each state.
    /// Then hands the created transitions to the appropriate states.
    /// Defines transitions for the worker townentity.
    /// </summary>
    private void CreateWorkerTransitions(Vector3 orePatchPosition)
    {
        // IdleState transitions
        List<Transition> idleTransitions = new List<Transition>
        {
            new Transition(() => { return !worldTime.isDay; }, sleepState),
            new Transition(() => { return worldTime.isDay; }, walkToOreState)
        };
        idleState.Transitions = idleTransitions;

        // WorkState transitions
        List<Transition> workTransitions = new List<Transition>
        {
            new Transition(() => { return !worldTime.isDay; }, randomWalkState)
        };
        workState.Transitions = workTransitions;

        // SleepState transitions
        List<Transition> sleepTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, idleState)
        };
        sleepState.Transitions = sleepTransitions;

        // WalkToOreState transitions
        List<Transition> walkToOreTransitions = new List<Transition>
        {
            new Transition(() => { return Vector3.Distance(transform.position, orePatchPosition) <= destinationReachedDistance; }, workState),
            new Transition(() => { return !worldTime.isDay; }, walkToTownState)
        };
        walkToOreState.Transitions = walkToOreTransitions;

        // WalkToTownState transitions
        List<Transition> walkToTownTransitions = new List<Transition>
        {
            new Transition(() => { return Vector3.Distance(transform.position, sleepPosition) <= destinationReachedDistance && !worldTime.isDay; }, sleepState)
        };
        walkToTownState.Transitions = walkToTownTransitions;

        // RandomWalkState transitions
        List<Transition> randomWalkTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay && agent.velocity.magnitude <= .1f; }, idleState),
            new Transition(() => { return !worldTime.isDay; }, walkToTownState),
        };
        randomWalkState.Transitions = randomWalkTransitions;
    }

    /// <summary>
    /// Creates instances of the transition class and fills its List with possible <see cref="Transition"/>'s for each state.
    /// Then hands the created transitions to the appropriate states.
    /// Defines transitions for the explorer townentity.
    /// </summary>
    private void CreateExplorerTransitions()
    {
        // IdleState transitions
        List<Transition> idleTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, randomWalkState),
            new Transition(() => { return !worldTime.isDay; }, walkToTownState)
        };
        idleState.Transitions = idleTransitions;

        // WalkToSaloonState transitions
        List<Transition> walkToTownTransitions = new List<Transition>
        {
            new Transition(() => { return Vector3.Distance(transform.position, sleepPosition) <= destinationReachedDistance && !worldTime.isDay; }, sleepState)
        };
        walkToTownState.Transitions = walkToTownTransitions;

        // RandomWalkState transitions
        List<Transition> randomWalkTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, idleState),
            new Transition(() => { return !worldTime.isDay; }, walkToTownState)
        };
        randomWalkState.Transitions = randomWalkTransitions;

        // SleepState transitions
        List<Transition> sleepTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, randomWalkState)
        };
        sleepState.Transitions = sleepTransitions;
    }

    #endregion Initialization
}