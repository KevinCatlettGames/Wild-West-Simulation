using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
/// <summary>
/// The PoolWindow handles displaying the elements of the PoolTab in the EnvironmentTool.
/// Once all is set and the Generate Button is pressed, this script handles calls for user input validation by the <see cref="PoolValidator"/>
/// and generating a new <see cref="Pool"/> Scriptable Object by the <see cref="PoolGenerator"/>.
/// </summary>
public class PoolWindow
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
    private string name = "PoolName";

    /// <summary>
    /// The tags list holding all tags created by the user.
    /// </summary>
    private List<string> tags = new List<string>();

    /// <summary>
    /// The objects list holding all GameObjects that are in this pool.
    /// </summary>
    private List<GameObject> objects = new List<GameObject>();

    /// <summary>
    /// Should the spawn amount be random?
    /// </summary>
    private bool randomizeSpawnAmount = true;

    /// <summary>
    /// The amount of GameObjects that are definetly spawned from this pool.
    /// </summary>
    private int loadAmount = 10;

    /// <summary>
    /// The positioning range of the GameObject, where x is the minimum value and y is the maximum value in the xaxis and z axis.
    /// </summary>
    private Vector2 randomPlacementRange = new Vector2(0, 175);

    /// <summary>
    /// The GUIStyles for the different elements of this window.
    /// </summary>
    private GUIStyle labelStyle;

    private GUIStyle textFieldStyle;
    private GUIStyle buttonStyle;

    /// <summary>
    /// The error text that is currently being displayed.
    /// </summary>
    private string errorText = "";

    private const string nameErrorText = "The pool name is too short, over twenty characters long, or already exists, please pick a valid pool name.";
    private const string tagErrorText = "One of the tags is too short or over twenty character long, please input a correct name for a tag.";
    private const string objectsErrorText = "One of the objects you have selected must be null, please make sure all selections are actually valid.";

    #endregion Variables

    #region Constructor

    public PoolWindow(GUIStyle labelStyle, GUIStyle textFieldStyle, GUIStyle buttonStyle)
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
        DrawNameOption();
        DrawTagOption();
        DrawObjectsOptions();
        DrawLoadAmountOptions();
        DrawGenerateButton();
        DrawErrorBox();
    }

    /// <summary>
    /// Displays the Name label and a textfield in which the user can input the name.
    /// </summary>
    private void DrawNameOption()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Pool Name:", labelStyle);
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
    }

    /// <summary>
    /// Displays the Tag label and a TextArea Dropdown in which the user can input the tags.
    /// </summary>
    private void DrawTagOption()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tags:", labelStyle);
        GUILayout.FlexibleSpace();

        // Make sure one TextArea is always displayed.
        if (tags.Count <= 0)
            tags.Add("DefaultTag");
        GUILayout.BeginVertical();
        // Display Dropdown options corresponding to the tags count.
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].Length > NAMELIMIT)
            {
                string modifiedName = tags[i].Substring(0, tags[i].Length - 1);
                tags[i] = modifiedName;
            }

            tags[i] = GUILayout.TextArea(tags[i], textFieldStyle);
        }

        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Handle the + and - Button placement.
        if (tags.Count < 5)
        {
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                tags.Add("DefaultTag" + (tags.Count + 1).ToString());
            }
        }
        if (tags.Count > 1)
        {
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                tags.RemoveAt(tags.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
    }

    /// <summary>
    /// Displays the Objects label and a Dropdown in which the user can input the prefabs.
    /// </summary>
    private void DrawObjectsOptions()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Objects:", labelStyle);
        GUILayout.FlexibleSpace();

        // Make sure at least one Prefab is selected.
        if (objects.Count <= 0)
        {
            if ((GameObject)AssetDatabase.LoadAssetAtPath(PathHolder.ENVIRONMENTTOOLPREFABFOLDER + "/_DefaultObject.prefab", typeof(GameObject)))
                objects.Add((GameObject)AssetDatabase.LoadAssetAtPath(PathHolder.ENVIRONMENTTOOLPREFABFOLDER + "/_DefaultObject.prefab", typeof(GameObject)));
        }
        GUILayout.BeginVertical();
        // Display Dropdown options corresponding to the objects count.
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i] = EditorGUILayout.ObjectField(objects[i], typeof(GameObject), false, GUILayout.Width(205)) as GameObject;
        }

        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Handle the + and - Button placement.
        if (objects.Count < 5)
        {
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                if ((GameObject)AssetDatabase.LoadAssetAtPath(PathHolder.ENVIRONMENTTOOLPREFABFOLDER + "/_DefaultObject.prefab", typeof(GameObject)))
                {
                    objects.Add((GameObject)AssetDatabase.LoadAssetAtPath(PathHolder.ENVIRONMENTTOOLPREFABFOLDER + "/_DefaultObject.prefab", typeof(GameObject)));
                }
            }
        }
        if (objects.Count > 1)
        {
            if (GUILayout.Button("-", GUILayout.Width(30)))
                objects.RemoveAt(objects.Count - 1);
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(SPACING);
    }

    /// <summary>
    /// Displays the LoadAmount toggle and label and a slider with which the user can input the amount of objects that should be loaded.
    /// </summary>
    private void DrawLoadAmountOptions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Pool amount:", labelStyle);
        GUILayout.FlexibleSpace();
        // Display load amount slider.
        loadAmount = EditorGUILayout.IntSlider(loadAmount, 1, PoolValidator.MAXIMUMLOADAMOUNT, GUILayout.Width(205));

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Random place amount toggle:", labelStyle);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 100);
        // Display toggle for choosing if a random spawn amount should be used.
        randomizeSpawnAmount = EditorGUILayout.Toggle("", randomizeSpawnAmount);

        GUILayout.EndHorizontal();
        GUILayout.Space(SPACING);

        if (randomizeSpawnAmount)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Place amount values (min and max):", labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            // Display random placement range sliders
            randomPlacementRange = new Vector2(EditorGUILayout.Slider(randomPlacementRange.x, 1, loadAmount, GUILayout.Width(205)),
            EditorGUILayout.Slider(randomPlacementRange.y, randomPlacementRange.x, loadAmount, GUILayout.Width(205)));

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(SPACING);
        }
    }

    /// <summary>
    /// Displays the Generate Button and calls methods on <see cref="PoolValidator"/> and <see cref="PoolGenerator"/> once the Button
    /// has been pressed.
    /// </summary>
    private void DrawGenerateButton()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Scriptable Object", buttonStyle))
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
    /// Calls input validation and creates a new pool if validation comes back with true booleans.
    /// </summary>
    private void ValidateGeneration()
    {
        Debug.Log("Validating selection");

        if (!PoolValidator.ValidateName(name))
        {
            errorText = nameErrorText;
            Debug.LogError($"Pool Creation: {errorText}");
            return;
        }
        else if (!PoolValidator.ValidateTags(tags))
        {
            errorText = tagErrorText;
            Debug.LogError($"Pool Creation: {errorText}");
            return;
        }
        else if (!PoolValidator.ValidateObjects(objects))
        {
            errorText = objectsErrorText;
            Debug.LogError($"PoolCreation: {errorText}");
            return;
        }
        else
        {
            errorText = "";
            Debug.Log("All options were valid and a new Pool scriptable object has been created.");

            PoolValues values = new PoolValues();
            values.Name = name;
            values.Tags = tags;
            values.Objects = objects;
            values.Amount = loadAmount;
            values.RandomizeSpawnAmount = randomizeSpawnAmount;
            values.MinAmount = (int)randomPlacementRange.x;
            values.MaxAmount = (int)randomPlacementRange.y;

            PoolGenerator.CreatePool(values);
            EditorUtility.DisplayDialog("Success", "A Pool scriptable object has been created!", "Close");
        }
    }

    #endregion Methods
}
#endif