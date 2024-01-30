using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all generation in the scene for terrain generation. This entails displaying 2D Noise- and Color maps, generating Meshes with detail by influencing the height of its vertices through octaves and using a
/// falloff map on these options.
/// </summary>
[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    #region Variables
 
    /// <summary>
    /// This class as singleton.
    /// </summary>
    public static MapGenerator Instance;

    /// <summary>
    /// Defines what should be currently drawn.
    /// </summary>
    public enum DrawMode
    { NoiseMap, ColorMap, Mesh, FalloffMap };

    [Tooltip("The currently selected draw mode.")]
    [SerializeField] private DrawMode drawMode;

    [Tooltip("The currently applied biome")]
    [SerializeField] private BiomeData biomeData; 

    [Tooltip("Should flatshading be applied on the mesh?")]
    [SerializeField] private bool useFlatShading;

    [Tooltip("How detailed should the mesh be while not in playmode?")]
    [Range(0, 6)]
    [SerializeField] private int editorPreviewLOD;

    [Tooltip("Should the mesh be automatically updated when changes occur?")]
    [SerializeField] private bool autoUpdate;

    public bool AutoUpdate
    { get { return autoUpdate; } }

    [Tooltip("Should threading be used to generate the terrain? If false performance is dramatically decreased.")]
    [SerializeField] private bool useThreading = true;

    [Tooltip("Stopwatch GUI")]
    [SerializeField] private TextMeshProUGUI stopWatchText;

    public bool UseThreading
    { get { return useThreading; } }

    /// <summary>
    /// The current falloffMap as two-dimensional float array.
    /// </summary>
    private float[,] falloffMap;

    /// <summary>
    /// Make sure this singleton is set, which can not only be done in play mode since this script needs to
    /// also work inside of the editor.
    /// Depending on if flatshading is used, the size of each chunk must be smaller so to represent the correct shading.
    /// </summary>
    public static int mapChunkSize
    {
        get
        {
            if (Instance == null)
                Instance = FindObjectOfType<MapGenerator>();
            if (Instance && Instance.useFlatShading)
                return 95;
            else
                return 239;
        }
    }

    /// <summary>
    /// Stops the time to see how long the original generation of chunks takes. 
    /// </summary>
    private Stopwatch stopwatch;
    public Stopwatch Stopwatch { get { return stopwatch; } set { stopwatch = value; } }

    /// <summary>
    /// Has the stopwatch been used? 
    /// </summary>
    private bool timeChecked;
    public bool TimeChecked { get { return timeChecked; } set { timeChecked = value; } }

    /// <summary>
    /// Is the stopwatch timer currently running? 
    /// </summary>
    private bool timeBeingChecked;
    public bool TimeBeingChecked { get { return timeBeingChecked; } set { timeBeingChecked = value; } }

    #region Threading Variables

    /// <summary>
    /// A Queue of <see cref="MapGenerator"/> specific elements which are processed one at a time listing all processes that should happen in a different thread.
    /// </summary>
    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

    /// <summary>
    /// A Qeuue of <see cref="MeshGenerator"/> specific elements which are processed one at a time listing all processes that should happen in a different thread.
    /// </summary>
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    /// <summary>
    /// Holds the Action that should be performed and the value that is given as parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam> A generic value since the <see cref="MapGenerator"/> and <see cref="MeshGenerator"/> classes use this with different parameters.
    private struct MapThreadInfo<T>
    {
        /// <summary>
        /// The method that should be called in the thread.
        /// </summary>
        public readonly Action<T> callback;

        /// <summary>
        /// The parameter that is passed through the thread.
        /// </summary>
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

    #endregion Threading Variables



    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Gets a falloffMap from the FalloffGenerator.
    /// </summary>
    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        stopwatch = new Stopwatch();
    }

    /// <summary>
    /// Makes sure all threads are being processed and with it all terrain chunks.
    /// </summary>
    private void Update()
    {
        // Check performance when threading by viewing how much time has passed after the scene loads and the first chunks have been initialized. 
        StopWatchWhileThreadingActivation();
        StopWatchWhileThreadingDeactivation();

        ProcessThreadQueues();
    }

    /// <summary>
    /// Make sure the <see cref="TerrainData"/> and <see cref="NoiseData"/> resubscribe per change,
    /// so that the subscription count stays at one.
    /// </summary>
    private void OnValidate()
    {
        if (!biomeData) return; 

        if (biomeData.TerrainData != null)
        {
            biomeData.TerrainData.OnValuesUpdated -= OnValuesUpdated;
            biomeData.TerrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (biomeData.NoiseData != null)
        {
            biomeData.NoiseData.OnValuesUpdated -= OnValuesUpdated;
            biomeData.NoiseData.OnValuesUpdated += OnValuesUpdated;
        }
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Define the starting point for the terrain and generate the first MapData.
    /// Depending on the <see cref="DrawMode"/> value, either display a heightMap, colorMap, falloffMap or mesh.
    /// </summary>
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();

        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;

            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;

            case DrawMode.Mesh:
                if (!biomeData || mapData.Equals(default(MapData))) return;
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, biomeData.TerrainData.MeshHeightMultiplier, biomeData.TerrainData.MeshHeightCurve, editorPreviewLOD, useFlatShading), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;

            case DrawMode.FalloffMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
                break;

            default:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, biomeData.TerrainData.MeshHeightMultiplier, biomeData.TerrainData.MeshHeightCurve, editorPreviewLOD, useFlatShading), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;
        }
    }

    #region Threading Methods

    /// <summary>
    /// Creates a new thread that generates a <see cref="MeshData"/>.
    /// </summary>
    /// <param name="center"></param> The center position of the mesh.
    /// <param name="callback"></param> An Action that receives a <see cref="MeshData"/> for generating a mesh.
    public void RequestMapDataThreaded(Vector2 center, Action<MapData> callback)
    {
        // Initialize the thread.
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        // Start the thread.
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// NOT ADVISED for endless terrain generation or constant new generation of chunks.
    /// Generates a map data without using threading.
    /// </summary>
    /// <param name="center"></param> The center of the newly initialized chunk.
    /// <returns></returns> A <see cref="MapData"/> holding all necessary information about a chunk.
    public MapData RequestMapDataUnthreaded(Vector2 center)
    {
        MapData mapData = GenerateMapData(center);
        return mapData;
    }

    /// <summary>
    /// Is called on a different thread.
    /// Handles generation of a mapData for a terrain chunk.
    /// </summary>
    /// <param name="center"></param> The center position of the mesh.
    /// <param name="callback"></param> An Action that receives a <see cref="MeshData"/> for generating a mesh.
    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);

        // Since the queue will be accessed from different threads, locking must occur so that a call does not get missed.
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    /// <summary>
    /// Creates a new thread that generates a new terrain chunk.
    /// </summary>
    /// <param name="mapData"></param> The mapdata to use for generating the terrain chunk.
    /// <param name="lod"></param> The level of detail this mesh should have.
    /// <param name="callback"></param> An Action that receives a <see cref="MeshData"/> for generating a mesh.
    public void RequestMeshDataThreaded(MapData mapData, int lod, Action<MeshData> callback)
    {
        // Initiialize the thread.
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        // Start the thread.
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// NOT ADVISED for endless terrain generation or constant new generation of chunks.
    /// Generates a <see cref="MeshData"/> without using threading.
    /// </summary>
    /// <param name="mapData"></param> The <see cref="MapData"/> from which to take the values from for mesh generation.
    /// <param name="lod"></param> The level of detail this mesh should have.
    /// <returns></returns> A <see cref="MeshData"/> holding the newly created mesh and its values.
    public MeshData RequestMeshDataUnthreaded(MapData mapData, int lod)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, biomeData.TerrainData.MeshHeightMultiplier, biomeData.TerrainData.MeshHeightCurve, lod, useFlatShading);
        return meshData;
    }

    /// <summary>
    /// Is called on a different thread.
    /// Handles generation of the terrain chunk.
    /// </summary>
    /// <param name="mapData"></param> The <see cref="MapData"/> from which to create the terrain from.
    /// <param name="lod"></param> The lod index this mesh should be in.
    /// <param name="callback"></param> An Action that receives a <see cref="MeshData"/> for generating a mesh.
    private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, biomeData.TerrainData.MeshHeightMultiplier, biomeData.TerrainData.MeshHeightCurve, lod, useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    /// <summary>
    /// Checks if a queue has a unprocessed thread and if so initializes it and removes
    /// it from the queue.
    /// </summary>
    private void ProcessThreadQueues()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {              
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    /// <summary>
    /// Makes sure the stopwatch begins when the scene first loads and threading is used. 
    /// </summary>
    private void StopWatchWhileThreadingActivation()
    {
        if (mapDataThreadInfoQueue.Count > 0 && !timeChecked && !timeBeingChecked)
        {
            timeBeingChecked = true;
            stopwatch.Start();
        }
    }

    /// <summary>
    /// Makes sure the stopwatch stops after the first chunsk have been loaded in when the scene first loads. 
    /// </summary>
    private void StopWatchWhileThreadingDeactivation()
    {
        if (meshDataThreadInfoQueue.Count <= 0 && mapDataThreadInfoQueue.Count <= 0 && !timeChecked && timeBeingChecked)
        {
            EvaluateStopWatch();
        }
    }

    #endregion Threading Methods

    /// <summary>
    /// Taking randomization values like noise and the seed into account,
    /// this creates a new <see cref="MapData"/>.
    /// </summary>
    /// <param name="center"></param> The center position of the mesh being created.
    /// <returns></returns> A <see cref="MeshData"/> will all necessary information to generate a new mesh.
    private MapData GenerateMapData(Vector2 center)
    {

        #if (UNITY_EDITOR)
        // Sets a random integer to use for creating the noise map.
        if (biomeData.NoiseData.RandomSeed && EditorApplication.isPlaying)
        {
            biomeData.NoiseData.Seed = (int)UnityEngine.Random.Range(0, 100000);
        }
        #endif 

        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, biomeData.NoiseData.Seed, biomeData.NoiseData.NoiseScale, biomeData.NoiseData.Octaves, biomeData.NoiseData.Persistance, biomeData.NoiseData.Lacunarity, center + biomeData.NoiseData.Offset, biomeData.NoiseData.MinMaxConsideration);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        // Iterate through all values in the noise and color map.
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                // If the falloff map is used, the generated falloffMap should replace the noiseMap values where no detail should be.
                if (biomeData.TerrainData.UseFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                // Get the height of the current iteration and use that to fill the colorMap with the appropriate color defined in the Regions.
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < biomeData.TerrainData.Regions.Length; i++)
                {
                    if (currentHeight >= biomeData.TerrainData.Regions[i].Height)
                    {
                        colorMap[y * mapChunkSize + x] = biomeData.TerrainData.Regions[i].Color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    /// <summary>
    /// Gets called from the <see cref="OnValidate"/> method every time a change occurs in the inspector.
    /// Makes sure the generation refreshes from inside the editor and while not in play mode.
    /// </summary>
    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    /// <summary>
    /// Stops the stopwatch and outputs the result to the console.
    /// </summary>
    public void EvaluateStopWatch()
    {
        timeBeingChecked = false;
        timeChecked = true;
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Time passed while generating: {stopwatch.Elapsed.TotalSeconds}");

        if(stopWatchText)
            stopWatchText.text = $"Time passed while generating: {stopwatch.Elapsed.TotalSeconds.ToString()}";

    }

    #endregion Methods
}

/// <summary>
/// Multiple regions combined handles the changing of color of the terrain depending on the height of the points.
/// </summary>
[System.Serializable]
public struct Region
{
    #region Variables

    [Tooltip("The name of this region, only used in the inspector.")]
    [SerializeField] private string name;

    public string Name { get { return name; } }

    [Tooltip("The height at which this color should start appearing. All higher points that have not been declared by other regions will have this color.")]
    [SerializeField] private float height;

    public float Height { get { return height; } }

    [Tooltip("The color this region should have starting at the given height.")]
    [SerializeField] private Color color;

    public Color Color { get { return color; } }

    #endregion Variables
}

/// <summary>
/// Holds the height and color maps of a chunk.
/// </summary>
public struct MapData
{
    #region Variables

    /// <summary>
    /// The heightMap that has been generated and is now stored in this struct for further use.
    /// </summary>
    public readonly float[,] heightMap;

    /// <summary>
    /// The colorMap that has been generated and is now stored in this struct for further use.
    /// </summary>
    public readonly Color[] colorMap;

    #endregion Variables

    #region Constructor

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colourMap;
    }

    #endregion Constructor
}