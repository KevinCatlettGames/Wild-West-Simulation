using UnityEngine;

/// <summary>
/// One of the GameObjects inside of the array is chosen and activated randomly.
/// </summary>
public class ActivateOnlyOneFromArrayRandomly : MonoBehaviour
{
    [Tooltip("A array of GameObjects of which to pick one of.")]
    [SerializeField] private GameObject[] objects;

    /// <summary>
    /// Generates a random int and activates the gameobject inside of the objects array with the corresponding index.
    /// </summary>
    private void Awake()
    {
        int randomIndex = Random.Range(0, objects.Length);
        objects[randomIndex].SetActive(true);
    }
}