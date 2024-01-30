using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes sure the input of the user is valid and returns boolean values verifiying the input.
/// Methods are called by the <see cref="PoolWindow"/> class.
/// </summary>
public static class PoolValidator
{
    /// <summary>
    /// How many objects can be instanced at maximum from one pool (theoretical number).
    /// </summary>
    public const int MAXIMUMLOADAMOUNT = 500;

    /// <summary>
    /// The length of the pool name should not be zero.
    /// </summary>
    /// <param name="name"></param> The name that should be verified.
    /// <returns></returns> If the name is able to be used.
    public static bool ValidateName(string name)
    {
        // name length <= 0.
        if (string.IsNullOrEmpty(name)) return false;
        return true;
    }

    /// <summary>
    /// The tag list should not be empty and the tag length should not be zero.
    /// </summary>
    /// <param name="tags"></param> The List of tags that should be verified.
    /// <returns></returns> If the tags are able to be used.
    public static bool ValidateTags(List<string> tags)
    {
        // tags count <= 0.
        if (tags.Count <= 0) return false;

        // If one of the tags is 0 of length.
        foreach (string tag in tags)
        {
            if (tag.Length <= 0) return false;
        }
        return true;
    }

    /// <summary>
    /// The objects list should not be empty.
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns> If the objects are able to be used.
    public static bool ValidateObjects(List<GameObject> objects)
    {
        // objects count <= 0.
        if (objects.Count <= 0) return false;

        // If one of the objects is null.
        foreach (GameObject o in objects)
        {
            if (o == null) return false;
        }
        return true;
    }
}