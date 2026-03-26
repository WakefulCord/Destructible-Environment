using UnityEngine;
[CreateAssetMenu(fileName = "Terraform Data", menuName = "Scriptable Objects/New Destruction Data/Terraform Data")]

public class TerraformData : DestructiveData
{
    [Header("Terraform Data")]
    [SerializeField] private float terraformRange;
   
    [SerializeField] private float terraformCooldown = 0.1f; //might not ever need to change? could be used for having bigger impact hits


    public float GetTerraRange => terraformRange;
    public float GetTerraCooldown => terraformCooldown;
}
