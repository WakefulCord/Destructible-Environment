using UnityEngine;

[CreateAssetMenu(fileName = "Terrain Tool", menuName = "Scriptable Objects/Tools/Terrain Tool")]
public class TerrainTool : DestructionTool
{
    [Header("---Terrain Tool---")]
    [Header("Indicator Fields")]
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Terratool Stats")]
    [SerializeField] private float terraformRange;



    //destruction feedback ref
    public GameObject GetIndicatorPrefab => indicatorPrefab;
    public float GetTerraRange => terraformRange;
    public TerraformFeedback GetTerraformFeedback => (TerraformFeedback)GetDestructionFeedback;

}
