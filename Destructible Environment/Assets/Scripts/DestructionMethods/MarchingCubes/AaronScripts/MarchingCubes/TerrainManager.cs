using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : DestructableBehaviour // main script for handling marching cubes terrain
{
    #region Class References
   

    GameManager gameManager;
    GridManager gridManager;
    #endregion

    #region Private Fields
    [Header("Chunk Fields")]
    [SerializeField] private GameObject terrainChunkPrefab;
    [SerializeField] private Material chunkMaterial;
    Dictionary<Vector3Int, TerrainChunk> chunks;
    [SerializeField] int chunkSize = 16;

    [Header("Terrain Fields")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int depth;
    [SerializeField] private float[,,] densityGrid;
    private int worldChunksX;
    private int worldChunksZ;
    private int worldChunksY;
    [SerializeField] private float isoLevel = 0f;
    [SerializeField] private float terrainYOffset = 0f;

    private Vector3 gridOrigin;

    [Header("Visual/Noise Fields")]
    [SerializeField] private float seed = 1234f;
    [SerializeField] private float noiseVal = 0.05f;

    [SerializeField] float surfaceHeightAmplitude = 0.1f;
    [SerializeField] float surfaceDetailNoiseOffset = 100f;
    [SerializeField] float surfaceDetailStrength = 0.01f;
    [SerializeField] private Gradient terrainGradient;

    [Header("Dig Site Fields")]
    [SerializeField] private GameObject exitTargetPrefab;

    [SerializeField] private List<GameObject> exitTargets;
    [SerializeField] private int maxExitTargets = 3;
    [SerializeField] private float exitTargetSpawnOffset = 5;
    [SerializeField] private Transform teleportTransfrom;



    #endregion

    #region Properties


    public float GetIsoLevel => isoLevel;
    public Gradient TerrainGradient => terrainGradient;
    public Material ChunkMaterial => chunkMaterial;
    public int TerrainHeight => height;
    public float AverageSurfaceHeight => GetAverageSurfaceHeight();

    public override DestructionLayer GetLayer => DestructionLayer.MarchingCubes;

    public Transform PlayerTeleportPos => teleportTransfrom;
    #endregion

    #region Start Up
    public override void InitializeDestruction()
    {
        base.InitializeDestruction();
        OnStart();
    }
    private void OnStart()
    {
        gridManager = GetComponent<GridManager>();
        gameManager = GameManager.Instance;

        //initilise fields
        chunks = new Dictionary<Vector3Int, TerrainChunk>();

        densityGrid = new float[width, height, depth]; // initialise density grid 

    

        worldChunksX = width / chunkSize;
        worldChunksZ = depth / chunkSize;
        worldChunksY = height / chunkSize;
        
        SetGridOrigin();


       
        RegenerateTerrain();

        //gridManager.VisualiseNoise(densityGrid);
    }

    private void SetGridOrigin()
    {
        gridOrigin = new Vector3(-width / 2f, -height / 2f, -depth / 2f);

    }

    private void InitaliseTerrainValues()
    {
        //for each densitygrid point
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    densityGrid[x, y, z] = TerrainDensity(x, y, z); // create perlin based noise
                }
    }

    public float TerrainDensity(int x, int y, int z)
    {
       

        float baseSurfaceHeight =
            Mathf.PerlinNoise((x + seed) * noiseVal, (z + seed) * noiseVal) * height * surfaceHeightAmplitude;

        baseSurfaceHeight += terrainYOffset;

        float distanceFromSurface = baseSurfaceHeight - y;

        float surfaceDetailNoise =
            Mathf.PerlinNoise(x * noiseVal + seed, y * noiseVal) *
            Mathf.PerlinNoise(z * noiseVal + seed, y * noiseVal + surfaceDetailNoiseOffset);

        surfaceDetailNoise = (surfaceDetailNoise - 0.5f) * 2f;

        float density = distanceFromSurface + surfaceDetailNoise * surfaceDetailStrength;

        return density;
    }

    #endregion

    #region Chunk Methods

    public override void UpdateDestruction()
    {
        base.UpdateDestruction();

        foreach (TerrainChunk chunk in chunks.Values)
        {
            if (chunk.IsDirty)
            {
                chunk.RegenerateMesh();
                chunk.SetIsDirty(false);
            }
        }
    }
    private void GenerateChunks() // creates chunks for terrain
    {

        for (int x = 0; x < worldChunksX; x++)
        {
            for (int y = 0; y < worldChunksY; y++)
            {
                for (int z = 0; z < worldChunksZ; z++)
                {
                    CreateChunk(new Vector3Int(x, y, z));
                }
            }
        }

    }

    private void CreateChunk(Vector3Int chunkPos)
    {
        if (terrainChunkPrefab == null)
        {
            Debug.LogError("Terrain Chunk Prefab is not assigned");
            return;
        }

        //create chunk object
        GameObject chunkObj = Instantiate(terrainChunkPrefab, transform);
        chunkObj.name = $"Chunk_{chunkPos.x}_{chunkPos.y}_{chunkPos.z}"; // name it its chunk pos

        TerrainChunk chunk = chunkObj.GetComponent<TerrainChunk>();



        chunk.transform.localPosition = gridOrigin + new Vector3(
            chunkPos.x * chunkSize,
            chunkPos.y * chunkSize,
            chunkPos.z * chunkSize
        );

        if (chunkMaterial != null)
        {
            MeshRenderer renderer = chunkObj.GetComponent<MeshRenderer>();

            renderer.sharedMaterial = chunkMaterial;
        }

        chunk.Initialise(this, chunkPos, chunkSize);

        chunks.Add(chunkPos, chunk);
    }
    public TerrainChunk GetChunkFromGridPos(Vector3 gridPos) // convert grid pos to closest chunk for terraform
    {
        Vector3Int chunkCoord = new Vector3Int(
           Mathf.FloorToInt(gridPos.x / chunkSize),
           Mathf.FloorToInt(gridPos.y / chunkSize),
           Mathf.FloorToInt(gridPos.z / chunkSize)
       );

        if (chunks.TryGetValue(chunkCoord, out TerrainChunk chunk))
            return chunk;

        return null;
    }

    public void RegenerateChunk(Vector3 gridPos) // rebuulds chunk after deformation 
    {
        TerrainChunk chunk = GetChunkFromGridPos(gridPos);
        if (chunk != null)
        {
            chunk.RegenerateMesh();
        }
    }
    #endregion


    #region Terraform Methods
    public float GetDensity(int x, int y, int z) // get desnity at x,y,z from density grid 
    {
        if (IsInBounds(x, y, z))
        {
            return densityGrid[x, y, z];
        }
        return 0f;
    }

    public void UpdateDensity(int x, int y, int z, float amount) // used fro terraform
    {
        if (IsInBounds(x, y, z))
        {
            densityGrid[x, y, z] += amount;
        }
    }

    public void UpdateDensityAndRegenerate(Vector3 worldPos, float amount, float radius)
    {
       
        //convert world pos to bounds of terraform0
        int minX = Mathf.FloorToInt(worldPos.x - radius);
        int maxX = Mathf.CeilToInt(worldPos.x + radius);
        int minY = Mathf.FloorToInt(worldPos.y - radius);
        int maxY = Mathf.CeilToInt(worldPos.y + radius);
        int minZ = Mathf.FloorToInt(worldPos.z - radius);
        int maxZ = Mathf.CeilToInt(worldPos.z + radius);

        HashSet<TerrainChunk> affectedChunks = new HashSet<TerrainChunk>(); // hash set prevents duplicate chunks in set - 

        //loop through terraform range (in a square)
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {

                    //to make it ciruclar ignore any points that are not in "radius"
                    float distance = Vector3.Distance(worldPos, new Vector3(x, y, z));
                    if (distance <= radius)
                    {
                        
                       
                        float falloff = 1f - (distance / radius); // falloff create sphereical terraforming as closer to edge makes falloff closer to 0
                        UpdateDensity(x, y, z, amount * falloff); // update density grid with falloff

                        TerrainChunk chunk = GetChunkFromGridPos(new Vector3(x, y, z)); // get chunk of current density grid point 
                        if (chunk != null)
                        {
                            affectedChunks.Add(chunk); // add chunk to list to be regenerated
                        }
                    }
                }
            }
        }

        foreach (TerrainChunk chunk in affectedChunks)
        {
            chunk.SetIsDirty(true);
        }
    }
    #endregion


    #region Helpers
    public bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < width &&
        y >= 0 && y < height &&
        z >= 0 && z < depth;

    }

    public Vector3 WorldToGrid(Vector3 worldPos)
    {
        return transform.InverseTransformPoint(worldPos) - gridOrigin;
    }

    public Vector3 GridToWorld(Vector3 gridPos)
    {
        return transform.TransformPoint(gridPos + gridOrigin);
    }

    public override void ApplyDamage(DestructionHitData hitData)
    {
        Vector3 gridPos = WorldToGrid(hitData.hitPoint);
        UpdateDensityAndRegenerate(gridPos, hitData.damage, hitData.radius);
    }

    public void RegenerateTerrain() 
    {
        Debug.Log($"Regenerating Terrain Mesh On {gameObject.name}");
        DestroyTerrain();
        InitaliseTerrainValues();
        GenerateChunks();

        //dig site

        ClearExitTargets();
        SpawnExitTargets();
    }

    private void DestroyTerrain()
    {
        foreach (TerrainChunk chunk in chunks.Values)
        {
            Destroy(chunk.gameObject);
        }

        chunks.Clear();
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        SetGridOrigin();

        Gizmos.color = Color.black;

        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, depth));

        float avgGridY = GetAverageSurfaceHeight();
        float avgWorldY = GridToWorld(new Vector3(0f, avgGridY, 0f)).y;

        Vector3 center = new Vector3(transform.position.x, avgWorldY, transform.position.z);
        Vector3 size = new Vector3(width, 0f, depth);

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }

    private float GetAverageSurfaceHeight()
    {
        if (width <= 0 || height <= 1 || depth <= 0)
        {
            return 0f;
        }

        float totalHeight = 0f;
        int samples = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (TryGetSurfaceHeightAt(x, z, out float surfaceY))
                {
                    totalHeight += surfaceY;
                    samples++;
                }
            }
        }

        if (samples == 0)
        {
            return height * 0.5f;
        }

        return totalHeight / samples;
    }

    private bool TryGetSurfaceHeightAt(int x, int z, out float surfaceY)
    {
        for (int y = 0; y < height - 1; y++)
        {
            float a = TerrainDensity(x, y, z);
            float b = TerrainDensity(x, y + 1, z);

            bool crossesIso = (a >= isoLevel && b < isoLevel) || (a < isoLevel && b >= isoLevel);
            if (!crossesIso)
            {
                continue;
            }

            float t = Mathf.InverseLerp(a, b, isoLevel);
            surfaceY = Mathf.Lerp(y, y + 1, t);
            return true;
        }

        surfaceY = 0f;
        return false;
    }


    #endregion

    #region Dig Site Fields
    private void SpawnExitTargets()
    {
        for (int i = 0; i < maxExitTargets; i++)
        {
            Vector3 position = GetRandomPoint();
            GameObject exitTarget = Instantiate(exitTargetPrefab, position, Quaternion.identity, this.transform);

            exitTargets.Add(exitTarget);

            //add destuction (interaction) to gamemanager
            gameManager.RegisterDestructable(exitTarget.GetComponent<DestructableBehaviour>());
        }
    }
        
    private void ClearExitTargets()
    {
        if (exitTargets == null || exitTargets.Count == 0) return;
        foreach (GameObject g in exitTargets)
        {
            if (g != null && gameManager != null)
            {
                DestructableBehaviour destructable = g.GetComponent<DestructableBehaviour>();
                if (destructable != null)
                {
                    gameManager.UnregisterDestructable(destructable);
                }
            }
            Destroy(g);
        }
        exitTargets.Clear();
    }

    private Vector3 GetRandomPoint()
    {
        float avY = GetAverageSurfaceHeight();

        float x = Random.Range(exitTargetSpawnOffset, width - exitTargetSpawnOffset);
        float y = Random.Range(exitTargetSpawnOffset, avY - exitTargetSpawnOffset);
        float z = Random.Range(exitTargetSpawnOffset, depth - exitTargetSpawnOffset);


        return GridToWorld(new Vector3(x, y, z));
    }
    #endregion
}
