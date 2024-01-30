using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates a button on the <see cref="UpdatableData"/> scriptable object and its derived classes.
/// </summary>
[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    /// <summary>
    /// Gets called when the inspector is active and a <see cref="UpdateableData"> class is shown.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Call the base OnInspectorGUI so that the scriptable objects are shown correctly.
        base.OnInspectorGUI();

        // Store the current UpdateableData that is being inspected.
        UpdatableData data = (UpdatableData)target;

        // Call the NotifyOfUpdatedValues method if the button is pressed
        if (GUILayout.Button("Update"))
            data.NotifyOfUpdatedValues();
    }
}