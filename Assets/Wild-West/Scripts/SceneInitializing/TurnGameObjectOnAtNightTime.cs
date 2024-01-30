using UnityEngine;

/// <summary>
/// Subscribes to a event that gets invoked when the daytime changes and toggles the active state of the given GameObject.
/// </summary>
public class TurnGameObjectOnAtNightTime : MonoBehaviour
{
    [Tooltip("The GameObject to manipulate.")]
    [SerializeField] private GameObject objectToActivate;

    #region Methods

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    private void Start()
    {
        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged += ToggleObject;
    }

    /// <summary>
    /// Unsubscribes to the event.
    /// </summary>
    private void OnDisable()
    {
        if (WorldTime.Instance)
            WorldTime.Instance.dayTimeChanged -= ToggleObject;
    }

    /// <summary>
    /// Toggles the active state of the GameObject.
    /// </summary>
    private void ToggleObject()
    {
        objectToActivate.SetActive(!objectToActivate.activeSelf);
    }

    #endregion Methods
}