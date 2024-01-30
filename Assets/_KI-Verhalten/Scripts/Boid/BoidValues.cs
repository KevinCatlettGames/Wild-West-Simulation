using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds values that manipulate the velocity in which the boids moves, changing its move direction and behaviour dynamically.
/// The higher the value, the stronger its pull.
/// </summary>
[CreateAssetMenu()]
public class BoidValues : ScriptableObject
{
    #region Variables

    [Tooltip("How strong the Boid will adjust its movement to match that of nearby neighbours.")]
    [SerializeField] private float alignmentValue;

    public float AlignmentValue
    { get { return alignmentValue; } }

    [Tooltip("How strong the Boid will adjust its movement to go to the center position of its neighbours.")]
    [SerializeField] private float cohesionValue;

    public float CohesionValue
    { get { return cohesionValue; } }

    [Tooltip("How strong the Boid will adjust its movement to avoid getting too close to its neighbours.")]
    [SerializeField] private float seperationValue;

    public float SeperationValue
    { get { return seperationValue; } }

    [Tooltip("How strong the Boid will adjust its movement to reach its target destination.")]
    [SerializeField] private float target;

    public float Target
    { get { return target; } }

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calculates the average alignment vector based on a list of neighbours velocities.
    /// </summary>
    /// <param name="neighbours"></param> The <see cref="IBoid"/>'s that are taken into account for the calculation.
    /// <returns></returns> The normalized average alignment of all neighbours or a zero vector if the neighbours list is empty.
    public Vector3 Alignment(List<IBoid> neighbours)
    {
        // If the list of neighbours is empty, return zero vector.
        if (neighbours.Count == 0) return Vector3.zero;

        // Initialize the alignment vector.
        Vector3 alignment = Vector3.zero;

        // Add the current neighbour's velocity to the alignment vector.
        for (int i = 0; i < neighbours.Count; i++)
        {
            alignment += neighbours[i].CurrentVelocity;
        }

        // Calculate the average alignment vector.
        alignment /= neighbours.Count;

        return alignment.normalized; // Return normalized!
    }

    /// <summary>
    /// Calculates the center vector by averaging the positions of neighbours.
    /// </summary>
    /// <param name="transform"></param> The transform of the <see cref="BirdBoid"/> that is calling this method.
    /// <param name="neighbours"></param> The <see cref="BirdBoid"/>'s that are taken into account for the calculation.
    /// <returns></returns> The normalized centerpoint of the neighbours, or a zero vector if the neighbours list is empty.
    public Vector3 Cohesion(Transform transform, List<IBoid> neighbours)
    {
        // If the list of neighbours is empty, return zero vector.
        if (neighbours.Count == 0) return Vector3.zero;

        // Initialize the cohesion vector.
        Vector3 center = Vector3.zero;

        // Calculate the vector from the neighbor's position to the current position
        // and add it to the center vector.
        for (int i = 0; i < neighbours.Count; i++)
        {
            center += (neighbours[i] as BirdBoid).transform.position - transform.position;
        }

        // Calculate the average center vector.
        center /= neighbours.Count;

        return center.normalized; // Return normalized!
    }

    /// <summary>
    /// Calculates the average direction vector by summing the difference vectors between the current position and
    /// the positions of the <see cref="BirdBoid"/> neighbours.
    /// </summary>
    /// <param name="transform"></param> The transform of the <see cref="BirdBoid"/> that is calling this method.
    /// <param name="neighbours"></param> The <see cref="BirdBoid"/>'s that are taken into account for the calculation.
    /// <returns></returns> The negation of the resulting vector, or a zero vector if the neighbours list is empty.
    public Vector3 Seperation(Transform transform, List<IBoid> neighbours)
    {
        // If the list of neighbours is empty, return the zero vector.
        if (neighbours.Count == 0) return Vector3.zero;

        // Initialize the vectors.
        Vector3 direction = Vector3.zero;
        Vector3 difference;

        // Loop through each neighbour.
        for (int i = 0; i < neighbours.Count; i++)
        {
            // Calculate the vector from the neighbour's position to the current position.
            difference = (neighbours[i] as BirdBoid).transform.position - transform.position;

            // Add the normalized difference vector divided by its squared magnitude to the direction vector.
            direction += difference / difference.sqrMagnitude;
        }

        // Calculate the average direction vector.
        direction /= neighbours.Count;

        return -direction; // Negation!
    }

    #endregion Methods
}