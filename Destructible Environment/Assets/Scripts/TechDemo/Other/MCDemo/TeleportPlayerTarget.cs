using UnityEngine;

public class TeleportPlayerTarget : DestructableBehaviour
{
    public override DestructionLayer GetLayer => DestructionLayer.VoxelWall;

    PlayerManager playerManager;
    private Transform teleportTransfrom;

    public override void InitializeDestruction()
    {
        base.InitializeDestruction();
        playerManager = PlayerManager.Instance;

        teleportTransfrom = GetComponentInParent<TerrainManager>().PlayerTeleportPos; 
    }

    public override void ApplyDamage(DestructionHitData hitData)
    {
        base.ApplyDamage(hitData);

        playerManager.TeleportPlayer(teleportTransfrom);
    }
}
