using System;

/// <summary>
/// Holds a Func declaring that returns a boolean declaring if a <see cref="State"/> can transition or not.
/// </summary>
public struct Transition
{
    #region Variables

    /// <summary>
    /// The function declaring if a State can transition or not.
    /// </summary>
    public Func<bool> transition;

    /// <summary>
    /// The state that should be transitioned to, if the func returns true.
    /// </summary>
    public State state;

    #endregion Variables

    #region Constructor

    public Transition(Func<bool> transition, State state)
    {
        this.transition = transition;
        this.state = state;
    }

    #endregion Constructor
}