
using UnityEngine;

/// <summary>
/// A interface allowing external logic to change the target position of a boid. 
/// </summary>
public interface IBoid
{
    /// <summary>
    /// Makes the Boid target a new position.
    /// </summary>
    /// <param name="newPosition"></param> The new position this boid should target.
    public void ChangeTargetPosition(Vector3 newPosition);

    public Vector3 CurrentVelocity { get; set; }
}

