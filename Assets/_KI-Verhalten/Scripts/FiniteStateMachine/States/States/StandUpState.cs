using UnityEngine;

/// <summary>
/// Plays a animation once and switches to a new state once <see cref="standUpTime"/> has been surpassed.
/// </summary>
public class StandUpState : State
{
    #region Variables

    /// <summary>
    /// How long the entity stays layed down before standing up.
    /// </summary>
    private float waitTimeUntilStandUp;

    /// <summary>
    /// How long the process of standing up takes.
    /// </summary>
    private float standUpTime;

    /// <summary>
    /// How long the entity has been in the process of standing up.
    /// </summary>
    private float currentStandUpTime;

    /// <summary>
    /// Should the entity start the stan up process?
    /// </summary>
    private bool doStandUp;

    /// <summary>
    /// Has the animation been played?
    /// </summary>
    private bool animationPlayed;

    /// <summary>
    /// The name of the animation that is played.
    /// </summary>
    private string animationName;

    #endregion Variables

    #region Constructor

    public StandUpState(Entity entity, float totalStandUpTime, string animationName) : base(entity)
    {
        this.animationName = animationName;
        this.standUpTime = totalStandUpTime;
    }

    #endregion Constructor

    #region State Methods

    public override void EnterState()
    {
        entity.Agent.isStopped = true;

        Initialize();
    }

    public override void UpdateState()
    {
        PerformStayAsleep();
        PerformStandUp();
    }

    #endregion State Methods

    #region Methods

    /// <summary>
    /// Sets the required variables to their initial state.
    /// </summary>
    private void Initialize()
    {
        currentStandUpTime = 0;
        waitTimeUntilStandUp = Random.Range(1, 4);

        doStandUp = false;
        animationPlayed = false;
    }

    /// <summary>
    /// Wait for a duration then changes the <see cref="doStandUp"/> boolean to true.
    /// </summary>
    private void PerformStayAsleep()
    {
        if (waitTimeUntilStandUp >= 0)
            waitTimeUntilStandUp -= Time.deltaTime;
        else if (!doStandUp)
            doStandUp = true;
    }

    /// <summary>
    /// Begins the stand up animation and waits for a duration then checks for state switching.
    /// </summary>
    private void PerformStandUp()
    {
        if (doStandUp)
        {
            if (!animationPlayed)
            {
                entity.EntityAnimator.Play(animationName);
                animationPlayed = true;
            }

            if (currentStandUpTime <= standUpTime)
                currentStandUpTime += Time.deltaTime;
            else
                CheckSwitchState();
        }
    }

    #endregion Methods
}