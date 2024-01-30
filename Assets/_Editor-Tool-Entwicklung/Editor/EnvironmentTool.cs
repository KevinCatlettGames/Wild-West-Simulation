using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles the skeleton of the EnvironmentTool. Creates the Editor Window and makes sure all necessary
/// calls are made to show the appropriate options by communicating with the <see cref="PrefabWindow"/> and <see cref="PoolWindow"/>
/// scripts to switch between Prefab creation and Pool creation.
///
/// The EnvironmentTool allows for quick Prefab and <see cref="Pool"/> creation, which are used inside of the
/// Object Pooling system.
/// </summary>
public class EnvironmentTool : EditorWindow
{
    #region Variables

    /// <summary>
    /// Singleton
    /// </summary>
    public static EnvironmentTool Instance;

    /// <summary>
    /// Const Window Values
    /// </summary>
    private const float MINPREFABWINDOWXSIZE = 600;
    private const float MINPREFABWINDOWYSIZE = 600;
    private const float MINSCRIPTABLEOBJECTXSIZE = 600;
    private const float MINSCRIPTABLEOBJECTYSIZE = 700;
    private const float SPACING = 20;

    /// <summary>
    /// LabelStyle
    /// </summary>
    private static GUIStyle labelStyle;

    private const int labelSize = 16;
    private const FontStyle labelFont = FontStyle.Bold;

    /// <summary>
    /// TextField Style
    /// </summary>
    private static GUIStyle textFieldStyle;

    private const int textFieldWidth = 205;

    /// <summary>
    /// Button Style
    /// </summary>
    private static GUIStyle buttonStyle;

    private const FontStyle buttonFont = FontStyle.Bold;
    private const int buttonSize = 16;
    private const int buttonHeight = 50;

    /// <summary>
    /// Has the window been initialized during this session?
    /// </summary>
    private static bool initialized;

    /// <summary>
    /// Currently active Tab
    /// </summary>
    private enum ActiveTab
    { PrefabTab, PoolTab }

    private ActiveTab activeTab = ActiveTab.PrefabTab;

    /// <summary>
    /// The different Windows / Tabs that can be openend and their logic.
    /// </summary>
    private static PrefabWindow prefabWindow;

    private const string prefabHeader = "Prefab Window";
    private const string prefabHelpText = "This is the create environment object prefab window. Here you can set the most important values for a environment object prefab and generate a prefab by pressing the generate button. Please make sure to fill out all the options first.";
    private static PoolWindow poolWindow;
    private const string poolHeader = "Pool Window";
    private const string poolHelpText = "This is the create environment object placement settings window. Here you can create a scriptable object entailing information about placing a objecttype in the world. Please make sure to fill out all the options first.";

    /// <summary>
    /// Handles generation of prefabs and storage inside of the Asset Folder.
    /// </summary>
    private PrefabGenerator prefabGenerator;

    #endregion Variables

    #region Window instancing

    /// <summary>
    /// When the EnvironmentTool is opened, this method handles instancing of the window
    /// and creation of the necessary window instances.
    /// </summary>
    [MenuItem("Tools/EnvironmentTool")]
    public static void ShowWindow()
    {
        // Create a singleton instance of this class.
        if (!Instance)
        {
            Instance = CreateWindow<EnvironmentTool>();
            Instance.titleContent = new GUIContent("Environment Tool");
        }

        // Display the EnvironmentTool window.
        Instance.Show();
        Instance.Focus();

        // Create instances of the different windows.
        if (!initialized)
        {
            if (prefabWindow == null)
                prefabWindow = new PrefabWindow(labelStyle, textFieldStyle, buttonStyle);

            if (poolWindow == null)
                poolWindow = new PoolWindow(labelStyle, textFieldStyle, buttonStyle);
        }
    }

    #endregion Window instancing

    #region Unity Methods

    /// <summary>
    /// Makes sure the GUIStyles are set when a instance of this class is active.
    /// </summary>
    private void OnEnable()
    {
        SetGUIStyles();
    }

    /// <summary>
    /// Makes sure there is no <see cref="PrefabGenerator"/> in the scene when this gets disabled.
    /// </summary>
    private void OnDisable()
    {
        DestroyPrefabGenerator();
    }

    /// <summary>
    /// Handles visualization inside of the window.
    /// </summary>
    private void OnGUI()
    {
        DrawToolbar();
        SetWindow();
    }

    #endregion Unity Methods

    #region Initialization

    /// <summary>
    /// Sets the styles of the individuell elements to their const values.
    /// </summary>
    private void SetGUIStyles()
    {
        //Labelstyle
        labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontSize = labelSize;
        labelStyle.fontStyle = labelFont;

        //Textfield Style
        textFieldStyle = new GUIStyle(EditorStyles.textField);
        textFieldStyle.fixedWidth = textFieldWidth;

        //Button Style
        buttonStyle = new GUIStyle(EditorStyles.miniButton);
        buttonStyle.fontSize = buttonSize;
        buttonStyle.fontStyle = buttonFont;
        buttonStyle.fixedHeight = buttonHeight;
    }

    /// <summary>
    /// Since a instance of the <see cref="PrefabGenerator"/> must be made when generating a prefab,
    /// it must be made sure that the instance is no longer in the scene once generation ends or the window
    /// gets closed.
    /// </summary>
    private void DestroyPrefabGenerator()
    {
        if (prefabGenerator != null)
        {
            DestroyImmediate(prefabGenerator.gameObject);
        }
    }

    #endregion Initialization

    #region Window Skeleton

    /// <summary>
    /// Draws two Tab options and makes sure the correct window / tab is currently being shown by setting the
    /// <see cref="ActiveTab"/> enum.
    /// </summary>
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Prefab Generator", EditorStyles.toolbarButton, GUILayout.Width(300)))
        {
            activeTab = ActiveTab.PrefabTab;
        }

        if (GUILayout.Button("Pool Generator", EditorStyles.toolbarButton, GUILayout.Width(300)))
        {
            activeTab = ActiveTab.PoolTab;
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Displays necessary elements on the top of the screen.
    /// Depending on the currently set <see cref="ActiveTab"/> value, the power is given to the <see cref="PrefabWindow"/> or <see cref="PoolWindow"/>.
    /// </summary>
    private void SetWindow()
    {
        DrawHeader();
        DrawHelpBox();
        switch (activeTab)
        {
            case ActiveTab.PrefabTab:
                Instance.minSize = new Vector2(MINPREFABWINDOWXSIZE, MINPREFABWINDOWYSIZE);
                prefabWindow.DrawWindow();
                break;

            case ActiveTab.PoolTab:
                Instance.minSize = new Vector2(MINSCRIPTABLEOBJECTXSIZE, MINSCRIPTABLEOBJECTYSIZE);
                poolWindow.DrawWindow();
                break;
        }
    }

    /// <summary>
    /// Depending on the currently set <see cref="ActiveTab"/> value, the header displays the name of the prefab generator
    /// or of the pool generator.
    /// </summary>
    private void DrawHeader()
    {
        switch (activeTab)
        {
            case ActiveTab.PrefabTab:
                GUILayout.Label(prefabHeader, labelStyle);
                break;

            case ActiveTab.PoolTab:
                GUILayout.Label(poolHeader, labelStyle);
                break;
        }
    }

    /// <summary>
    /// Depending on the currently set <see cref="ActiveTab"/> value, the helpbox shows a help message for the prefab generator
    /// or for the pool generator
    /// </summary>
    private void DrawHelpBox()
    {
        switch (activeTab)
        {
            case ActiveTab.PrefabTab:
                EditorGUILayout.HelpBox(prefabHelpText, MessageType.Info);
                EditorGUILayout.Space(SPACING);
                break;

            case ActiveTab.PoolTab:
                EditorGUILayout.HelpBox(poolHelpText, MessageType.Info);
                EditorGUILayout.Space(SPACING);
                break;
        }
    }

    #endregion Window Skeleton
}