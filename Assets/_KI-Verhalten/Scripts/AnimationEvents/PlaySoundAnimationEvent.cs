using UnityEngine;

/// <summary>
/// Holds logic that can be used to play sound from animation events.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlaySoundAnimationEvent : MonoBehaviour
{
    [Tooltip("AudioClips that are played. One of the clips is chosen at random for variation.")]
    [SerializeField] private AudioClip[] audioClips;

    /// <summary>
    /// The audioSource that plays a clip.
    /// </summary>
    private AudioSource audioS;

    /// <summary>
    /// Gets a reference to the audioSource.
    /// </summary>
    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays a audioClip once, gets a random index
    /// </summary>
    public void PlaySound()
    {
        int randomClipIndex = Random.Range(0, audioClips.Length);
        audioS.PlayOneShot(audioClips[randomClipIndex]);
    }
}