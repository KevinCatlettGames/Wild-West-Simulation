using UnityEngine;

/// <summary>
/// Plays a animation once and switches to a new state once <see cref="layDownTime"/> has been surpassed.
/// </summary>
public class LayDownState : State
{
    #region Variables

    /// <summary>
    /// How long the entity stands still before laying down.
    /// </summary>
    private float waitTimeUntilLayDown;

    /// <summary>
    /// How long the process of laying down takes.
    /// </summary>
    private float layDownTime;

    /// <summary>
    /// How long the entity has been in the process of laying down.
    /// </summary>
    private float currentLayDownTime;

    /// <summary>
    /// Should the entity start the lay down process?
    /// </summary>
    private bool doLayDown;

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

    public LayDownState(Entity entity, float totalLayDownTime, string animationName) : base(entity)
    {
        this.animationName = animationName;
        this.layDownTime = totalLayDownTime;
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
        PerformStandStill();
        PerformLayDown();
    }

    #endregion State Methods

    #region Methods

    /// <summary>
    /// Sets the required variables to their initial state.
    /// </summary>
    private void Initialize()
    {
        currentLayDownTime = 0;
        waitTimeUntilLayDown = Random.Range(1, 4);

        doLayDown = false;
        animationPlayed = false;
    }

    /// <summary>
    /// Waits for a duration then changes the <see cref="doLayDown"/> boolean to true.
    /// </summary>
    private void PerformStandStill()
    {
        if (waitTimeUntilLayDown >= 0)
            waitTimeUntilLayDown -= Time.deltaTime;
        else if (!doLayDown)
            doLayDown = true;
    }

    /// <summary>
    /// Begins the lay down animation and waits for a duration then checks for state switching.
    /// </summary>
    private void PerformLayDown()
    {
        if (doLayDown)
        {
            if (!animationPlayed)
            {
                entity.EntityAnimator.Play(animationName);
                animationPlayed = true;
            }

            if (currentLayDownTime <= layDownTime)
                currentLayDownTime += Time.deltaTime;
            else
                CheckSwitchState();
        }
    }

    #endregion Methods
}