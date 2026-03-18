using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Tool", menuName = "Scriptable Objects/New Tool/Explosion Tool")]
public class ExpolsionTool : DestructionTool
{
    //grenade? - maybe branch into throwable explosives and bomb placing? 

    [Header("Explosion Tool Fields")]
    [SerializeField] private float throwDistance;
    [SerializeField] private float throwSpeed;
}
