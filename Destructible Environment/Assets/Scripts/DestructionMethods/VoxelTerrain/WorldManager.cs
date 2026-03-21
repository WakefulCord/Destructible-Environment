using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;

    [SerializeField] float scale = 0.1f;
    [SerializeField] float maxHeight = 16f;
    [SerializeField] int worldSize = 32;


    // Start is called before the first frame update
    void Start()
    {
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(worldMaterial, Vector3.zero);
        

        

        for (int x = 0; x < worldSize; x++) //number of iterations = world size
        {
            for (int z = 0; z < worldSize; z++)
            {
                float noise = Mathf.PerlinNoise(x * scale, z * scale);
                int height = Mathf.FloorToInt(noise * maxHeight);

                for (int y = 0; y < height; y++)
                {
                    container[new Vector3(x, y, z)] = new Voxel() { ID = 1 };
                }
            }
        }

        container.GenerateMesh();
        container.UploadMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateBlock(Vector3Int pos, byte id)
    {
        if (id == 0)
            container[pos] = new Voxel() { ID = 0 };
        else
            container[pos] = new Voxel() { ID = id };

        container.GenerateMesh();
        container.UploadMesh();
    }

    public void CreateBlock(Vector3 pos, byte id)
    {
        CreateBlock(WorldToVoxel(pos), id);
    }

    public void DestroyBlock(Vector3Int pos)
    {
        CreateBlock(pos, 0);
    }

    public void DestroyBlock(Vector3 pos)
    {
        CreateBlock(WorldToVoxel(pos), 0);
    }

    Vector3Int WorldToVoxel(Vector3 worldPos) //helper function to convert world coords to voxel ints
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z)
        );
    }

    public void DestroyMultiple(Vector3 worldPos, int radius) //Destroy blocks in a circle around coords
    {
        Vector3Int centerVoxel = Vector3Int.FloorToInt(worldPos);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    Vector3Int pos = centerVoxel + offset;

                    // Only destroy blocks within spherical radius
                    if (offset.magnitude <= radius)
                    {
                        container[pos] = new Voxel() { ID = 0 };
                    }
                }
            }
        }
        container.GenerateMesh();
        container.UploadMesh();
    }

}
