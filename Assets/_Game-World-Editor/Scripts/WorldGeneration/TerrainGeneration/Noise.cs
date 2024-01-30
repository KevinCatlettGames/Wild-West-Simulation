using UnityEngine;

/// <summary>
/// A detailed perlin noise map, taking octaves, persistance and lacunarity into account allows for a gradual decrease and increase in noise.
/// </summary>
public class Noise
{
    // Local MinMaxConsideration allows for use of the local minimum and maximum noiseHeight, which is the standard noise height.
    // Global MinMaxConsideration allows estimation of a global minimum and maximum noiseHeight, which is needed for consideration of further chunks.
    public enum MinMaxConsideration
    { Local, Global };

    /// <summary>
    /// Generates a random perlin noise map to be used as base for terrain generation.
    /// </summary>
    /// <param name="mapWidth"></param> The width of the map.
    /// <param name="mapHeight"></param> The height of the map.
    /// <param name="seed"></param> A seed used for randomization.
    /// <param name="scale"></param> The scale of the noise.
    /// <param name="octaves"></param> Controls details of the perlin noise.
    /// <param name="persistance"></param> Controls decrease in amplitude of octaves.
    /// <param name="lacunarity"></param> Controls increase in frequency of octaves.
    /// <param name="offset"></param> When changed the perlin noise is offset giving new values.
    /// <param name="minMaxConsideration"></param> Controls wether the calculation of minimum and maximum noiseHeight values should take the generation of further chunks into account.
    /// <returns></returns> A two dimensional float array entailing values between 0 and 1 that represent a perlin noise map.
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, MinMaxConsideration minMaxConsideration)
    {
        // Initialize a two dimensional float array the size of mapWidth and mapHeight.
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Initialize a random int value using a seed.
        System.Random prng = new System.Random(seed);

        // Initialize a Vector2 array, where the length is equal to the octaves value.
        Vector2[] octaveOffsets = new Vector2[octaves];

        // Declare a max height value to keep track of what the maximum height value can be.
        float maxPossibleHeight = 0;

        float amplitude = 1;
        float frequency = 1;

        // Loops through the octaves and creates the values for each octave, so to layer multiple levels of noise.
        for (int i = 0; i < octaves; i++)
        {
            // Randomizes the values of each octave.
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            // Make sure the amplitude is added onto the max height.
            maxPossibleHeight += amplitude;

            // Assures decreasion of the strength of each further octave.
            amplitude *= persistance;
        }
        if (scale <= 0)
            // Stops the scale from entailing a negative number, which would crash the perlin noise generation.
            scale = 0.0001f;

        // Makes sure the height can not be too small or too big.
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // Generate the noise map taking the octaveOffset array into account.
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Reset amplitude, frequency and noiseHeight so that it does not influence the octaveoffset
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // At which weight in the PerlinNoise should a value be taken from.
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    // Create the perlin noise map and make sure the perlin noise will be in the range negative one to one by multiplying by two and subtracting my one.
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    // To make sure that the octave distances do not get too large, the amplitude gets "pulled back" by the persistance.
                    amplitude *= persistance;
                    // To make sure that the octave distances do not get too small, the frequency gets "pulled apart" by the lacunarity.
                    frequency *= lacunarity;
                }

                // Make sure the noiseHeight is not higher or lower than the min and max noise height.
                // If this were to happen the NoiseGeneration would crash.
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                // Add it to the two dimensional perlin noise map float array.
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // If the endless terrain system is not used, the minimum and maximum noise height are the given values.
                if (minMaxConsideration == MinMaxConsideration.Local)
                {
                    // Changes the value to be 0 or 1, depending on which is closest, so that the values can be used by the mesh generation in a reasonable manner.
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                // If the endless terrain is used, the minimum and maximum noiseheight must be calculated.
                else
                {
                    // With the found maxPossibleHeight value, calculated while creating the octaves, a generally fitting noise height can be found.
                    float generalNoiseHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;

                    // Make sure the generalNoiseHeight value is clamped, so that it does not reach below zero or the maximum value possible in a float.
                    noiseMap[x, y] = Mathf.Clamp(generalNoiseHeight, 0, int.MaxValue);
                }
            }
        }
        return noiseMap;
    }
}