using UnityEngine;

/// <summary>
/// Takes a duration at calculates the average fps during that time then displays it onto the screen. 
/// </summary>
public class FPSCounter : MonoBehaviour
{
    #region Variables 


    [Tooltip("The duration to calculate a average fps in")]
    [SerializeField] private float checkDuration = 1f; 

    /// <summary>
    /// The time until the average frame is displayed in the console. 
    /// </summary>
    private float totalTime = 0f;

    /// <summary>
    /// The elapsed frame count plus the current frame. 
    /// </summary>
    private int frameCount = 0;

    /// <summary>
    /// The current average fps. 
    /// </summary>
    private float averageFPS = 0f;

    #endregion Variables 

    #region Methods 

    private void Update()
    {
        FPSCalculations();
    }

    /// <summary>
    /// Draws a info text to the screen displaying the average fps 
    /// </summary>
    private void OnGUI()
    {
        // Display the average FPS on the screen
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 20;
        GUI.Label(new Rect(10, 10, 200, 50), "Average FPS: " + averageFPS.ToString("F2"), style);
    }

    /// <summary>
    /// Calculates the average fps in the time defined by <see cref="totalTime"/>.
    /// </summary>
    private void FPSCalculations()
    {
        totalTime += Time.deltaTime;
        frameCount++;

        // Calculate average FPS every second
        if (totalTime >= checkDuration)
        {
            averageFPS = frameCount / totalTime;
            frameCount = 0;
            totalTime = 0f;
        }
    }
    #endregion 
}