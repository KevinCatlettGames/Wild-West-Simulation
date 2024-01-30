using System.Collections;
using UnityEngine;

/// <summary>
/// On scene start this script waits for a short duration and then activates the tumbleweeds gameobject.
/// </summary>
public class TumbleweedHandler : MonoBehaviour
{
    #region Variables

    [Tooltip("The gameObject that should be activated.")]
    [SerializeField] private GameObject tumbleweeds;

    /// <summary>
    /// The wait time until activation occurs.
    /// </summary>
    private const float activationWaitTime = 15f;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calls the coroutine at the start of the scene to make sure that activation occurs.
    /// </summary>
    private void Start()
    {
        if (SceneInitializer.Instance)
            StartCoroutine(InitializeTumbleweedsCoroutine());
    }

    /// <summary>
    /// Waits for a duration then activates the tumbleweeds. This stops exceptions from occuring and
    /// allows correct placement of NavmeshAgents on the navmesh.
    /// </summary>
    /// <returns></returns> The wait time until the object is activated.
    private IEnumerator InitializeTumbleweedsCoroutine()
    {
        yield return new WaitForSeconds(activationWaitTime);

        if (SceneInitializer.Instance.WorldGenerated)
            tumbleweeds.SetActive(true);
    }

    #endregion Methods
}