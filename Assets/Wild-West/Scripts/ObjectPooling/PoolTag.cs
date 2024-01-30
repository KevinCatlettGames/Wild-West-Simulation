using UnityEngine;

/// <summary>
/// A class holding the tag of the gameObject this is attached that is used for the pool system.
/// </summary>
public class PoolTag : MonoBehaviour
{
    [Tooltip("The tag of this GameObject.")]
    [SerializeField] private string poolTag;

    public string Tag
    {
        get { return poolTag; }
        set { poolTag = value; }
    }
}