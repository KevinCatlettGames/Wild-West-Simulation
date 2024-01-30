using UnityEngine;

/// <summary>
/// Creates a two dimensional array representing a falloff map.
/// </summary>
public static class FalloffGenerator
{
    /// <summary>
    /// Creates a fall off map for further use by other scripts.
    /// </summary>
    /// <param name="size"></param> The horizontal and vertical size of the map.
    /// <returns></returns> A two dimensional array representing the falloff map.
    public static float[,] GenerateFalloffMap(int size)
    {
        // Make a instance of the two dimensional float array.
        float[,] map = new float[size, size];

        // Iterate as often as there are values in the size.
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Mathematical equation to generate falloff maps.
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                // Mathf.Max returns the largest of two values. .Abs returns the absolute value.
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    /// <summary>
    /// Manipulates a value, so that more black is present in the falloff map.
    /// </summary>
    /// <param name="value"></param> The newly created value for the map.
    /// <returns></returns>
    private static float Evaluate(float value)
    {
        // Mathematical equation to generate some more inner area in the falloff maps.
        float a = 3;
        float b = 2.2f;

        // Represent the value as needed in the falloff map.
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}