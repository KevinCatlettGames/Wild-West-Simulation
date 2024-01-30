using UnityEngine;

/// <summary>
/// Taking height values into account which have beforehand been calculated by using perlin noise this class generates values for a terrain mesh.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Takes height values and other elements into account to generate all values needed to generate a terrain mesh.
    /// </summary>
    /// <param name="heightMap"></param> The heightmap made by generating random perlin noise and calculating octaves onto it.
    /// <param name="heightMultiplier"></param> A simple float value that is multiplied ontop of the heightmap.
    /// <param name="_heightCurve"></param> A Animation Curve overriding the heightmap for more control over heights at different values.
    /// <param name="levelOfDetail"></param> Depending on the distance to an specified GameObject, the amount of vertices / triangles / faces gets lower or higher to have LOD on the mesh and save performance.
    /// <param name="useFlatShading"></param> Should the generated terrain be flat shaded? This means all normals are pointed in the same direction which makes lighting simulation the same on the entire mesh, making it look flatshaded.
    /// <returns></returns> A MeshData holding all information needed to generate the terrain.
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail, bool useFlatShading)
    {
        // When taking the values from the height curve inside of the editor, a bug happens where the keys are not used correctly. Initializing a new AnimationCurve solves this issue.
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        // Set the meshLOD to be a value one higher than what is set in the levelOfDetail, since a value of zero should not actually happen.
        int meshLOD = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        // Get the border of the map.
        int borderedSize = heightMap.GetLength(0);

        // Set the actual mesh size to be one less than the borders, because these will be generated seperately.
        int meshSize = borderedSize - 2 * meshLOD;

        // Get values indicating what the size of the mesh actually is, without LOD.
        int meshSizeUnsimplified = borderedSize - 2;
        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        // Get the vertices for each line while taking the LOD into account.
        int verticesPerLine = (meshSize - 1) / meshLOD + 1;

        MeshData meshData = new MeshData(verticesPerLine, useFlatShading);

        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        // Iterate through all vertices
        for (int y = 0; y < borderedSize; y += meshLOD)
        {
            for (int x = 0; x < borderedSize; x += meshLOD)
            {
                // Check if the current iteration is a vertex on the mesh border
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                // If it is, set the current value to be the borderindex and subtract one from the borderindex so that it is correctly represented inside of the two dimensional vertexIndicesMap array.
                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                // If not, just add to the vertex index to check how many vertices there are, without taking the bordervertices into account.
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        // Iterate through all border vertices while taking the LOD into account.
        for (int y = 0; y < borderedSize; y += meshLOD)
        {
            for (int x = 0; x < borderedSize; x += meshLOD)
            {
                // Get the index of the currently inspected vertex.
                int vertexIndex = vertexIndicesMap[x, y];

                // Calculate the percentage of the current position within the mesh grid.
                Vector2 percent = new Vector2((x - meshLOD) / (float)meshSize, (y - meshLOD) / (float)meshSize);

                // Evaluate the height of the terrain at the current grid position using a height curve
                // and apply a height multiplier to get the final height value.
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;

                // Calculate the 3D position of the vertex using the calculated percentage.
                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

                // Add a vertex to the meshData with the newly calculated vertexposition,
                meshData.AddVertex(vertexPosition, percent, vertexIndex);

                // Check if the current grid position (x, y) is within the bounds of the bordered grid.
                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    // Get the vertex indices for four corners of a quad in the grid.
                    int a = vertexIndicesMap[x, y]; // Bottom left index.
                    int b = vertexIndicesMap[x + meshLOD, y]; // Bottom right index.
                    int c = vertexIndicesMap[x, y + meshLOD]; // Top left index.
                    int d = vertexIndicesMap[x + meshLOD, y + meshLOD]; // Top right index.

                    // This creates two triangles that form a quad: (a, d, c) and (d, a, b).
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
                vertexIndex++;
            }
        }
        // Once all vertices and triangles have been generated the mesh data can be processed, which means either the flatshading
        // can be generated or the standard normals.
        meshData.ProcessMesh();

        return meshData;
    }
}

/// <summary>
/// Holds all information about the Mesh, which can then be handed to the <see cref="MapGenerator"/> class to generate the map.
/// Can take care of adding vertices, triangles and calculating the normals before being handed over.
/// </summary>
public class MeshData
{
    #region Variables

    /// <summary>
    /// The vertices of this mesh calculated by the <see cref="MeshGenerator"/> class.
    /// </summary>
    private Vector3[] vertices;

    /// <summary>
    /// The vertices on the border of the mesh calculated by the <see cref="MeshGenerator"/> class.
    /// </summary>
    private Vector3[] borderVertices;

    /// <summary>
    /// The triangles of this mesh calculated by the <see cref="MeshGenerator"/> class.
    /// </summary>
    private int[] triangles;

    /// <summary>
    /// The triangles on the border of the mesh calculated by the <see cref="MeshGenerator"/> class.
    /// </summary>
    private int[] borderTriangles;

    /// <summary>
    /// Keeps count of the triangles which is needed to create the normals of the mesh.
    /// </summary>
    private int triangleIndex;

    /// <summary>
    /// Keeps count of the triangles on the border of the mesh which is needed to create the normals of the mesh.
    /// </summary>
    private int borderTriangleIndex;

    /// <summary>
    /// A array holding the x and y axis values of each uv point.
    /// </summary>
    private Vector2[] uvs;

    /// <summary>
    /// A array holding the x and y axis values of each normal point.
    /// </summary>
    private Vector3[] bakedNormals;

    /// <summary>
    /// Should flat shading occur?
    /// </summary>
    private bool useFlatShading;

    #endregion Variables



    #region Constructor

    public MeshData(int verticesPerLine, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;

        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];

        // Multiply by six to for the six vertices in two triangles, which make a quad.
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        // Multiply by four to get the total number of border vertices, plus four for the corners.
        borderVertices = new Vector3[verticesPerLine * 4 + 4];

        // Multiply by twenty-four to get the total amount of triangles
        borderTriangles = new int[24 * verticesPerLine];
    }

    #endregion Constructor

    #region Methods

    #region Vertices and Triangles

    /// <summary>
    /// Either adds a new value to the array of bordervertices or normal vertices, depending on the position of the vertex
    /// and the index.
    /// </summary>
    /// <param name="vertexPosition"></param> The position of the vertex that should be added.
    /// <param name="uv"></param> The Vector2 uv value of the vertex.
    /// <param name="vertexIndex"></param> The index of this vertex.
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
            borderVertices[-vertexIndex - 1] = vertexPosition;
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    /// <summary>
    /// Either adds a new value to the bordertriangles or normal triangles, depending on the values of the three vertices of the triangle.
    /// </summary>
    /// <param name="a"></param> The integer of the first vertex.
    /// <param name="b"></param> The integer of the second vertex.
    /// <param name="c"></param> The integer of the third vertex.
    public void AddTriangle(int a, int b, int c)
    {
        // If a given integer is less than 0, all vertices are part of a bordertriangle.
        if (a < 0 || b < 0 || c < 0)
        {
            // Add the new integers to the index array and add to the triangle index so that already set ones are not overwritten.
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        }
        else
        {
            // Add the new integers to the index array and add to the triangle index so that already set ones are not overwritten.
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    #endregion Vertices and Triangles



    #region Normals

    /// <summary>
    /// Makes sure lighting works correctly without showing seems, even with chunks.
    /// </summary>
    /// <returns></returns> A Vector3 holding the calculated normals for this mesh.
    private Vector3[] CalculateNormals()
    {
        // Initialize a Vector3 that has the same length as the amount of existing vertices.
        Vector3[] vertexNormals = new Vector3[vertices.Length];

        // Dividing by three gives the number of actual triangles.
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            // Multiplied with three gives the index of the triangle inside of the triangle array.
            int normalTriangleIndex = i * 3;

            // Get the vertex indeces of all vertices in the triangle.
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            // Get the triangles normal.
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            // Add the triangles normal to the vertexnormal Vector3 array.
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        // Also loop through the border triangles to generate the normals for the triangles.
        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            // Only add the vertex to the vertexNormals array if it is above or equal to zero, if not it is does not exist in the vertexIndex array.
            if (vertexIndexA >= 0)
                vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0)
                vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0)
                vertexNormals[vertexIndexC] += triangleNormal;
        }

        // Normalize each value.
        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }

    /// <summary>
    /// Calculates the normal vector of a triangle based on the given values.
    /// </summary>
    /// <param name="indexA"></param> The value of the first vertex.
    /// <param name="indexB"></param> The value of the second vertex.
    /// <param name="indexC"></param> The value of the third vertex.
    /// <returns></returns> An array entailing the normals of each triangle.
    private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        // Check if the vertex is a border vertex and set a Vector3 value accordingly.
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        // Use cross product, where given two vectors a vector is returned that is perpendicular to the vectors to calculate the sides
        // of the triangle.

        // Get two sides.
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        // Return the cross product and normalize the result.
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    /// <summary>
    /// When needed this method rebakes the normals of the mesh.
    /// </summary>
    private void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    /// <summary>
    /// Makes sure normals are not calculated for each triangle and their vertices to be the same, causing the flatshading effect.
    /// </summary>
    private void FlatShading()
    {
        // Fill arrays with the already defined triangles.
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            // Add the newly created flatshaded values to the triangles array.
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    #endregion Normals



    #region Processing

    /// <summary>
    /// Calculate the normals depending on if flatshading should be applied.
    /// </summary>
    public void ProcessMesh()
    {
        if (useFlatShading)
            FlatShading();
        else
            BakeNormals();
    }

    /// <summary>
    /// Initializes the data saved in this class and creates a mesh out of it.
    /// </summary>
    /// <returns></returns> The generated mesh.
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        if (useFlatShading)
            mesh.RecalculateNormals();
        else
            mesh.normals = bakedNormals;

        return mesh;
    }

    #endregion Processing

    #endregion Methods

}