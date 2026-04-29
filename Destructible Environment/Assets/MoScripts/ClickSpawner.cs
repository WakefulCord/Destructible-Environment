using UnityEngine;

public class ClickSpawner : MonoBehaviour
{
    public Cubes cubesPrefab;   
    public LayerMask groundMask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnMeshAtClick();
        }
    }

    void SpawnMeshAtClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
        {
            // Spawn at hit point
            Cubes c = Instantiate(cubesPrefab, hit.point, Quaternion.identity);

            // Build the mesh
          //  c.BuildMesh();
        }
    }
}