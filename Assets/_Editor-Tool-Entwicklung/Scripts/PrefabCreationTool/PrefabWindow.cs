using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
/// <summary>
/// The PrefabWindow handles displaying the elements of the PrefabTab in the EnvironmentTool.
/// Once all is set and the Generate Button is pressed, this script handles calls for user input validation by the <see cref="PrefabValidator"/>
/// and generating a new Prefab by the <see cref="prefabGenerator"/>.
/// </summary>
public class PrefabWindow
{
    #region Variables

    /// <summary>
    /// The spacing between elements.
    /// </summary>
    private const float SPACING = 20;

    /// <summary>
    /// The maximum length limit of the name created by the user.
    /// </summary>
    private const int NAMELIMIT = 20;

    /// <summary>
    /// The currently set name of the pool.
    /// </summary>
    private string name = "NewEnvironmentObject";

    /// <summary>
    /// The Mesh chosen by the user.
    /// </summary>
    private Mesh mesh;

    /// <summary>
    /// The Material chosen by the user.
    /// </summary>
    private Material material;

    /// <summary>
    /// Should the object cast a shadow?
    /// </summary>
    private bool shadowCastOption = true;

    /// <summary>
    /// Should the objects scale be random?
    /// </summary>
    private bool randomScale;

    /// <summary>
    /// Should the objects rotation be random?
    /// </summary>
    private bool randomRotation;

    /// <summary>
    /// The x maximum range from the center.
    /// </summary>
    private float xMaxFromCenter = PrefabValidator.PlaceRange.y * 0.5f;

    /// <summary>
    /// The z maximum range from the center.
    /// </summary>
    private float zMaxFromCenter = PrefabValidator.PlaceRange.y * 0.5f;

    /// <summary>
    /// The GUIStyles for the different elements of the window.
    /// </summary>
    private GUIStyle labelStyle;

    private GUIStyle textFieldStyle;
    private GUIStyle buttonStyle;

    /// <summary>
    /// The error text that is currently being displayed.
    /// </summary>
    private string errorText = "";

    private const string nameErrorText = "The name is too short, over twenty characters long, or already exists, please pick a valid name.";
    private const string meshErrorText = "The mesh can not be empty, please select a mesh to use";
    private const string materialErrorText = "The material can not be empty, please select a material to use";

    /// <summary>
    /// The class that generates a prefab that incorporates the users input.
    /// </summary>
    private PrefabGenerator prefabGenerator;

    #endregion Variables

    #region Constructor

    public PrefabWindow(GUIStyle labelStyle, GUIStyle textFieldStyle, GUIStyle buttonStyle)
    {
        this.labelStyle = labelStyle;
        this.textFieldStyle = textFieldStyle;
        this.buttonStyle = buttonStyle;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Calls all Draw Methods in the order so that the elements are displayed after each other.
    /// </summary>
    public void DrawWindow()
    {
        GUILayout.Space(SPACING);
        DrawGeneralOptions();
        DrawToggleOptions();
        DrawPlacementOptions();
        DrawGenerateButton();
        DrawErrorBox();
    }

    /// <summary>
    ///Displays the Name, Mesh and Material label and the field in which the user can input his choices.
    /// </summary>
    private void DrawGeneralOptions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Prefab Name:", labelStyle);
        GUILayout.FlexibleSpace();

        // Make sure the shown name does not exceed the name limit.
        if (name.Length > NAMELIMIT)
        {
            string modifiedName = name.Substring(0, name.Length - 1);
            name = modifiedName;
        }
        name = GUILayout.TextField(name, textFieldStyle);

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mesh:", labelStyle);
        GUILayout.FlexibleSpace();

        // Display the Mesh objectfield.
        mesh = EditorGUILayout.ObjectField(mesh, typeof(Mesh), false, GUILayout.Width(205)) as Mesh;

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Material:", labelStyle);
        GUILayout.FlexibleSpace();

        // Display the Material objectfield.
        material = EditorGUILayout.ObjectField(material, typeof(Material), false, GUILayout.Width(205)) as Material;

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
    }

    /// <summary>
    /// Displays the toggles with which the user can input if the object should cast shadows and have a random scale and rotation.
    /// </summary>
    private void DrawToggleOptions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Shadow casting:", labelStyle);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 5);

        // Display shadow cast toggle.
        shadowCastOption = EditorGUILayout.Toggle("", shadowCastOption);

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable random scale", labelStyle);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 36);

        // Display random scale and rotation toggle.
        randomScale = EditorGUILayout.Toggle("", randomScale);

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable random rotation", labelStyle);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 56);

        // Display random scale and rotation toggle.
        randomRotation = EditorGUILayout.Toggle("", randomRotation);

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
    }

    /// <summary>
    /// Displays the placement options with which the user can set how the prefab will be placed.
    /// </summary>
    private void DrawPlacementOptions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Maximum x position from center ({PrefabValidator.PlaceRange.x} - {PrefabValidator.PlaceRange.y}):", labelStyle);

        // Check so the x range does not exceed the min and max values.
        if (xMaxFromCenter < PrefabValidator.PlaceRange.x)
        {
            xMaxFromCenter = PrefabValidator.PlaceRange.x;
        }
        else if (xMaxFromCenter > PrefabValidator.PlaceRange.y)
        {
            xMaxFromCenter = PrefabValidator.PlaceRange.y;
        }

        // Displays the slider for the x position range.
        xMaxFromCenter = EditorGUILayout.Slider(xMaxFromCenter, PrefabValidator.PlaceRange.x, PrefabValidator.PlaceRange.y, GUILayout.Width(205));

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Maximum z position from center ({PrefabValidator.PlaceRange.x} - {PrefabValidator.PlaceRange.y}):", labelStyle);

        // Check so the z range does not exceed the min and max values.
        if (zMaxFromCenter < PrefabValidator.PlaceRange.x)
        {
            zMaxFromCenter = PrefabValidator.PlaceRange.x;
        }
        else if (zMaxFromCenter > PrefabValidator.PlaceRange.y)
        {
            zMaxFromCenter = PrefabValidator.PlaceRange.y;
        }

        // Display the slider for the z position range.
        zMaxFromCenter = EditorGUILayout.Slider(zMaxFromCenter, PrefabValidator.PlaceRange.x, PrefabValidator.PlaceRange.y, GUILayout.Width(205));

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
    }

    /// <summary>
    /// Displays the Generate Button and calls methods on <see cref="PrefabValidator"/> and <see cref="PrefabGenerator"/> one the Button
    /// has been pressed.
    /// </summary>
    private void DrawGenerateButton()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Env. Object", buttonStyle))
        {
            ValidateGeneration();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(SPACING);
    }

    /// <summary>
    /// Draws a error inside on a HelpBox. Displays it if the errorText length is not zero.
    /// </summary>
    private void DrawErrorBox()
    {
        if (errorText.Length <= 0)
            return;

        EditorGUILayout.HelpBox(errorText, MessageType.Error);
        EditorGUILayout.Space(SPACING);
    }

    /// <summary>
    /// Makes sure all values the user has input are valid and displays a corresponding error message if it is not.
    /// </summary>
    private void ValidateGeneration()
    {
        Debug.Log("Validating selection");

        if (!PrefabValidator.ValidateName(name))
        {
            errorText = nameErrorText;
            Debug.LogError($"Prefab Creation: {errorText}");
            return;
        }
        else if (!PrefabValidator.ValidateMesh(mesh))
        {
            errorText = meshErrorText;
            Debug.LogError($"Prefab Creation: {errorText}");
            return;
        }
        else if (!PrefabValidator.ValidateMaterial(material))
        {
            errorText = materialErrorText;
            Debug.LogError($"Prefab Creation: {errorText}");
            return;
        }
        else
        {
            errorText = "";
            Debug.Log("All options were valid and a new Prefab has been created.");

            Generate();

            PrefabValues values = new PrefabValues();
            values.Name = name;
            values.Mesh = mesh;
            values.Material = material;
            values.CastShadow = shadowCastOption;
            values.RandomScale = randomScale;
            values.RandomRotation = randomRotation;
            values.MaxDistanceFromCenter = new Vector2(xMaxFromCenter, values.MaxDistanceFromCenter.y);
            values.MaxDistanceFromCenter = new Vector2(values.MaxDistanceFromCenter.x, zMaxFromCenter);

            if (prefabGenerator != null)
            {
                prefabGenerator.InitializePrefabCreation(values);
            }

            EditorUtility.DisplayDialog("Success", "A Prefab has been created!", "Close");
        }
    }

    /// <summary>
    /// Makes a new instance of a <see cref="PrefabGenerator"/>, since code created a new prefab must
    /// be a <see cref="MonoBehaviour"/>.
    /// </summary>
    private void Generate()
    {
        if (prefabGenerator == null)
        {
            GameObject generatorContainer = new GameObject();
            generatorContainer.name = "Environment Detail Generator";
            prefabGenerator = generatorContainer.AddComponent<PrefabGenerator>();
        }
    }

    #endregion Methods
}
#endif