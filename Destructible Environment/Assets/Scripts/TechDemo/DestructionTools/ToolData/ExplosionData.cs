using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Data", menuName = "Scriptable Objects/New Destruction Data/Explosion Data")]

public class ExplosionData : DestructiveData
{
    [Header("Explosion Fields")]
    [SerializeField] private float explosionRadius;
}
