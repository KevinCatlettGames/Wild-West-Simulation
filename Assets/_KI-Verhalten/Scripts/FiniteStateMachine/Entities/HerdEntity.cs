using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entity that is part of a herd. This entity takes its target position from a targetPositionGenerator on daybreak, making it move to a
/// new destination every day.
/// </summary>
public class HerdEntity : Entity
{
    #region Variables

    #region States

    private State idleState;
    private State wanderState;
    private State sleepState;
    private State randomWalkState;
    private State layDownState;
    private State standUpState;

    #endregion States

    #region State Variables

    [Header("State Variables")]
    [Header("Wander State")]
    [Tooltip("The minimum distance the agent needs to be from the wander position for it to be considered as reached.")]
    [SerializeField] private float distanceWhenWanderPositionIsReached;

    [Header("Stand Up state")]
    [Tooltip("The time it takes for the agent to stand up.")]
    [SerializeField] private float standUpTransitionTime;

    [Header("Lay Down state")]
    [Tooltip("The time it takes for the agent to lay down.")]
    [SerializeField] private float layDownTransitionTime;

    [Header("Walk State")]
    [Tooltip("The time the agent should walk at maximum.")]
    [SerializeField] private float randomWalkTime;

    [Tooltip("The radius in which the agent should walk.")]
    [SerializeField] private float walkRadius;

    [Tooltip("The speed in which the child gameObject holding the mesh will rotate to face the direction its parent is facing.")]
    [SerializeField] private float meshRotationSpeed = 10f;

    #endregion State Variables

    #region Animation

    [Header("Animation Names")]
    [Tooltip("The name of the idle animation inside of the animator.")]
    [SerializeField] private string idleAnimationName;

    [Tooltip("The name of the wander animation inside of the animator.")]
    [SerializeField] private string wanderAnimationName;

    [Tooltip("The name of the stand up animation inside of the animator.")]
    [SerializeField] private string standUpAnimationName;

    [Tooltip("The name of the lay down animation inside of the animator.")]
    [SerializeField] private string layDownAnimationName;

    [Tooltip("The name of the walk animation inside of the animator.")]
    [SerializeField] private string randomWalkAnimationName;

    [Tooltip("The name of the sleep animation inside of the animator.")]
    [SerializeField] private string sleepAnimationName;

    #endregion Animation

    /// <summary>
    /// On daybreak this class creates a new position for a agent to take as wander position.
    /// </summary>
    private PositionGenerator targetPositionGenerator;

    #endregion Variables

    #region Unity Method

    /// <summary>
    /// Calls the <see cref="Initialize"/> method and the Start method on the derived <see cref="Entity"/> class.
    /// </summary>
    private new void Start()
    {
        Initialize();
        base.Start();
    }

    #endregion Unity Method

    #region Initialization

    /// <summary>
    /// Gets a reference to the <see cref="PositionGenerator"/> in the parent gameObject.
    /// Begins the state creation process.
    /// </summary>
    private void Initialize()
    {
        targetPositionGenerator = GetComponentInParent<PositionGenerator>();

        CreateStates();
        CreateTransitions();

        initialState = wanderState;
    }

    /// <summary>
    /// Creates an instance of each state this entity could potentially transition to.
    /// Gives their constructors the required variables.
    /// </summary>
    private void CreateStates()
    {
        idleState = new IdleState(this, idleAnimationName);
        wanderState = new WanderState(this, wanderAnimationName, targetPositionGenerator);
        sleepState = new SleepState(this, false, sleepAnimationName);
        standUpState = new StandUpState(this, standUpTransitionTime, standUpAnimationName);
        layDownState = new LayDownState(this, layDownTransitionTime, layDownAnimationName);
        randomWalkState = new RandomWalkState(this, randomWalkTime, walkRadius, wanderAnimationName);
    }

    /// <summary>
    /// Creates instances of the transition class and fills its List with possible <see cref="Transition"/>'s for each state.
    /// Then hands the created transitions to the appropriate states.
    /// </summary>
    private void CreateTransitions()
    {
        // IdleState transitions
        List<Transition> idleTransitions = new List<Transition>
        {
            new Transition(() => { return !worldTime.isDay; }, layDownState),
            new Transition(() => { return worldTime.isDay; }, randomWalkState)
        };
        idleState.Transitions = idleTransitions;

        // WanderState transitions
        List<Transition> wanderTransitions = new List<Transition>
        {
            new Transition(() => { return !worldTime.isDay || Vector3.Distance(transform.position, targetPositionGenerator.CurrentWanderPosition) < distanceWhenWanderPositionIsReached; }, randomWalkState)
        };
        wanderState.Transitions = wanderTransitions;

        // SleepState transitions
        List<Transition> sleepTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, standUpState)
        };
        sleepState.Transitions = sleepTransitions;

        // StandUpState transitions
        List<Transition> standUpTransitions = new List<Transition>
        {
            new Transition(() => { return true; }, wanderState)
        };
        standUpState.Transitions = standUpTransitions;

        // LayDownState transitions
        List<Transition> layDownTransitions = new List<Transition>
        {
            new Transition(() => { return true; }, sleepState)
        };
        layDownState.Transitions = layDownTransitions;

        // WalkState transitions
        List<Transition> randomWalkTransitions = new List<Transition>
        {
            new Transition(() => { return worldTime.isDay; }, idleState),
            new Transition(() => { return !worldTime.isDay; }, layDownState),
        };
        randomWalkState.Transitions = randomWalkTransitions;
    }

    #endregion Initialization
}