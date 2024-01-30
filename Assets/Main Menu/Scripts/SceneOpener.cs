using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Opens a Scene which index is specified in the parameter. 
/// </summary>
public class SceneOpener : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
            LoadScene(0);
    }

    /// <summary>
    /// See class summary. 
    /// </summary>
    /// <param name="indexToLoad"></param> The index of the scene that should be loaded. 
    public void LoadScene(int indexToLoad)
    {
        SceneManager.LoadScene(indexToLoad);
    }
}