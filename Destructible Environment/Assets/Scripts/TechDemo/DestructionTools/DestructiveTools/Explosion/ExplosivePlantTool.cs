using UnityEngine;

[CreateAssetMenu(fileName = "Explosive Plant Tool", menuName = "Scriptable Objects/Tools/Explosive Tool/Explosive Plant Tool")]

public class ExplosivePlantTool : ExplosionTool // place/ detonate
{
    [Header("---Explosive Plant Tool---")]
    [SerializeField] private PlantedExplosiveTriggerMode explosiveTriggerMode;

    [Header("Fuse Settings")]
    [SerializeField] private float fuseTimer;

    public PlantedExplosiveTriggerMode ExplosiveTriggerMode => explosiveTriggerMode;
    public float FuseTimer => fuseTimer;
}

public enum PlantedExplosiveTriggerMode
{
    Fuse, // place explosive that has a fuse timer then explodes
    RemoteDetonator, // place explosive to remote detonate 
}
    

