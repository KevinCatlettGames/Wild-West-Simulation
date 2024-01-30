using UnityEngine;

/// <summary>
/// Constantly rotates a object in the given rotation.
/// </summary>
public class Rotating : MonoBehaviour
{
    [Tooltip("The rotation in which to rotate the GameObject.")]
    [SerializeField] private Vector3 rotation;

    private void Update()
    {
        transform.Rotate(rotation);
    }
}