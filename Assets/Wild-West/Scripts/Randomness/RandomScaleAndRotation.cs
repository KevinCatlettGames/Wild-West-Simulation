using UnityEngine;

/// <summary>
/// Randomizes the scale of the object this is attached to and its rotation, if the corresponding booleans are true.
/// </summary>
public class RandomScaleAndRotation : MonoBehaviour
{
    #region Variables


    [Tooltip("Should this script randomize the scale?")]
    [SerializeField] private bool randomScale;
    public bool RandomScale { get { return randomScale; } set { randomScale = value; } }
   
    [Tooltip("Should this script randomize the rotation?")]
    [SerializeField] private bool randomRotation;  
    public bool RandomRot { get { return randomRotation; } set {  randomRotation = value; } }

    [Tooltip("The minimum size this GameObject can be.")]
    [SerializeField] private float minScale = 1;

    [Tooltip("The maximum size this GameObject can be.")]
    [SerializeField] private float maxScale = 5;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Calls the methods that handle randomization.
    /// </summary>
    private void Start()
    {
        RandomRotation();
        RandomSize();
    }

    /// <summary>
    /// If this script should, the eulerAngles of this transform are randomly set.
    /// </summary>
    private void RandomRotation()
    {
        if (!randomRotation) return;

        float randomYRotation = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(transform.localRotation.x, randomYRotation, transform.localRotation.z);
    }

    /// <summary>
    /// If this script should, the size of the transform is randomly set.
    /// </summary>
    private void RandomSize()
    {
        if (!randomScale) return;

        float size = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(size, size, size);
    }

    #endregion Methods
}