using UnityEngine;

public class RegenMeshTarget : DestructableBehaviour
{
    [SerializeField] private TerrainManager targetTerrain;
    public override DestructionLayer GetLayer => DestructionLayer.VoxelWall;

    [SerializeField] private float cooldown;
    private float cdTimer;

    [SerializeField]private bool canReset;

    public override void InitializeDestruction()
    {
        base.InitializeDestruction();

        canReset = true;

        cdTimer = 0;

        cooldown = Mathf.Max(cooldown, 2f); // cooldown > 2 
    }
    public override void ApplyDamage(DestructionHitData hitData)
    {
        base.ApplyDamage(hitData);
        if (canReset)
        {
            RegenerateMesh();

        }
    }

    private void RegenerateMesh()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Assign a target terrain to use a regen mesh target");
            return;
        }

        targetTerrain.RegenerateTerrain();
    }

    public override void UpdateDestruction()
    {
        base.UpdateDestruction();

        if (!canReset)
        {
            cdTimer += Time.deltaTime;
            if (cdTimer >= cooldown)
            {
                canReset = true;
                cdTimer = 0f;
            }
        }
    }
}
