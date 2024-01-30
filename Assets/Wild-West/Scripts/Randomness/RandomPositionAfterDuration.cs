using UnityEngine;

/// <summary>
/// Finds a new position inside of given values at sets this transforms position to that position.
/// </summary>
public class RandomPositionAfterDuration : MonoBehaviour
{
    #region Variables

    [Tooltip("The duration until a new position is found.")]
    [SerializeField] private float waitTime = 20f;

    [Tooltip("The minimum and maximum x value where a position is found.")]
    [SerializeField] private float minMaxXValue;

    [Tooltip("The minimum and maximum z value where a position is found.")]
    [SerializeField] private float minMaxZValue;

    /// <summary>
    /// The wait time that is being counted down.
    /// </summary>
    private float currentWaitTime;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Initializes the countdown values and find a first random position.
    /// </summary>
    private void Start()
    {
        currentWaitTime = waitTime;
        FindRandomPosition();
    }

    /// <summary>
    /// Handles the counting down and finds a random position if the countdown reaches zero.
    /// </summary>
    private void Update()
    {
        currentWaitTime -= Time.deltaTime;
        if (currentWaitTime <= 0)
        {
            currentWaitTime = waitTime;
            FindRandomPosition();
        }
    }

    /// <summary>
    /// Generates random values for the position and sets the transforms position to those values.
    /// </summary>
    private void FindRandomPosition()
    {
        float xValue = Random.Range(-minMaxXValue, minMaxXValue);
        float zValue = Random.Range(-minMaxZValue, minMaxZValue);
        Vector3 randomPosition = new Vector3(xValue, transform.position.y, zValue);
        transform.position = randomPosition;
    }

    #endregion Methods
}