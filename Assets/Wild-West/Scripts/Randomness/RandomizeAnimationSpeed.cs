using UnityEngine;

/// <summary>
/// Generates a random float value and sets the referenced animators speed to the value on Awake.
/// </summary>
public class RandomizeAnimationSpeed : MonoBehaviour
{
    [Tooltip("The speed of this animator will be influenced randomly.")]
    [SerializeField] private Animator animator;

    [Tooltip("The range in which the animator speed can be where x is the minimum value and y is the maximum value.")]
    [SerializeField] private Vector2 animatorSpeed = Vector2.one;

    /// <summary>
    /// Generates a random float value and set the animators speed to it.
    /// </summary>
    private void Awake()
    {
        animator.speed = Random.Range(animatorSpeed.x, animatorSpeed.y);
    }
}