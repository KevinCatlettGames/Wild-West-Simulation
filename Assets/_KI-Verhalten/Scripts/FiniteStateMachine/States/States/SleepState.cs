/// <summary>
/// Plays a animation on state entering and switches to a new state on daybreak(<see cref="WorldTime"/>).
/// </summary>
public class SleepState : State
{
    #region Variables

    /// <summary>
    /// Should the mesh be deactivated while sleeping?
    /// </summary>
    private bool deactivateMesh;

    /// <summary>
    /// The name of the animation that will be played.
    /// </summary>
    private string animationName;

    #endregion Variables

    #region Constructor

    public SleepState(Entity entity, bool deactivateMesh, string animationName) : base(entity)
    {
        this.deactivateMesh = deactivateMesh;
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
        CheckSwitchState();
    }

    public override void ExitState()
    {
        if (deactivateMesh)
        {
            entity.MeshTransform.gameObject.SetActive(true);
        }
        entity.SleepEffectGameobject.SetActive(false);
    }

    #endregion State Methods

    #region Methods

    /// <summary>
    /// Sets the required variables to their initial values.
    /// </summary>
    private void Initialize()
    {
        entity.Agent.isStopped = true;

        entity.EntityAnimator.Play(animationName);

        entity.SleepEffectGameobject.SetActive(true);
        if (deactivateMesh)
        {
            entity.MeshTransform.gameObject.SetActive(false);
        }
    }

    #endregion Methods
}