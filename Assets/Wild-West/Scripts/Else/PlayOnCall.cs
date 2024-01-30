using UnityEngine;

/// <summary>
/// Plays a sound as soon as a particle is emitted from a Particle System. 
/// </summary>
public class PlayOnCall : MonoBehaviour
{
    #region Variables 

    [Tooltip("The AudioSource that should play audio.")]
    [SerializeField] private AudioSource audioS;

    [Tooltip("THe AudioClip that should be played.")]
    [SerializeField] private AudioClip audioC;

    /// <summary>
    /// Has the audio been played for this emission?
    /// </summary>
    private bool played;

    #endregion Variables 

    #region Methods 

    /// <summary>
    /// Makes sure the Sound plays once per emission once the emission takes place. 
    /// </summary>
    public void PlayAudio()
    {
        audioS.PlayOneShot(audioC);
    }

    #endregion Methods 
}