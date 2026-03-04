using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour // main script for handling marching cubes terrain
{
    #region Class References
    private static TerrainManager _instance;

    GridManager gridManager;
    #endregion

    #region Private Fields
    [Header("Terrain Fields")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int depth;


    [SerializeField] private float isoLevel = 0f; 
    [SerializeField] private float[,,] densityGrid;

    [SerializeField] private static float seed;

    private List<Vector3> meshVerts;
    private List<int> meshTris;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    // For storing edge interpolation results
    private Vector3[] edgeVertex = new Vector3[12];

    [SerializeField] private float noiseVal = 0.05f;
    #endregion

    #region Properties
    public static TerrainManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TerrainManager>();
                if (_instance == null )
                {
                    Debug.LogError("Terrain Manager has not been assigned");
                }
            }
            return _instance;

        }
    }


    
    #endregion

    #region Start Up
    private void Start()
    {
        gridManager = GetComponent<GridManager>();

        densityGrid = new float[width, height, depth]; // initialise density grid 

        seed = UnityEngine.Random.Range(0f, 9999f); //for noise
        
        InitaliseTerrainValues(); // set noise

        GenerateMesh();  // create mesh (march Cubes)

        //gridManager.VisualiseNoise(densityGrid);
    }

    private void GenerateMesh()
    {
        meshVerts = new List<Vector3>();
        meshTris = new List<int>();

        for (int x = 0; x < width - 1; x++)
            for (int y = 0; y < height - 1; y++)
                for (int z = 0; z < depth - 1; z++)
                {
                    MarchCube(new Vector3Int(x, y, z));
                }

        BuildMesh();
    }

   
    private static readonly Vector3[] cornerOffset =
    {
    new Vector3(0,0,0),
    new Vector3(1,0,0),
    new Vector3(1,1,0),
    new Vector3(0,1,0),
    new Vector3(0,0,1),
    new Vector3(1,0,1),
    new Vector3(1,1,1),
    new Vector3(0,1,1)
};


    private void MarchCube(Vector3Int pos) // from sebastian lague terraforming video 
    {
      
        //Get corner density values for 8 corners of the cube
        float[] cubeCorners = new float[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3Int corner = pos + new Vector3Int(
                (int)cornerOffset[i].x,
                (int)cornerOffset[i].y,
                (int)cornerOffset[i].z
            );
            cubeCorners[i] = densityGrid[corner.x, corner.y, corner.z];
        }

        //get caseindex for lookup table from density values
        int caseIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > isoLevel)
                caseIndex |= (1 << i);
        }

        if (caseIndex == 0 || caseIndex == 255)
            return;

    
        int edgeMask = MCLookUp.edgeTable[caseIndex]; //where the edge is

       
        for (int e = 0; e < 12; e++) //calc where triangle should be placed based on corner values
        {
            if ((edgeMask & (1 << e)) == 0)
                continue;

            //edge points
            int cornerA = MCLookUp.cornerIndexAFromEdge[e];
            int cornerB = MCLookUp.cornerIndexBFromEdge[e];

            //Get world pos of the two corners of this edge 
            Vector3 posA = pos + cornerOffset[cornerA];
            Vector3 posB = pos + cornerOffset[cornerB];

            //density grid values 
            float valueA = cubeCorners[cornerA];
            float valueB = cubeCorners[cornerB];

            edgeVertex[e] = Interpolate(posA, posB, valueA, valueB); // important for smooth terrain, edge position is influenced by density grid values
        }

       
        int[] triangles = MCLookUp.triangulation[caseIndex];
        for (int i = 0; i < 15; i += 3)
        {
            if (triangles[i] == -1)
                break;

            int triIndex = meshVerts.Count;

            meshVerts.Add(edgeVertex[triangles[i]]);
            meshVerts.Add(edgeVertex[triangles[i + 1]]);
            meshVerts.Add(edgeVertex[triangles[i + 2]]);

           
            meshTris.Add(triIndex);
            meshTris.Add(triIndex + 2);
            meshTris.Add(triIndex + 1);
        }
    }
    private Vector3 Interpolate(Vector3 p1, Vector3 p2, float v1, float v2)
    {
        float t = (isoLevel - v1) / (v2 - v1);
        t = Mathf.Clamp01(t); 
        return p1 + (p2 - p1) * t;
    }   
    private void InitaliseTerrainValues()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    densityGrid[x, y, z] = TerrainDensity(x, y, z);
                }
    }

    public float TerrainDensity(int x, int y, int z) // not made by me
    {
        float heightMap = Mathf.PerlinNoise((x + seed) * noiseVal, (z + seed) * noiseVal) * height * 0.4f;


        float distanceFromSurface = heightMap - y;


        float noise3D = Mathf.PerlinNoise(x * noiseVal + seed, y * noiseVal) * 
                        Mathf.PerlinNoise(z * noiseVal + seed, y * noiseVal + 100f);
        noise3D = (noise3D - 0.5f) * 2f; 

       
        float density = distanceFromSurface + noise3D * 3f;

        return density;
    }

    #endregion

    #region Class Methods


    private void BuildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(meshVerts);
        mesh.SetTriangles(meshTris, 0);
        mesh.RecalculateNormals();

       
        Vector3 center = new Vector3(width * 0.5f, height * 0.5f, depth * 0.5f);
        Vector3 size = new Vector3(width, height, depth);
        mesh.bounds = new Bounds(center, size);

        meshFilter.sharedMesh = mesh;

        Debug.Log("Mesh built successfully - Verts: " + meshVerts.Count + ", Tris: " + (meshTris.Count / 3));

        MeshCollider meshCol = GetComponent<MeshCollider>();

        if (meshCol != null)
        {
            meshCol.sharedMesh = mesh;
            Debug.Log("Mesh Collider Set");
        }
    }

    #endregion


    #region Terraform Methods
    public void AddDensity(int x, int y, int z, float amount)
    {
        if (IsInBounds(x, y, z))
        {
            densityGrid[x, y, z] += amount;
        }
    }

    public void SubtractDensity(int x, int y, int z, float amount)
    {
        if (IsInBounds(x, y, z))
        {
            densityGrid[x, y, z] -= amount;
        }
    }

    public void RegenerateMesh()
    {
        GenerateMesh();
    }
    #endregion


    #region Helpers
    public bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < width &&
        y >= 0 && y < height &&
        z >= 0 && z < depth;

    }
    #endregion
}
