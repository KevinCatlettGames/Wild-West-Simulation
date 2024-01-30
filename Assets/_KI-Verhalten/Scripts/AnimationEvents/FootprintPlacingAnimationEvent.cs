using UnityEngine;

/// <summary>
/// Methods used as animation events to invoke walk effects and sounds.
/// </summary>
public class FootprintPlacingAnimationEvent : MonoBehaviour
{
    #region Variables

    [Tooltip("The particles to emit from the left foot. Multiple for animals.")]
    [SerializeField] private ParticleSystem[] leftFootParticleSystems;

    [Tooltip("The particles to emit rom the right foot. Multiple for animals.")]
    [SerializeField] private ParticleSystem[] rightFootParticleSystems;

    [Tooltip("The audioSource that should play a walking sfx.")]
    [SerializeField] private AudioSource audioS;

    [Tooltip("The audioClips the audioSource should play, which of one will be picked at random for variation.")]
    [SerializeField] private AudioClip[] audioC;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Emits the left foot particle and plays a sound.
    /// </summary>
    public void EmitLeftFootPrintEffect()
    {
        foreach (ParticleSystem ps in leftFootParticleSystems)
            ps.Emit(10);

        int randomClipIndex = Random.Range(0, audioC.Length);
        audioS.PlayOneShot(audioC[randomClipIndex]);
    }

    /// <summary>
    /// Emits the right foot particle and plays a sound.
    /// </summary>
    public void EmitRightFootPrintEffect()
    {
        foreach (ParticleSystem ps in rightFootParticleSystems)
        {
            ps.Emit(10);
        }
        int randomClipIndex = Random.Range(0, audioC.Length);
        audioS.PlayOneShot(audioC[randomClipIndex]);
    }

    #endregion Methods
}