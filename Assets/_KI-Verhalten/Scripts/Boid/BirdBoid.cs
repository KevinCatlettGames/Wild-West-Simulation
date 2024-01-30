using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity that interacts with other Gameobjects of the same type to simulate flocking.
/// Stores the current movement information of the bird and handles group awareness.
/// </summary>
public class BirdBoid : MonoBehaviour, IBoid
{
    #region Variables

    [Tooltip("The Scriptable Object holding information like alignment, cohesion, seperation and target pull strength.")]
    [SerializeField] private BoidValues boidValues;

    /// <summary>
    /// All boids currently in the trigger radius of this gameObjects collider.
    /// </summary>
    public List<IBoid> neighbours;

    /// <summary>
    /// The total velocity this boid currently has.
    /// </summary>
    private Vector3 currentVelocity;

    public Vector3 CurrentVelocity
    { get { return currentVelocity; } set { currentVelocity = value; } }

    /// <summary>
    /// The target velocity which this boid will aim at.
    /// </summary>
    private Vector3 targetVelocity;

    /// <summary>
    /// The speed this boid moves through the world with.
    /// </summary>
    private float speed = 5;

    /// <summary>
    /// The position this boid constantly moves towards.
    /// </summary>
    private Vector3 targetPosition = Vector3.zero;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Initializes the <see cref="neighbours"/> list.
    /// </summary>
    private void Awake()
    {
        neighbours = new List<IBoid>();
    }

    /// <summary>
    /// Calls the <see cref="Move"/> method.
    /// </summary>
    private void Update()
    {
        Move();
    }

    #endregion Unity Methods

    #region Trigger Methods

    /// <summary>
    /// Adds the <see cref="BirdBoid"/> that enters the trigger to the <see cref="neighbours"/> list.
    /// </summary>
    /// <param name="other"></param> The collider that entered the trigger.
    private void OnTriggerEnter(Collider other)
    {
        BirdBoid boid = other.GetComponentInParent<BirdBoid>();
        if (boid != null)
        {
            neighbours.Add(boid);
        }
    }

    /// <summary>
    /// Removes the <see cref="BirdBoid"/> that exits the trigger from the <see cref="neighbours"/> list.
    /// </summary>
    /// <param name="other"></param> The collider that exited the trigger.
    private void OnTriggerExit(Collider other)
    {
        BirdBoid boid = other.GetComponentInParent<BirdBoid>();
        if (boid != null)
        {
            neighbours.Remove(boid);
        }
    }

    #endregion Trigger Methods

    #region Movement

    /// <summary>
    /// Adds the <see cref="BoidMovement"/> values together in update and uses it to control dynamic movement of this GameObject.
    /// </summary>
    private void Move()
    {
        // Add the strength in movement to the target to the targetVelocity.
        targetVelocity += (targetPosition - transform.position).normalized * boidValues.Target;

        // Add the alignment value to the targetVelocity.
        targetVelocity += boidValues.Alignment(neighbours) * boidValues.AlignmentValue;

        // Add the cohesion value to the targetVelocity.
        targetVelocity += boidValues.Cohesion(transform, neighbours) * boidValues.CohesionValue;

        // Add the seperation value to the targetVelocity.
        targetVelocity += boidValues.Seperation(transform, neighbours) * boidValues.SeperationValue;

        // Get the difference between the targetVelocity and the currentVelocity.
        Vector3 diff = targetVelocity - currentVelocity;

        // Slowly adjust the currentVelocity, taking the speed into account.
        currentVelocity += diff * Time.deltaTime;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, speed);

        // Apply the calculated new velocity onto the transform.
        transform.position = transform.position + currentVelocity * Time.deltaTime;
        transform.forward = currentVelocity;

        // Reset the targetVelocity so it is ready for the next Update.
        targetVelocity = Vector3.zero;
    }

    #endregion Movement

    #region Public Methods

    /// <summary>
    /// Makes the Boid target a new position.
    /// </summary>
    /// <param name="newPosition"></param> The new position this boid should target.
    public void ChangeTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    #endregion Public Methods
}