using System.Collections;
using UnityEngine;

/// <summary>
/// Activates all GameObjects inside of an array after a duration.
/// </summary>
public class ObjectsActivation : MonoBehaviour
{
    #region Variables

    [Tooltip("The objects that are deactivated at the beginning but get activated through this script.")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Tooltip("The duration until activation.")]
    [SerializeField] private float duration = 15f;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calls a coroutine when enabled.
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(WaitAndActivateCoroutine());
    }

    /// <summary>
    /// Waits for a duration then activates every gameobject inside of the array.
    /// </summary>
    /// <returns></returns> The duration to wait until activation.
    private IEnumerator WaitAndActivateCoroutine()
    {
        yield return new WaitForSeconds(duration);
        foreach (GameObject obj in objectsToActivate)
            obj.SetActive(true);
    }

    #endregion Methods
}