using System.Collections.Generic;
using UnityEngine;

public class ExplosivePlantBehaviour : ToolBehaviour
{

    private readonly List<ExplosiveBehaviour> plantedExplosives = new List<ExplosiveBehaviour>(); // inactive explosives that have been planted but not yet detonated
    private readonly List<ExplosiveBehaviour> activeExplosives = new List<ExplosiveBehaviour>(); // explosives that have been detonated/ fuse has been lit but not yet exploded


    public ExplosivePlantTool GetExpPlantTool => (ExplosivePlantTool)GetToolData;

    public override void OnToolInit(DestructionTool t, Camera playerCam)
    {
        base.OnToolInit(t, playerCam);
    }

    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

        for (int i = activeExplosives.Count - 1; i >= 0; i--) // loop backwards to remove safely
        {
            ExplosiveBehaviour explosive = activeExplosives[i];

            if (explosive == null)
            {
                activeExplosives.RemoveAt(i);
                continue;
            }

            explosive.OnExplosiveUpdate();
        }
    }

    protected override void PrimaryUseBehaviour()
    {
        base.PrimaryUseBehaviour();

        SpawnBomb();

        

    }

    private void SpawnBomb()
    {
        ExplosivePlacementData placementData = GetExplosivePlacementData();

        //spawn explosive

        Vector3 spawnPos = placementData.hitPoint + placementData.hitNormal * 0.05f;
        Quaternion rot = Quaternion.LookRotation(placementData.hitNormal);


        GameObject bomb = Instantiate(GetExpPlantTool.ExplosivePrefab, spawnPos, rot);

        ExplosiveBehaviour explosive = bomb.GetComponent<ExplosiveBehaviour>();

        //configure explosive based on trigger mode
        if (explosive == null)
        {
            Debug.LogError("Spawned explosive prefab does not have an ExplosiveBehaviour component!");
            return;
        }
        if (GetTriggerMode() == PlantedExplosiveTriggerMode.Fuse)
        {
            ConfigureExplosive_Fuse(explosive, GetExpPlantTool.FuseTimer);
        }
        else if (GetTriggerMode() == PlantedExplosiveTriggerMode.RemoteDetonator)
        {
            ConfigureExplosive_Remote(explosive);
        }
    }

    protected override void SecondaryUseBehaviour()
    {
        base.SecondaryUseBehaviour();
        DetonateAll();
    }

    private void ConfigureExplosive_Fuse(ExplosiveBehaviour explosive, float fuseTime)
    {
        explosive.Exploded += HandleExplosiveExploded;
        explosive.OnFuseExplosiveInit(GetExpPlantTool, fuseTime);
        AddActiveExplosion(explosive);
    }

    private void ConfigureExplosive_Remote(ExplosiveBehaviour explosive)
    {
        explosive.Exploded += HandleExplosiveExploded; // remove from list when exploded

        explosive.OnRemoteExplosiveInit(GetExpPlantTool);
        plantedExplosives.Add(explosive); // adds explosive to inactive list
    }

    private void AddActiveExplosion(ExplosiveBehaviour explosive) // starts an explosion
    {
        plantedExplosives.Remove(explosive);
        activeExplosives.Remove(explosive);


        activeExplosives.Add(explosive);
        explosive.ActivateExplosive();
    }

    private struct ExplosivePlacementData
    {
        public Vector3 hitPoint;
        public Vector3 hitNormal;
    }
    private ExplosivePlacementData GetExplosivePlacementData()
    {
        RaycastHit hit;
        if (!Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, Mathf.Infinity))
            return new ExplosivePlacementData();

        ExplosivePlacementData data = new ExplosivePlacementData
        {
            hitPoint = hit.point,
            hitNormal = hit.normal
        };
        return data;
    }



    // Detonates all planted explosives (for remote detonator mode)
    private void DetonateAll()
    {

        for (int i = plantedExplosives.Count - 1; i >= 0; i--)
        {
            if (plantedExplosives[i] != null)
            {
                AddActiveExplosion(plantedExplosives[i]);
            }
        }
        plantedExplosives.Clear();
    }

    private void HandleExplosiveExploded(ExplosiveBehaviour explosive)
    {
        if (explosive == null)
        {
            return;
        }

        explosive.Exploded -= HandleExplosiveExploded;
        plantedExplosives.Remove(explosive);
        activeExplosives.Remove(explosive);
    }

    public PlantedExplosiveTriggerMode GetTriggerMode()
    {
        if (GetExpPlantTool == null)
        {
            Debug.Log("No Tool Data - defaulting to remote detonator");
            return PlantedExplosiveTriggerMode.RemoteDetonator;//default
        }
        return GetExpPlantTool.ExplosiveTriggerMode;

    }
}
