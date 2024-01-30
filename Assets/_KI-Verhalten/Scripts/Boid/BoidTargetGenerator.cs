using UnityEngine;

/// <summary>
/// The boids listing in the <see cref="boids"/> array are given a new position to fly to, after a duration,
/// so that they fly around the entire map.
/// </summary>
public class BoidTargetGenerator : MonoBehaviour
{
    #region Variables

    [Tooltip("The duration until a new position is generated and set.")]
    [SerializeField] private float duration;

    [Tooltip("All boids that should be influenced by this script.")]
    [SerializeField] private BirdBoid[] boids;

    [Tooltip("The min and max positions the position will be generated in, where x is the x axis and y is the y axis.")]
    [SerializeField] private Vector2 minMaxPosition = new Vector2(175, 175);

    [Tooltip("The height the boids will fly in.")]
    [SerializeField] private float height = 100f;

    /// <summary>
    /// The duration when Start is called.
    /// </summary>
    private float initialDuration = 0;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Sets the <see cref="initialDuration"/> and calls the <see cref="SetTargetPositions"/> method once
    /// inordner to initialize a first random position.
    /// </summary>
    private void Start()
    {
        initialDuration = duration;
        SetTargetPositions();
    }

    /// <summary>
    /// Calls the <see cref="RandomPositionCountdown"/> method.
    /// </summary>
    private void Update()
    {
        RandomPositionCountdown();
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Counts down to zero and called the <see cref="SetTargetPositions"/> method when it has been reached
    /// inorder to set a new random position.
    /// </summary>
    private void RandomPositionCountdown()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            duration = initialDuration;
            SetTargetPositions();
        }
    }

    /// <summary>
    /// Generates a random position for the Boids to fly to.
    /// Calls the <see cref="BirdBoid.ChangeTargetPosition(Vector3)"/> method on each <see cref="BirdBoid"/>
    /// in the array to simulate a bird group flying that has the same goal.
    /// </summary>
    private void SetTargetPositions()
    {
        Vector3 newPosition = new Vector3(Random.Range(-minMaxPosition.x, minMaxPosition.x), height, Random.Range(-minMaxPosition.y, minMaxPosition.y));

        foreach (BirdBoid boid in boids)
        {
            boid.ChangeTargetPosition(newPosition);
        }
    }

    #endregion Methods
}