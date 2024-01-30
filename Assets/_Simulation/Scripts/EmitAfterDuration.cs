using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes sure a specified Particle System Emit particles after a random interval.
/// </summary>
public class EmitAfterDuration : MonoBehaviour
{
    #region Variables 

    [Tooltip("The Particle System that should emit particles.")]
    [SerializeField] private ParticleSystem emitter;

    [Tooltip("The amount of particles that should be emitted.")]
    [SerializeField] private int emissionAmount = 10;

    [Tooltip("The min and max interval between emissions")]
    [SerializeField] private Vector2 minMaxInterval = new Vector2(10, 20);

    public UnityEvent OnEmission; 
    /// <summary>
    /// The current interval timer. 
    /// </summary>
    private float countDownTime;

    #endregion Variables

    #region Unity Methods 

    private void Update()
    {
        if (emitter == null) return;

        CountDown();
    }

    #endregion Unity Methods

    #region Methods 

    /// <summary>
    /// Counts down the timer until it reaches zero then triggers the emission and finds a new random interval.
    /// </summary>
    private void CountDown()
    {
        countDownTime -= Time.deltaTime;
        if (countDownTime <= 0)
        {
            countDownTime = Random.Range(minMaxInterval.x, minMaxInterval.y);
            Emit();
            OnEmission?.Invoke();
        }
    }

    /// <summary>
    /// Emits the emissionAmount from the emitter. 
    /// </summary>
    private void Emit()
    {
        emitter.Emit(emissionAmount);
    }

    #endregion Methods 
}
