using UnityEngine;

/// <summary>
/// Generates Textures out of color or float arrays to visualize color- and heightmaps on an object, like a plane.
/// Where a Texture generated with a colorMap displays colors, a Texture generated with a heightMap displays a black and white gradient between zero and one.
/// </summary>
public static class TextureGenerator
{
    #region Methods

    /// <summary>
    /// Generates a Texture by setting its size and the color of each pixel to be the same values as given in the parameters.
    /// </summary>
    /// <param name="colourMap"></param> The array of color which allows for custom colour generation, depending on what is needed.
    /// <param name="width"></param> The width of the texture is set outside of this method to allow for custom texture sizes.
    /// <param name="height"></param> The height of the texture is set outside of this method to allow for custom texture sizes.
    /// <returns></returns> A Texture2D which has the size and colors for each pixel that are given as parameters.
    public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height)
    {
        // Create a new Texture with the size given as parameters.
        Texture2D texture = new Texture2D(width, height);

        // Set the fundamental settings of the texture.
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Take each pixel and apply the colorMap onto it, taking the index of each color inside of the array into account for assignment.
        texture.SetPixels(colourMap);
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Generates a Texture by setting its size and the color of each pixel to be a black and white gradient between zero and one,
    /// depending on the float values inside of the twodimensional heightMap array at the corresponding index.
    /// </summary>
    /// <param name="heightMap"></param> The twodimensional array of float values which allows for custom heightMap visualization on a Texture.
    /// <returns></returns> A Texture2D which has the size and black and white gradient color for each pixel that are given as parameters.
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // Set the width to be as large as the first dimension.
        int width = heightMap.GetLength(0);
        // Set the height to be as large as the second dimension.
        int height = heightMap.GetLength(1);

        // Make a new color array instance and make its size as large as there should be pixels on the texture.
        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Multiply y by the width to get the index of the row, then to get the column add the x value.
                // Lerp between black and white to get a gradient, where the value of the current heightMap value is to be used as lerp value.
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    #endregion Methods
}