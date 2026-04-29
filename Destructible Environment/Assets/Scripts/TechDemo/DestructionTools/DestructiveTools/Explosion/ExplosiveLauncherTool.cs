using UnityEngine;

[CreateAssetMenu(fileName = "Explosive Launcher Tool", menuName = "Scriptable Objects/Tools/Explosive Tool/Explosive Launcher Tool")]

public class ExplosiveLauncherTool : ExplosionTool // rpg/ grenade launcher/ throwable explosion
{
    [Header("---Explosion Launcher Tool---")]
    [Header("Launcher Settings")]
    [Range(10,60)][Tooltip("Speed at which launcher shoots projectile")][SerializeField] private float launchSpeed;


    public float LaunchSpeed => launchSpeed;
}
