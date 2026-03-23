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

    // OnExplosiveUpdate is called once per frame
    void Update()
    {

    }

}
