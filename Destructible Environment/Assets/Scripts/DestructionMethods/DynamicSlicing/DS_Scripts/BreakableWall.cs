using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IDestructable
{
    public int width = 10;
    public int height = 10;

    public WallVoxel[,] voxels;
    [SerializeField] Sprite up, down, left, right, uL, uR, dL, dR;
    [SerializeField] GameObject debris;

    public DestructionLayer GetLayer => DestructionLayer.VoxelWall;

    private void Awake()
    {
        voxels = new WallVoxel[width, height];
        StartCoroutine(CheckFloatingCoroutine());

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
        int missingCount = 0;

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

    IEnumerator CheckFloatingCoroutine()    //calls the CheckFloating function every 0.2 seconds for performance
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckFloating(voxels);
        }
    }

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

    public void ApplyDamage(DestructionHitData hitData)
    {
        foreach (Collider collider in Physics.OverlapSphere(hitData.hitPoint, hitData.radius))    //breaks each voxel in a radius
        {
            if (collider.GetComponent<WallVoxel>() != null)
                collider.GetComponent<WallVoxel>().breakVoxel();
        }
    }

  
}
