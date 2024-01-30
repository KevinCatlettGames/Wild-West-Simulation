using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;

/// <summary>
/// The endless terrain system handles the when new chunks should be generated, the level of detail of each chunk based on an external
/// GameObject and its distance to the chunk and when each chunk should be deactivated or activated.
/// </summary>
public class EndlessTerrain : MonoBehaviour
{
    #region Variables

    [Tooltip("How detailed each detailLevel should be.")]
    [SerializeField] private LODMeshValues[] detailLevels;

    [Tooltip("The viewer from which the terrain will determine which chunks to set as visible.")]
    [SerializeField] private Transform viewer;

    [Tooltip("The Material which displays the colorMap, if regions are enabled.")]
    [SerializeField] private Material mapMaterial;

    /// <summary>
    /// The general scale of the chunk GameObject. Setting this smaller or larger will scale all chunks accordingly
    /// </summary>
    private const float scale = 1;

    /// <summary>
    /// How far the viewer must from the chunk, for the chunk to update.
    /// </summary>
    private const float distanceToViewerForChunkUpdate = 25f;

    /// <summary>
    /// How far the viewer must move from a chunk for the level of detail to change, or for it to become visible or invisible.
    /// </summary>
    private const float viewerMoveDistanceUntilChunkUpdate = distanceToViewerForChunkUpdate * distanceToViewerForChunkUpdate;

    /// <summary>
    /// The MapGenerator in the scene.
    /// </summary>
    private static MapGenerator mapGenerator;

    /// <summary>
    /// How far from the viewer should chunks be active.
    /// </summary>
    private static float maxViewDistance;

    /// <summary>
    /// The current position of the GameObject vieweing the endless terrain.
    /// </summary>
    private static Vector2 viewerPosition;

    /// <summary>
    /// The viewers position last frame.
    /// </summary>
    private Vector2 viewerPositionOld;

    /// <summary>
    /// How large is one chunk.
    /// </summary>
    private int chunkSize;

    /// <summary>
    /// How many chunks are currently active.
    /// </summary>
    private int numberOfVisibleChunks;

    /// <summary>
    /// Storing all chunks in a dictionary prevents the generation of duplicates and checking if a chunk already exists.
    /// </summary>
    private Dictionary<Vector2, TerrainChunk> allTerrainChunks = new Dictionary<Vector2, TerrainChunk>();

    /// <summary>
    /// The chunks that were visible last update become invisible every frame so that it can be redetermined which chunks should be visible.
    /// </summary>
    private static List<TerrainChunk> oldVisibleTerrainChunks = new List<TerrainChunk>();

    #endregion Variables



    #region Unity Methods

    /// <summary>
    /// Initializes the endless terrain generator and gets the chunkSize and how many chunks can be visible at any given time.
    /// </summary>
    private void Start()
    {
        if (!MapGenerator.Instance) return;

        mapGenerator = MapGenerator.Instance;

        maxViewDistance = detailLevels[detailLevels.Length - 1].VisiblityDistance;

        // The chunk size is as large as stated in the MapGenerator, minus one since the chunkSize is actually one less that stated.
        chunkSize = MapGenerator.mapChunkSize - 1;

        numberOfVisibleChunks = Mathf.RoundToInt(maxViewDistance / chunkSize);

        UpdateChunks();
    }

    /// <summary>
    /// Update the viewer position every frame to make sure the correct chunks are visible and invisible.
    /// </summary>
    private void Update()
    {
        UpdateTerrainGeneral();
    }

    #endregion Unity Methods



    #region Method

    /// <summary>
    /// Makes sure to get the current position of the viewer and updates which chunks are visible and invisible
    /// base on the position of the viewer.
    /// </summary>
    private void UpdateTerrainGeneral()
    {
        if (!MapGenerator.Instance) return;

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        // Update the current viewer position
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > viewerMoveDistanceUntilChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateChunks();
        }
    }

    /// <summary>
    /// Gets called every frame and sets the chunks not be visible, then determines
    /// through iteration which chunks are close enough to the viewer to become visible.
    /// </summary>
    private void UpdateChunks()
    {
        for (int i = 0; i < oldVisibleTerrainChunks.Count; i++)
        {
            oldVisibleTerrainChunks[i].SetVisible(false);
        }

        oldVisibleTerrainChunks.Clear();

        // Get the chunks coordinate the viewer is currently standing on.
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        // Iterate through all visible chunks.
        for (int yOffset = -numberOfVisibleChunks; yOffset <= numberOfVisibleChunks; yOffset++)
        {
            for (int xOffset = -numberOfVisibleChunks; xOffset <= numberOfVisibleChunks; xOffset++)
            {
                // Get the position of each iterated chunk that is currently possibly being viewed.
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                // If the chunk already exists, update it.
                if (allTerrainChunks.ContainsKey(viewedChunkCoord))
                {
                    allTerrainChunks[viewedChunkCoord].RecheckTerrainChunks();
                }
                // If it does not exist, generate a new chunk.
                else
                {
                    allTerrainChunks.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    #endregion Method

    /// <summary>
    /// Represents a chunk and holds all information related to it.
    /// </summary>
    public class TerrainChunk
    {
        #region Variables

        /// <summary>
        /// The GameObject holding this chunk, which is initialized as a primitive plane.
        /// </summary>
        private GameObject meshObject;

        /// <summary>
        /// The position of this chunk
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Defines where and where not the mesh is by entailing axis information.
        /// </summary>
        private Bounds bounds;

        /// <summary>
        /// Renders the chunk.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Holds the mesh information of this chunk.
        /// </summary>
        private MeshFilter meshFilter;

        /// <summary>
        /// The possible level of details this chunk could have.
        /// </summary>
        private LODMeshValues[] detailLevels;

        /// <summary>
        /// The mesh in its different lod variants.
        /// </summary>
        private MeshWithLOD[] lodMeshes;

        /// <summary>
        /// The MapData for this chunk.
        /// </summary>
        private MapData mapData;

        /// <summary>
        /// Has the mapData been received? Only update the chunk if this is the case.
        /// </summary>
        private bool gotMapData;

        /// <summary>
        /// Used to check if the LOD needs to be higher or lower.
        /// </summary>
        private int previousLOD = -1;

        #endregion Variables



        #region Constructor

        public TerrainChunk(Vector2 coord, int size, LODMeshValues[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            // Initialize the position and bounds
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            // Initialize the GameObject in the scene
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            // Set the newly created GameObject
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;

            // Disable it at the beginning to make sure it is not immediately shown.
            SetVisible(false);

            // Fill the array of level of detail meshes.
            lodMeshes = new MeshWithLOD[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new MeshWithLOD(detailLevels[i].Lod, RecheckTerrainChunks);
            }

            if (mapGenerator.UseThreading)
                mapGenerator.RequestMapDataThreaded(position, OnGotMapData);
            else
            {
                if (!mapGenerator.TimeChecked)
                {
                    mapGenerator.Stopwatch.Start();
                    mapGenerator.TimeBeingChecked = true;
                }

                OnGotMapData(mapGenerator.RequestMapDataUnthreaded(position));
            }
        }

        #endregion Constructor



        #region Methods

        /// <summary>
        /// Stores the Map Data and applies a texture with color to the material of this chunk accordingly.
        /// Then updates the chunk.
        /// </summary>
        /// <param name="mapData"></param> The MapData this chunk will use to generate its detail and color.
        private void OnGotMapData(MapData mapData)
        {
            this.mapData = mapData;
            gotMapData = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            RecheckTerrainChunks();
        }

        /// <summary>
        /// Finds the point on the mesh that is closest to the viewer. If the distance is smaller
        /// than the view distance enable the mesh, if not disable the mesh.
        /// </summary>
        public void RecheckTerrainChunks()
        {
            if (gotMapData)
            {
                // Determine if this chunk is currently visible.
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDistance;

                // If so, determine which lod should be used.
                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].VisiblityDistance)
                            lodIndex = i + 1;
                        else
                            break;
                    }

                    // If the lod has changed, use the new lod.
                    if (lodIndex != previousLOD)
                    {
                        MeshWithLOD lodMesh = lodMeshes[lodIndex];
                        // If the lod that is to be used has a mesh, just update it.
                        if (lodMesh.hasMesh)
                        {
                            previousLOD = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        // If not, get a new mesh.
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    oldVisibleTerrainChunks.Add(this);
                }
                SetVisible(visible);
            }

            if(mapGenerator.TimeBeingChecked && !mapGenerator.TimeChecked)
            {
                mapGenerator.EvaluateStopWatch();
            }
        }

        /// <summary>
        /// Enables or disables the meshObject depending on its visible state.
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        /// <summary>
        /// Checks the active state of the meshobject.
        /// </summary>
        /// <returns></returns> The active state
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

        #endregion Methods

    }

    /// <summary>
    /// Holds a mesh and its lod index. The LOD mesh is then toggled depending on which LOD should be active at any given time.
    /// </summary>
    private class MeshWithLOD
    {
        #region Variables

        /// <summary>
        /// The mesh with the lod.
        /// </summary>
        public Mesh mesh;

        /// <summary>
        /// Has the mesh been requested to be gotten?
        /// </summary>
        public bool hasRequestedMesh;

        /// <summary>
        /// Has the mesh been gotten?
        /// </summary>
        public bool hasMesh;

        /// <summary>
        /// The index of the lod of this mesh.
        /// </summary>
        private int lod;

        /// <summary>
        /// An Action that tells this class to create the mesh once the <see cref="MeshData"/> has been requested and received.
        /// </summary>
        private System.Action updateMesh;

        #endregion Variables



        #region Constructor

        public MeshWithLOD(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateMesh = updateCallback;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Generates a mesh for this lod.
        /// </summary>
        /// <param name="meshData"></param>
        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateMesh();
        }

        /// <summary>
        /// Gets a <see cref="MapData"/> and pulls the mesh information from it.
        /// </summary>
        /// <param name="mapData"></param>
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            if (mapGenerator.UseThreading)
                mapGenerator.RequestMeshDataThreaded(mapData, lod, OnMeshDataReceived);
            else
                OnMeshDataReceived(mapGenerator.RequestMeshDataUnthreaded(mapData, lod));
        }

        #endregion Methods

    }

    /// <summary>
    /// Holds the index at which a lod should be used and a distance at which the lod should be toggled.
    /// </summary>
    [System.Serializable]
    public struct LODMeshValues
    {
        [Tooltip("The index this lod should have")]
        [SerializeField] private int lod;

        public int Lod { get { return lod; } }

        [Tooltip("The distance where the lod should be toggled.")]
        [SerializeField] private float visiblityDistance;

        public float VisiblityDistance { get { return visiblityDistance; } }
    }
}