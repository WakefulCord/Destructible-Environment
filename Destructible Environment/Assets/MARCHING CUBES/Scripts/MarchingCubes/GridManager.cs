using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject testGO;
    [SerializeField] private float cellSpacing = 1f;

    [SerializeField] private Gradient noiseGradient;
    public void VisualiseNoise(float[,,] densityGrid)
    {
        for (int x = 0; x < densityGrid.GetLength(0); x++)
            for (int y = 0; y < densityGrid.GetLength(1); y++)
                for (int z = 0; z < densityGrid.GetLength(2); z++)
                {
                    Vector3 pos = new Vector3(x + cellSpacing, y + cellSpacing, z + cellSpacing);
                    GameObject cellGO = Instantiate(testGO, pos, Quaternion.identity);

                    MeshRenderer cellMesh = cellGO.GetComponent<MeshRenderer>();

                    float t = densityGrid[x, y, z];
                    cellMesh.material.color = noiseGradient.Evaluate(t);
                }
    }
}
