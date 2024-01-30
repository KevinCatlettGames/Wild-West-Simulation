/// <summary>
/// Handles the current state of an entity and its transitions.
/// </summary>
public class StateMachine
{
    #region Variable

    /// <summary>
    /// The state currently active in the state machine.
    /// </summary>
    private State currentState;

    #endregion Variable

    #region Constructor

    public StateMachine(State firstState)
    {
        currentState = firstState;
        currentState.EnterState();
    }

    #endregion Constructor

    #region Methods

    public void Update()
    {
        currentState.UpdateState();
    }

    /// <summary>
    /// Sets the current state to the new state.
    /// </summary>
    /// <param name="newState"></param> The state to transition to.
    public void SetState(State newState)
    {
        if (newState == null) return;
        else
        {
            currentState.ExitState(); // Exit the current state.
            newState.EnterState(); // Enter the new state.
            currentState = newState; // Set the current state to the new state.
        }
    }

    #endregion Methods
}