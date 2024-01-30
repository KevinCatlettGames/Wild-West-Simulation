using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This entity only uses the randomWalkState and is mainly in the scene for decoration.
/// </summary>
public class TumbleweedEntity : Entity
{
    #region Variables

    #region States

    private State randomWalkState;

    #endregion States

    [Header("Walk State")]
    [Tooltip("The time the agent should walk at maximum.")]
    [SerializeField] private float maxWalkTime;

    [Tooltip("The radius in which the agent should walk.")]
    [SerializeField] private float maxWalkRadius;

    #endregion Variables

    #region Animation

    [Header("Animation Names")]
    [Tooltip("The name of the walk animation inside of the animator.")]
    [SerializeField] private string randomWalkAnimationName;

    #endregion Animation

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
        CreateStates();
        CreateTransitions();
        initialState = randomWalkState;
    }

    /// <summary>
    /// Creates an instance of each state this entity could potentially transition to.
    /// Gives their constructors the required variables.
    /// </summary>
    private void CreateStates()
    {
        randomWalkState = new RandomWalkState(this, maxWalkTime, maxWalkRadius, null);
    }

    /// <summary>
    /// Creates instances of the transition class and fills its List with possible <see cref="Transition"/>'s for each state.
    /// Then hands the created transitions to the appropriate states.
    /// </summary>
    private void CreateTransitions()
    {
        // WalkState transition
        List<Transition> randomWalkTransitions = new List<Transition>
        {
            new Transition(() => { return true; }, randomWalkState),
        };
        randomWalkState.Transitions = randomWalkTransitions;
    }

    #endregion Initialization
}