using System.Collections.Generic;
using UnityEngine;

public class HVoxelHousePart : DestructableBehaviour
{
    HVoxelHouse manager;
    
    [SerializeField] private int voxelCountX; 
    [SerializeField] private int voxelCountY;
    [SerializeField] private int voxelCountZ;
    [SerializeField] private HousePartType partType;
    [SerializeField] private float cellSize;

    [SerializeField] private List<HVoxel> ownedVoxels = new List<HVoxel>();

    [SerializeField] private float debrisChance = 0.25f;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private float debrisMaxLife = 1f;


    private struct DebrisData
    {
        public GameObject debrisGO;
        public float lifeTime;
    }
    List<DebrisData> activeDebris = new List<DebrisData>();
    public override DestructionLayer GetLayer => DestructionLayer.VoxelWall;

    
    public void Init(HVoxelHouse manager, int width, int height, int depth, HousePartType partType, float cellSize)
    {
        this.manager = manager;

        this.voxelCountX = width;
        this.voxelCountY = height;
        this.voxelCountZ = depth;
        this.partType = partType;
        this.cellSize = cellSize;


        activeDebris = new List<DebrisData>();
        
    }

    public override void UpdateDestruction()
    {
        base.UpdateDestruction();

        if (activeDebris != null && activeDebris.Count > 0)
        {
            List<DebrisData> toRemove = new List<DebrisData>();
            foreach (DebrisData debris in activeDebris)
            {
                if (debris.lifeTime <= Time.time)
                {
                    toRemove.Add(debris);
                }
            }


            foreach (DebrisData debris in toRemove)
            {
                activeDebris.Remove(debris);
                Destroy(debris.debrisGO);
            }

        }
    }
    public override void InitializeDestruction()
    {
        base.InitializeDestruction();
    }

    public override void ApplyDamage(DestructionHitData hitData)
    {
        base.ApplyDamage(hitData);


        float radiusOverDistance = hitData.radius;
        foreach (Collider collider in Physics.OverlapSphere(hitData.hitPoint, radiusOverDistance))    //breaks each voxel in a radius
        {
            Debug.Log($"{collider.gameObject.name }");
            if (collider.GetComponent<HVoxel>() != null)
            {
                Destroy(collider.gameObject);
                TrySpawnDebris(collider.transform.position);
            }
                    
                    //collider.GetComponent<HVoxel>().breakVoxel();
               
        }
    }

    private void TrySpawnDebris(Vector3 spawnPos)
    {
        if (Random.value < debrisChance)
        {
            SpawnDebris(spawnPos);
        }
    }

    private void SpawnDebris(Vector3 spawnPos)
    {
        GameObject debris  = Instantiate(debrisPrefab, spawnPos, Quaternion.identity); // handles its own despawn for now
        debris.transform.localScale = Vector3.one * cellSize;
        activeDebris.Add(new DebrisData
        {
            debrisGO= debris,
            lifeTime = Time.time + debrisMaxLife,
        });
    }



    public void CreateHousePart()
    {
        ownedVoxels.Clear();
      

        
        GenerateBoxPart();
    }

    private void GenerateBoxPart() // spawns voxels in grid
    {
        if (manager.VoxelPrefab == null)
        {
            Debug.LogWarning($"No voxel prefab assigned for part {name}", this);
            return;
        }

        for (int x = 0; x < voxelCountX; x++)
        {
            for (int y = 0; y < voxelCountY; y++)
            {
                for (int z = 0; z < voxelCountZ; z++)
                {
                    CreateVoxel(x, y, z);
                }
            }
        }
    }

    private void CreateVoxel(int x, int y, int z)
    {
        GameObject voxelGO = Instantiate(manager.VoxelPrefab, transform);
        HVoxel voxel = voxelGO.GetComponent<HVoxel>();
        voxel.transform.localPosition = GetVoxelWorldPos(x,y,z);
        voxel.transform.localScale = Vector3.one * cellSize;
        voxel.Init(x, y, z);
        ownedVoxels.Add(voxel);
    }

    
    private Vector3 GetVoxelWorldPos(int x,int y, int z)
    {
        

        return new Vector3(x * cellSize, y * cellSize, z * cellSize);
    }

    private Vector3 GetVoxelIndexFromWorld(Vector3 worldPos)
    {
        return worldPos / cellSize;
    }
}
