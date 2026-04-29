using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : DestructableBehaviour
{
    public int width;
    public int height;

    Structurestress structureStress;
    public int originalVoxelCount;
    public float integrityThreshold = 0.25f; // Threshold for wall integrity
    private bool isDestroyed = false;
    public int structureWeight = 1;

    public WallVoxel[,] voxels;
    [SerializeField] Sprite up, down, left, right, uL, uR, dL, dR;
    [SerializeField] GameObject debris;

    [SerializeField] Sprite[] bitmaskSprites;

    public Dictionary<int, int> bitMaskSprite = new Dictionary<int, int>() //disctionary for bitmask
    {{ 2, 1 }, { 8, 2 }, { 10, 3 }, { 11, 4 }, { 16, 5 }, { 18, 6 }, { 22, 7 }, { 24, 8 },
    { 26, 9 }, { 27, 10 }, { 30, 11 }, { 31, 12 }, { 64, 13 }, { 66, 14 }, { 72, 15 }, { 74, 16 },
    { 75, 17 }, { 80, 18 }, { 82, 19 }, { 86, 20 }, { 88, 21 }, { 90, 22 }, { 91, 23 }, { 94, 24 },
    { 95, 25 }, { 104, 26 }, { 106, 27 }, { 107, 28 }, { 120, 29 }, { 122, 30 }, { 123, 31 }, { 126, 32 },
    { 127, 33 }, { 208, 34 }, { 210, 35 }, { 214, 36 }, { 216, 37 }, { 218, 38 }, { 219, 39 }, { 222, 40 },
    { 223, 41 }, { 248, 42 }, { 250, 43 }, { 251, 44 }, { 254, 45 }, { 255, 46 }, { 0, 47 }};

    public override DestructionLayer GetLayer => DestructionLayer.VoxelWall;
    Structurestress structure;
    
    private void Awake()
    {
        voxels = new WallVoxel[width, height];
        //StartCoroutine(CheckFloatingCoroutine());
        originalVoxelCount = width * height;

        structureStress = GetComponentInParent<Structurestress>(); // Get the Structurestress component from the parent object

    }
    public void addToArray(WallVoxel voxel)
    {
        voxels[voxel.x, voxel.y] = voxel;
    }

    public void spawnDebris(WallVoxel voxel)
    {
        Instantiate(debris, voxel.transform.position, Quaternion.identity);
    }

    public bool CheckCardinal(WallVoxel voxel)  //returns false if there is 3 or less cardinaly adjacent voxels
    {
        int missingCount = 0;   //for each missing voxel, count up

        // Up
        if (voxel.y + 1 >= height || voxels[voxel.x, voxel.y + 1] == null)
            missingCount++;
        // Down
        if (voxel.y - 1 < 0 || voxels[voxel.x, voxel.y - 1] == null)
            missingCount++;
        // Left
        if (voxel.x - 1 < 0 || voxels[voxel.x - 1, voxel.y] == null)
            missingCount++;
        // Right
        if (voxel.x + 1 >= width || voxels[voxel.x + 1, voxel.y] == null)
            missingCount++;

        return missingCount < 3;
    }

    public void checkAdjacent(WallVoxel voxel)  //updates textures
    {
        //up
        if (voxel.y + 1 < height && voxels[voxel.x, voxel.y + 1] != null)
            voxels[voxel.x, voxel.y + 1].updateTexture(up);
        //down
        if (voxel.y - 1 >= 0 && voxels[voxel.x, voxel.y - 1] != null)
            voxels[voxel.x, voxel.y - 1].updateTexture(down);
        //left
        if (voxel.x - 1 >= 0 && voxels[voxel.x - 1, voxel.y] != null)
            voxels[voxel.x - 1, voxel.y].updateTexture(left);
        //right
        if (voxel.x + 1 < width && voxels[voxel.x + 1, voxel.y] != null)
            voxels[voxel.x + 1, voxel.y].updateTexture(right);

        //corners
        //up-left
        if (voxel.y + 1 < height && voxel.x - 1 >= 0 && voxels[voxel.x - 1, voxel.y + 1] != null)
            voxels[voxel.x - 1, voxel.y + 1].updateTexture(uL);
        //up-right
        if (voxel.y + 1 < height && voxel.x + 1 < width && voxels[voxel.x + 1, voxel.y + 1] != null)
            voxels[voxel.x + 1, voxel.y + 1].updateTexture(uR);
        //down-left
        if (voxel.y - 1 >= 0 && voxel.x - 1 >= 0 && voxels[voxel.x - 1, voxel.y - 1] != null)
            voxels[voxel.x - 1, voxel.y - 1].updateTexture(dL);
        //down-right
        if (voxel.y - 1 >= 0 && voxel.x + 1 < width && voxels[voxel.x + 1, voxel.y - 1] != null)
            voxels[voxel.x + 1, voxel.y - 1].updateTexture(dR);
    }

    public void updateWall() //loops through entire wall and updates textures
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (voxels[x, y] != null)
                {
                    voxels[x, y].updateTile(bitmaskSprites);
                }
            }
        }
    }

    public int GetBitmask(WallVoxel voxel)
    {
        int mask = 0;

        bool north = CheckVoxel(voxel, new Vector2(0, 1));
        bool south = CheckVoxel(voxel, new Vector2(0, -1));
        bool west = CheckVoxel(voxel, new Vector2(-1, 0));
        bool east = CheckVoxel(voxel, new Vector2(1, 0));

        bool northWest = CheckVoxel(voxel, new Vector2(-1, 1));
        bool northEast = CheckVoxel(voxel, new Vector2(1, 1));
        bool southWest = CheckVoxel(voxel, new Vector2(-1, -1));
        bool southEast = CheckVoxel(voxel, new Vector2(1, -1));

        //creates the bitmask using the bools above
        if (northWest && north && west) mask += 1;
        if (north) mask += 2;
        if (northEast && north && east) mask += 4;
        if (west) mask += 8;
        if (east) mask += 16;
        if (southWest && south && west) mask += 32;
        if (south) mask += 64;
        if (southEast && south && east) mask += 128;

        return mask;
    }

    public bool CheckVoxel(WallVoxel voxel, Vector2 checkPos)
    {
        int checkX = voxel.x + (int)checkPos.x;
        int checkY = voxel.y + (int)checkPos.y;

        if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height)    //check whether voxel is in the bounds of the array
            return true; //returns true to treat out of bounds as filled - this makes it so the wall edges aren't affected by missing out of bounds voxels

        return voxels[checkX, checkY] != null;
    }

    /*IEnumerator CheckFloatingCoroutine()    //calls the CheckFloating function every 0.2 seconds for performance
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckFloating(voxels);
        }
    }*/

    public void CheckFloating(WallVoxel[,] voxels) //uses breadth first search to check if there are any voxels that are floating
    {
        bool[,] grounded = new bool[width, height];
        Queue<WallVoxel> queue = new Queue<WallVoxel>();

        for (int x = 0; x < width; x++)
        {
            if (voxels[x, 0] != null)
            {
                queue.Enqueue(voxels[x, 0]);    //each voxel on the bottom row of the wall is marked grounded and added to the startpoint queue
                grounded[x, 0] = true;               //each voxel in the startpoint queue will be used as a start point for the actual breadth first search
            }
        }

        int[] dx = new int[] { 1, -1, 0, 0 };   //these 2 will be used to check the 4 cardinally adjacent voxels of each voxel in the queue
        int[] dy = new int[] { 0, 0, 1, -1 };

        while (queue.Count > 0) //this is the actual search loop that will look for each connected voxel from each voxel in the queue
        {
            WallVoxel current = queue.Dequeue();
            int cx = current.x; //records the current voxel's position
            int cy = current.y;

            for (int i = 0; i < 4; i++) //for each cardinally adjacent voxel to the current
            {
                int nx = cx + dx[i];    //adds the dx and dy to the current position to get the new voxel's pos
                int ny = cy + dy[i];    //at i=1, it checks the current pos + (1, 0), at i=3, it checks the current pos + (0, -1)
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)   //ensure that only voxels within bounds of the wall are checked
                {
                    if (voxels[nx, ny] != null && !grounded[nx, ny])    //if the adjacent voxel exists and is not already marked grounded,
                    {
                        grounded[nx, ny] = true;    //mark it grounded and add it to the queue
                        queue.Enqueue(voxels[nx, ny]);
                    }
                }
            }
        }

        //any voxels that aren't added to the grounded array are floating

        //loops through entire wall array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (voxels[x, y] != null && !grounded[x, y])//if an existing voxel is not grounded
                {
                    voxels[x, y].breakVoxel();//destroy it
                }
            }
        }
    }

    public override void ApplyDamage(DestructionHitData hitData)
    {

        float radiusOverDistance = hitData.radius;
        foreach (Collider collider in Physics.OverlapSphere(hitData.hitPoint, radiusOverDistance))    //breaks each voxel in a radius
        {
            if (collider.GetComponent<WallVoxel>() != null)
                collider.GetComponent<WallVoxel>().breakVoxel();
        }
    }

    public int CountCurrentVoxels()
    {
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (voxels[x, y] != null)
                { 
                    count++; 
                }
            }
        }
        return count;
    }

    public void CheckWallIntegrity()
    {
        if (isDestroyed) return;

        int currentVoxelCount = CountCurrentVoxels();
        if (originalVoxelCount > 0 && (float)currentVoxelCount / originalVoxelCount < integrityThreshold) //if the current voxel count is less than the threshold percentage of the original voxel count
        {
            Debug.Log("Wall integrity failed"); // Log the wall destruction
            if (structureStress != null)
                updateStructure(); // Update the structure stress
            Destroy(gameObject); // Destroy the wall
            isDestroyed = true;
        }
    }
    public void updateStructure()
    {
        structureStress.structLimitCalc(structureWeight); //apply damage to the structure stress script
    }
}
