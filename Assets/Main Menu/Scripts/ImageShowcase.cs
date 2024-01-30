using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loops through an array of images and displays one after the other. 
/// </summary>
public class ImageShowcase : MonoBehaviour
{
    #region Variables 

    [Tooltip("The Image component to show the sprite on.")]
    [SerializeField] private Image image;

    [Tooltip("The array of sprites that should be looped through.")]
    [SerializeField] private Sprite[] imagesToShow;

    [Tooltip("How long at maximum until a new sprite is shown.")]
    [SerializeField] private float imageChangeDuration = 5f;

    /// <summary>
    /// The index of the currently shown image.
    /// </summary>
    private int currentImageIndex = 0;

    /// <summary>
    /// How much time is left until the next sprite is shown. 
    /// </summary>
    private float currentCountdown = 0;

    #endregion

    #region Unity Methods 

    private void Awake()
    {
        currentCountdown = imageChangeDuration; 
    }

    private void Update()
    {
        CountDown();    
    }

    #endregion Unity Methods

    #region Methods 
    /// <summary>
    /// Counts down and calls the Swap method when the timer is at zero. 
    /// </summary>
    private void CountDown()
    {
        currentCountdown -= Time.deltaTime;
        if (currentCountdown <= 0)
        {
            currentCountdown = imageChangeDuration;
            SwapImage();
        }
    }

    /// <summary>
    /// Swaps the image for the next one. If the last image in the array is being shown, it will loop to the beginning.
    /// </summary>
    private void SwapImage()
    {
        for(int i = 0; i < imagesToShow.Length; i++)
        {
            if(currentImageIndex == i)
            {
                if(currentImageIndex == imagesToShow.Length)
                    currentImageIndex = 0;
                else
                currentImageIndex = i++;

                image.sprite = imagesToShow[currentImageIndex];
                break;
            }
        }
    }

    #endregion 
}
