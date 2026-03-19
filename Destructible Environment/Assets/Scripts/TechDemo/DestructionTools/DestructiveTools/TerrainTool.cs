using UnityEngine;
[CreateAssetMenu(fileName = "Terrain Tool", menuName = "Scriptable Objects/New Tool/Terrain Tool")]

public class TerrainTool : DestructionTool
{
    [Header("Terrain Tool Fields")]
    [SerializeField] private GameObject indicatorPrefab;

    public GameObject GetIndicatorPrefab => indicatorPrefab;

    public TerraformData GetTerraData => (TerraformData)GetData;
}
