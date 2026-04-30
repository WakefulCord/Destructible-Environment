using System.Collections.Generic;
using UnityEngine;

public class HVoxelHouse : MonoBehaviour
{
    GameManager gameManager;
    [Header("House Fields")]
    [SerializeField] private int width;
    [SerializeField] private int depth;
   
    [SerializeField] private int floorCount = 1;
    [SerializeField] private int floorHeight = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int wallsPerWall = 4; // split walls into chunks
    [SerializeField] private int floorsPerFloor = 4;

    [SerializeField] private GameObject partPrefab;
    [SerializeField] private GameObject voxelPrefab;

    [SerializeField] private List<HVoxelHousePart> houseParts = new List<HVoxelHousePart>();

    private Vector3 worldOffset;

    public int HouseHeight
    {
        get { return floorCount * floorHeight; }
    }
    public GameObject VoxelPrefab => voxelPrefab;
    public void OnStart()
    {
        gameManager = GameManager.Instance;
        worldOffset = new Vector3(
        -(width / 2f) * cellSize,
        -(HouseHeight / 2f) * cellSize,
        -(depth / 2f) * cellSize
);
        worldOffset += Vector3.one * (cellSize * 0.5f);
        GenerateHouse();

    }

    private void GenerateHouse()
    {
        ClearExistingParts();

        for (int floor = 0; floor < floorCount; floor++)
        {   //-8.4
            int floorY = floor * (floorHeight);
            
            CreateWallPart(floor, HousePartType.FrontWall, floorY);
            CreateWallPart(floor, HousePartType.BackWall, floorY);
            CreateWallPart(floor, HousePartType.RightWall, floorY);
            CreateWallPart(floor, HousePartType.LeftWall, floorY);

            
            CreateFloorPart(floor, floorY);
            
        }

        int ceilingY = floorCount * (floorHeight);
        CreateCeilingPart(ceilingY);
    }

    private void CreateWallPart(int floorNum, HousePartType partType, int floorY)
    {
       

        bool isXAxis = (partType == HousePartType.FrontWall || partType == HousePartType.BackWall); // get axis to place wall along

        int correctedLength = isXAxis ? width : depth; // correct axis length
        int segmentSize = correctedLength / wallsPerWall; // how many walls segments can fit in this length



        for (int i = 0; i < wallsPerWall; i++)
        {
            //int currentSize = (i == wallsPerWall - 1) ? correctedLength - (segmentSize * i) : segmentSize; // handle remainder if goes over

            Vector3 basePos = GetWallPartPosition(partType, floorY); // side to position wall 

            Vector3 offset;

            if (isXAxis) // move segment to point on axis
            {
                offset = new Vector3(i * segmentSize * cellSize, 0f, 0f);
            }
            else
            {
                offset = new Vector3(0f, 0f, i * segmentSize * cellSize);

            }
            Debug.Log($"{partType}_{floorNum}_{i} offset: {offset}");
            Vector3 localPos = basePos + offset + worldOffset;

            //if is placed on x axis then x axis width needs to be 1 (1 voxel thick wall)
            int partWidth = isXAxis ? segmentSize : 1;
            int partDepth = isXAxis ? 1 : segmentSize;

            HVoxelHousePart part = CreatePart(
                $"{partType}_{floorNum}_{i}",
                localPos,
                partWidth,
                floorHeight,
                partDepth,
                partType
            );

            if (part != null)
                houseParts.Add(part);
                gameManager.RegisterDestructable(part);

        }
    }

    private void CreateCeilingPart(int ceilingY)
    {
        Vector3 pos = worldOffset + new Vector3(0f, ceilingY * cellSize, 0f);
        pos.y -= cellSize;
        HVoxelHousePart part = CreatePart(
            "Ceiling",
            pos,
            width,
            1,
            depth,
            HousePartType.Ceiling
        );

        if (part != null)
        {
            houseParts.Add(part);
            gameManager.RegisterDestructable(part);

        }

    }

    private void CreateFloorPart(int floor, int floorY)
    {
        Vector3 pos = worldOffset + new Vector3(0f, floorY * cellSize, 0f);
        pos.y -= cellSize;
        HVoxelHousePart part = CreatePart(
            $"Floor_{floor}",
            pos,
            width,
            1,
            depth,
            HousePartType.Floor
        );

        if (part != null)
            houseParts.Add(part);
            gameManager.RegisterDestructable(part);

    }

    private HVoxelHousePart CreatePart(string partName, Vector3 localPosition, int partWidth, int partHeight, int partDepth, HousePartType partType)
    {
        
        GameObject partGO = Instantiate(partPrefab, transform);
        HVoxelHousePart part = partGO.GetComponent<HVoxelHousePart>();
        part.name = partName;
        part.transform.localPosition = localPosition;
        part.Init(this, partWidth, partHeight, partDepth, partType, cellSize);
        part.CreateHousePart();
        return part;
    }

    private Vector3 GetWallPartPosition(HousePartType partType, int floorY)
    {
        float y = floorY * cellSize;

        switch (partType)
        {
            case HousePartType.FrontWall:
                return new Vector3(0f, y, 0f);

            case HousePartType.BackWall:
                return new Vector3(0f, y, (depth) * cellSize);

            case HousePartType.LeftWall:
                return new Vector3(0f, y, 0f);

            case HousePartType.RightWall:
                return new Vector3((width) * cellSize, y, 0f);

            default:
                return Vector3.zero;
        }
    }

    private void ClearExistingParts()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        houseParts.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Vector3 size = new Vector3((width * cellSize), (HouseHeight * cellSize), (depth * cellSize));

        Gizmos.DrawCube(transform.position, size);

    }

    
    
}

public enum HousePartType
{
    FrontWall,
    BackWall,
    RightWall,
    LeftWall,
    Floor,
    Ceiling
}
