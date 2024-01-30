using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates a Button on the <see cref="MapGenerator"> class inside of the inspector and calls the <see cref="MapGenerator.DrawMapInEditor"/> method when it is pressed.
/// </summary>
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    /// <summary>
    /// Gets called when the inspector is active and the <see cref="MapGenerator"> class is shown.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Store the current MapGenerator that is being inspected.
        MapGenerator mapGen = (MapGenerator)target;

        // Make sure the Button can be drawn.
        if (DrawDefaultInspector())
        {
            // Update the map generator when a change occurs.
            if (mapGen.AutoUpdate)
                mapGen.DrawMapInEditor();
        }

        // Call the DrawMapInInspector mwethod if the button is pressed.
        if (GUILayout.Button("Generate"))
            mapGen.DrawMapInEditor();
    }
}