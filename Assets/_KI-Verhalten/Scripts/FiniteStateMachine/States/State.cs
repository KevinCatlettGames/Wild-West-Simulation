using System.Collections.Generic;

/// <summary>
/// The base class for all states. This class is abstract and cannot be instantiated.
/// </summary>
public abstract class State
{
    #region Variables

    /// <summary>
    /// The entity that is using this state.
    /// </summary>
    protected Entity entity;

    /// <summary>
    /// The transitions that will make this state transition to a new state.
    /// </summary>
    private List<Transition> transitions = new List<Transition>();
    public List<Transition> Transitions
    { get { return transitions; } set { transitions = value; } }

    #endregion Variables

    #region Constructor

    public State(Entity entity)
    {
        this.entity = entity;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Called when the state is entered for initialization of the state.
    /// </summary>
    public virtual void EnterState()
    {
    }

    /// <summary>
    /// Called when the state is entered for initialization of the state.
    /// Overloading the EnterState method to allow for a state to be passed in as the next state.
    /// </summary>
    /// <param name="nextState"></param>
    public virtual void EnterState(State nextState, string animationName)
    {
    }

    /// <summary>
    ///  Called every frame while the state is active.
    /// </summary>
    public virtual void UpdateState()
    {
    }

    /// <summary>
    /// Called every frame while the state is active.
    /// </summary>
    public virtual void CheckSwitchState()
    {
        foreach (var transition in transitions)
        {
            if (transition.transition.Invoke())
            {
                entity.StateMachine.SetState(transition.state);
                break;
            }
        }
    }

    /// <summary>
    /// Called when the state is exited for refreshing the state.
    /// </summary>
    public virtual void ExitState()
    {
    }

    #endregion Methods
}