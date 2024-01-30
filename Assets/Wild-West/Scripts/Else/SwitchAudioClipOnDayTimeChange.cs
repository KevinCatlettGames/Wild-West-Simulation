using UnityEngine;

/// <summary>
/// When the <see cref="WorldTime.dayTimeChanged"/> event is invoked, the currently playing audioClip changes.
/// This can be used, for example, for switching music or ambient sound depending on the time of day.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SwitchAudioClipOnDayTimeChange : MonoBehaviour
{
    #region Variables

    [Tooltip("The audioClip played while it is day.")]
    [SerializeField] private AudioClip dayTimeClip;

    [Tooltip("The audioClip player while it is night.")]
    [SerializeField] private AudioClip nightTimeClip;

    /// <summary>
    /// The AudioSource that is playing the audioClips.
    /// </summary>
    private AudioSource audioS;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes sure the necessary elements are initialized.
    /// </summary>
    private void Start()
    {
        Initialization();
    }

    /// <summary>
    /// Gets a reference to the audioSource and subscribes to the <see cref="WorldTime.dayTimeChanged"/> event.
    /// </summary>
    private void Initialization()
    {
        audioS = GetComponent<AudioSource>();

        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged += ChangeClip;
    }

    /// <summary>
    /// Unsubscribes to the <see cref="WorldTime.dayTimeChanged"/> event.
    /// </summary>
    private void OnDisable()
    {
        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged -= ChangeClip;
    }

    /// <summary>
    /// When called, the currently playing audioClip is changed to the valid one depending on the current day time.
    /// </summary>
    private void ChangeClip()
    {
        if (WorldTime.Instance.isDay)
        {
            audioS.clip = dayTimeClip;
            audioS.Play();
        }
        else
        {
            audioS.clip = nightTimeClip;
            audioS.Play();
        }
    }

    #endregion Methods
}