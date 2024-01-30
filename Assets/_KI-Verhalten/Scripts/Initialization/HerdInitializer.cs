using System.Collections;
using UnityEngine;

/// <summary>
/// On scene start this script waits until the world has been generated and then places all GameObjects
/// inside of the <see cref="herds"/> array at a random position.
/// </summary>
public class HerdInitializer : MonoBehaviour
{
    #region Variables

    [Tooltip("The GameObjects that should be placed at a random position.")]
    [SerializeField] private GameObject[] herds;

    [Tooltip("The range in which the herds will be placed, where x is the xaxis and y is the zaxis.")]
    [SerializeField] private Vector2 positionRange = new Vector2(100, 100);

    [Tooltip("The height at which the GameObjects will be positioned at.")]
    [SerializeField] private float height = 15;

    /// <summary>
    /// Have the herds been positioned?
    /// </summary>
    private bool positioned;

    // A required waiting time after all requirements are met, so that the program can be sure that no
    // inregularities occur.
    private const float waitBeforePlacingDuration = 15;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calls the <see cref="CheckIfPlacementShouldOccur"/> method in update to be sure that all requirements
    /// have been dealt with beforehand, in Awake and Start.
    /// </summary>
    private void Update()
    {
        if (SceneInitializer.Instance)
            CheckIfPlacementShouldOccur();
    }

    /// <summary>
    /// Only positions the herds if all other requirements have been met, so that no expections are thrown
    /// and the herds are placed in valid positions on the navmesh.
    /// </summary>
    private void CheckIfPlacementShouldOccur()
    {
        if (!positioned && SceneInitializer.Instance.WorldGenerated && !SceneInitializer.Instance.BisonsPlaced)
        {
            StartCoroutine(InitializeHerdCoroutine());
            positioned = true;
        }
    }

    /// <summary>
    /// Waits for a short duration and then places every object in the <see cref="herds"/> array at a random position
    /// inside of the terrain bounds.
    /// </summary>
    /// <returns></returns> A short duration so that the program can be sure that no inregularities occur.
    private IEnumerator InitializeHerdCoroutine()
    {
        yield return new WaitForSeconds(waitBeforePlacingDuration);

        foreach (GameObject go in herds)
        {
            go.transform.position = new Vector3(Random.Range(-positionRange.x, positionRange.x), height, Random.Range(-positionRange.y, positionRange.y));
            go.SetActive(true);
        }

        SceneInitializer.Instance.BisonsPlaced = true;
    }

    #endregion Methods
}