using UnityEngine;
[CreateAssetMenu(fileName = "Terraform Data", menuName = "Scriptable Objects/New Destruction Data/Terraform Data")]

public class TerraformData : DestructiveData
{
    [Header("Terraform Data")]
    [SerializeField] private float terraformRange;
    [SerializeField] private float brushSize;
  
}
