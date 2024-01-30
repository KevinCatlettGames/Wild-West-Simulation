using UnityEngine;

/// <summary>
/// A scriptable object base class that allows validation inside of the unity editor and invokes an action once validation has occured.
/// </summary>
public class UpdatableData : ScriptableObject
{
    #region Variables

    /// <summary>
    /// The action that is invoked once validation occurs.
    /// </summary>
    public event System.Action OnValuesUpdated;

    /// <summary>
    /// Should this scriptable object automatically use its changes and update other scripts when validation occurs?
    /// </summary>
    public bool autoUpdate;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Makes sure the Action is invoked if it should be.
    /// </summary>
    protected virtual void OnValidate()
    {
        if (autoUpdate)
            NotifyOfUpdatedValues();
    }

    /// <summary>
    /// Invokes the Action.
    /// </summary>
    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
            OnValuesUpdated();
    }

    #endregion Methods
}