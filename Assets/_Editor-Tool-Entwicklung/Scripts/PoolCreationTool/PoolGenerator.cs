using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
/// <summary>
/// After the <see cref="PoolValidator"/> has verified the input of the user this script creates a new instance of a <see cref="Pool"/> scriptable object
/// and saves all information from the <see cref="PoolValues"/> instance to it.
/// </summary>
public static class PoolGenerator
{
    /// <summary>
    /// See class summary.
    /// </summary>
    /// <param name="values"></param> The give <see cref="PoolValues"/> instance that is currently holding the input of the user.
    public static void CreatePool(PoolValues values)
    {
        // Instancing.
        var pool = ScriptableObject.CreateInstance<Pool>();

        // Setting the values.
        pool.PoolName = values.Name;
        pool.Tags = values.Tags.ToArray();
        pool.Objects = values.Objects.ToArray();
        pool.LoadAmount = values.Amount;
        pool.RandomizeSpawnAmount = values.RandomizeSpawnAmount;
        pool.MinPlaceAmount = values.MinAmount;
        pool.MaxPlaceAmount = values.MaxAmount;

        // Saving the scriptable object
        AssetDatabase.CreateAsset(pool, PathHolder.ENVIRONMENTTOOLSCRIPTABLEOBJECTFOLDER + pool.PoolName + ".asset");
        AssetDatabase.SaveAssets();
    }
}
#endif 