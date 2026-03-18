using UnityEngine;
[CreateAssetMenu(fileName = "Gun Tool", menuName = "Scriptable Objects/New Tool/Gun Tool")]

public class GunTool : DestructionTool
{
    [Header("Gun Tool Fields")]
    [SerializeField] private float fireRate;

    public float FireRate => fireRate;

    
}
