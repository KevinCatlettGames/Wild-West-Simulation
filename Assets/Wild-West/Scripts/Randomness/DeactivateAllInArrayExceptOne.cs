using UnityEngine;

/// <summary>
/// Generates a random int value at deactivates all other objects in an array except for the
/// gameobject with that index.
/// </summary>
public class DeactivateAllInArrayExceptOne : MonoBehaviour
{
    [Tooltip("The objects in this array will be deactivated, except one at random.")]
    [SerializeField] private GameObject[] models;

    /// <summary>
    /// See class summary.
    /// </summary>
    private void Awake()
    {
        int randomIndex = Random.Range(0, models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            if (i != randomIndex)
                models[i].gameObject.SetActive(false);
        }
    }
}