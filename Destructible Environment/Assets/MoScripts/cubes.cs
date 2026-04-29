using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Cubes : MonoBehaviour
{
    [SerializeField] private int width = 25;
    [SerializeField] private int height = 19;

    [SerializeField] float resolution = 0.5f;
    [SerializeField] float noiseScale = 0.07f;

    [SerializeField] private float isoLevel = 0.5f;

    [SerializeField] bool visualizeNoise;
    [SerializeField] bool use3DNoise;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float[,,] density;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateDensity();
        GenerateMesh();
    }

    void GenerateDensity()
    {
        density = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x <= width; x++)
            for (int y = 0; y <= height; y++)
                for (int z = 0; z <= width; z++)
                {
                    if (use3DNoise)
                    {
                        float n = PerlinNoise3D(
                            x * noiseScale,
                            y * noiseScale,
                            z * noiseScale
                        );

                        density[x, y, z] = n;
                    }
                    else
                    {
                        float h = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * height;
                        density[x, y, z] = y < h ? 0f : 1f;
                    }
                }
    }

    void GenerateMesh()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < width; z++)
                {
                    float[] cube = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int c = new Vector3Int(x, y, z) + marching.Corners[i];
                        cube[i] = density[c.x, c.y, c.z];
                    }

                    MarchCube(new Vector3(x, y, z), cube);
                }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    //public void BuildMesh()
   // {
     //   GenerateDensity();
      //  GenerateMesh();
    //}

    void MarchCube(Vector3 pos, float[] cube)
    {
        int config = 0;

        for (int i = 0; i < 8; i++)
            if (cube[i] > isoLevel)
                config |= 1 << i;

        if (config == 0 || config == 255)
            return;

        for (int i = 0; marching.Triangles[config, i] != -1; i++)
        {
            int edge = marching.Triangles[config, i];

            Vector3 p1 = pos + marching.Edges[edge, 0];
            Vector3 p2 = pos + marching.Edges[edge, 1];

            Vector3 v = (p1 + p2) * 0.5f * resolution;

            vertices.Add(v);
            triangles.Add(vertices.Count - 1);
        }
    }

    float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || density == null)
            return;

        for (int x = 0; x <= width; x++)
            for (int y = 0; y <= height; y++)
                for (int z = 0; z <= width; z++)
                {
                    float d = density[x, y, z];
                    Gizmos.color = new Color(d, d, d, 1);
                    Gizmos.DrawSphere(new Vector3(x, y, z) * resolution, 0.1f);
                }
    }
}