using UnityEngine;

/// <summary>
/// Sets a random wait time and plays a sound after that time has been reached.
/// Does this in a loop.
/// </summary>
public class MakeSoundAtRandomIntervals : MonoBehaviour
{
    #region Methods

    [Tooltip("The minimum wait time as x and maximum wait time as z.")]
    [SerializeField] private Vector2 intervalRange;

    [Tooltip("Only play the sound when it is day. Good, for example, for animals that should not make sounds at night.")]
    [SerializeField] private bool onlyDuringDayTime;

    [Tooltip("The audioSource that should play the audio.")]
    [SerializeField] private AudioSource audioS;

    [Tooltip("The audioClips that should be played. One of the clips are chosen at random for variation.")]
    [SerializeField] private AudioClip[] audioClips;

    /// <summary>
    /// The time until a audioClip is played.
    /// </summary>
    private float currentTime;

    #endregion Methods

    #region Methods

    /// <summary>
    /// Sets the random duration between audio at the beginning of the scene.
    /// </summary>
    private void Awake()
    {
        currentTime = Random.Range(intervalRange.x, intervalRange.y);
    }

    /// <summary>
    /// Calls the method to check if a audioclip should be played.
    /// </summary>
    private void Update()
    {
        CheckIfShouldPlay();
    }

    /// <summary>
    /// Checks if a audioclip should currently be played by this script.
    /// </summary>
    private void CheckIfShouldPlay()
    {
        if (onlyDuringDayTime && WorldTime.Instance)
            if (WorldTime.Instance.isDay)
                SoundPlaying();
            else if (!onlyDuringDayTime)
                SoundPlaying();
    }

    /// <summary>
    /// Counts down and plays a audioclip if the countdown has reached zero, then creates a new random wait time.
    /// </summary>
    private void SoundPlaying()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = Random.Range(intervalRange.x, intervalRange.y);

            int randomClipIndex = Random.Range(0, audioClips.Length);
            audioS.PlayOneShot(audioClips[randomClipIndex]);
        }
    }

    #endregion Methods
}