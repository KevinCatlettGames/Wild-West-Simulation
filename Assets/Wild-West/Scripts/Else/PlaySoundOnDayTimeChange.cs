using UnityEngine;

/// <summary>
/// When the time of day changes, for example from day to night, a sound will be played.
/// This is done by subscribing to the <see cref="WorldTime.dayTimeChanged"/> event.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnDayTimeChange : MonoBehaviour
{
    #region Variables

    [Tooltip("The audioclip to play when the time changes.")]
    [SerializeField] private AudioClip audioC;

    /// <summary>
    /// The audioSource that should play a audioClip.
    /// </summary>
    private AudioSource audioS;

    #endregion Variables

    /// <summary>
    /// Makes sure this script gets initialized.
    /// </summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Unsubscribes to the <see cref="WorldTime.dayTimeChanged"/> event.
    /// </summary>
    private void OnDisable()
    {
        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged -= PlayClip;
    }

    /// <summary>
    /// Gets a reference to the audioSource and subscribes to the <see cref="WorldTime.dayTimeChanged"/> event.
    /// </summary>
    private void Initialize()
    {
        audioS = GetComponent<AudioSource>();
        audioS.PlayOneShot(audioC);

        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged += PlayClip;
    }

    /// <summary>
    /// Plays the clip.
    /// </summary>
    private void PlayClip()
    {
        audioS.PlayOneShot(audioC);
    }
}