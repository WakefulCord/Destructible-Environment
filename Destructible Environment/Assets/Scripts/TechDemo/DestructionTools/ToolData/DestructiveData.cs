using UnityEngine;


//might not be needed - wait till 1 of each tool is done first
public class DestructiveData : ScriptableObject // stores damage and radius
{
    [Header("Destructive Data Fields")]
    [SerializeField] private float destructionDamage;
    [SerializeField] private float destructionRadius;

    public float GetDamage => destructionDamage;
    public float GetRadius => destructionRadius;
    
}

