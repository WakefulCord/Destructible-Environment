using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    TerrainManager terrainManager; 

    //mesh fields - need to build a mesh/collision
    List<Vector3> meshVerts;
    List<int> meshTris;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    Vector3[] edgeVertex = new Vector3[12];

    //fields from terrainmanager cached
    int chunkSize; 
    Vector3Int chunkPos;

    //8 corner points for marching cube
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

    public void Initialise(TerrainManager manager, Vector3Int pos, int size)
    {
        //cache reused values
        terrainManager = manager;
        chunkPos = pos;
        chunkSize = size;
        
        //set up components
        meshVerts = new List<Vector3>();
        meshTris = new List<int>();

        meshFilter = GetComponent<MeshFilter>();
        
        meshRenderer = GetComponent<MeshRenderer>();
        
        meshCollider = GetComponent<MeshCollider>();
        
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        meshVerts.Clear();
        meshTris.Clear();


        //for each density grid point in the chunk
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    MarchCube(x, y, z); // create triangles and vertices based on density value
                }
            }
        }

        BuildMesh();
    }

    private void BuildMesh()
    {
        //apply calculated mesh values 

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(meshVerts);
        mesh.SetTriangles(meshTris, 0);
        mesh.RecalculateNormals();

        if (terrainManager.TerrainGradient != null) // apply colour gradient to terrain for fun
        {
            Color[] colours = new Color[meshVerts.Count]; // colour each vert

            float globalMinHeight = terrainManager.GridOrigin.y;
            float globalMaxHeight = terrainManager.GridOrigin.y + terrainManager.TerrainHeight;

            for (int i = 0; i < meshVerts.Count; i++)
            {
                float gridY = meshVerts[i].y + chunkPos.y * chunkSize;
                float t = Mathf.InverseLerp(globalMinHeight, globalMaxHeight, gridY);

                t *= 2f;
                t = Mathf.Clamp01(t);

                colours[i] = terrainManager.TerrainGradient.Evaluate(t);
            }

            mesh.SetColors(colours);
        }

        meshFilter.sharedMesh = mesh;
        //set collider    
        meshCollider.sharedMesh = mesh;
    }
    private void MarchCube(int x, int y, int z)
    {
        Vector3Int localPos = new Vector3Int(x, y, z); 

        //get each denisty value from 8 corners of cube
        float[] cubeCorners = new float[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3Int localCorner = localPos + new Vector3Int(
                (int)cornerOffset[i].x,
                (int)cornerOffset[i].y,
                (int)cornerOffset[i].z
            );

            int worldX = chunkPos.x * chunkSize + localCorner.x;
            int worldY = chunkPos.y * chunkSize + localCorner.y;
            int worldZ = chunkPos.z * chunkSize + localCorner.z;

            cubeCorners[i] = terrainManager.GetDensity(worldX, worldY, worldZ);
        }

        //get caseindex for lookup table from density values
        int caseIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > terrainManager.GetIsoLevel)
                caseIndex |= (1 << i); // set bit if corner is inside surface
        }

        if (caseIndex == 0 || caseIndex == 255) // both incides mean empty - 0 all outside , 255 all inside(no surface)
            return;



        //calc where the edge of triangle should be placed along grid
        int edgeMask = MCLookUp.edgeTable[caseIndex];

        for (int e = 0; e < 12; e++)
        {
            if ((edgeMask & (1 << e)) == 0)
                continue;

            //edge points
            int cornerA = MCLookUp.cornerIndexAFromEdge[e];
            int cornerB = MCLookUp.cornerIndexBFromEdge[e];

            //position of corners
            Vector3 posA = localPos + cornerOffset[cornerA];
            Vector3 posB = localPos + cornerOffset[cornerB];

            //denisty at corners
            float valueA = cubeCorners[cornerA];
            float valueB = cubeCorners[cornerB];

            edgeVertex[e] = Interpolate(posA, posB, valueA, valueB); // position of mesh vertex based on density values of points at either side of vertex
        }


        //get triangles and verts from lookup table based on case index
        int[] triangles = MCLookUp.triangulation[caseIndex];
        for (int i = 0; i < 15; i += 3)
        {
            if (triangles[i] == -1)
                break;

            int triIndex = meshVerts.Count;

            //add vertrex poistion to mesh verts
            meshVerts.Add(edgeVertex[triangles[i]]);
            meshVerts.Add(edgeVertex[triangles[i + 1]]);
            meshVerts.Add(edgeVertex[triangles[i + 2]]);

            //triangles wound in 0,2,1 order to flip face normals out
            meshTris.Add(triIndex);
            meshTris.Add(triIndex + 2);
            meshTris.Add(triIndex + 1);
        }


    }

    private Vector3 Interpolate(Vector3 p1, Vector3 p2, float v1, float v2) // linear interpolation, returns where the vertex should be along the edge
    {
        float t = (terrainManager.GetIsoLevel - v1) / (v2 - v1);
        t = Mathf.Clamp01(t);
        return p1 + (p2 - p1) * t;
    }

    public void RegenerateMesh()
    {
        GenerateMesh();
    }
}
