using System.Collections;
using UnityEngine;

public class WallVoxel : MonoBehaviour
{
    public int x;
    public int y;

    BreakableWall breakableWall;
    Objectstress objectStress;

    [SerializeField] float debrisChance;

    
    private void Start()
    {
        objectStress = GetComponentInParent<Objectstress>();
        breakableWall = transform.GetComponentInParent<BreakableWall>();
        breakableWall.addToArray(this);
        StartCoroutine(shouldFall());
    }

    /*public void breakVoxel()
    {
        breakableWall.checkAdjacent(this);
        breakableWall.spawnDebris(this);
        Destroy(gameObject);
    }*/

    public void breakVoxel()
    {
        breakableWall.voxels[x, y] = null;

        breakableWall.updateWall();

        if(Random.value < debrisChance)
            breakableWall.spawnDebris(this);


        breakableWall.CheckFloating(breakableWall.voxels);

        Destroy(gameObject);
        breakableWall.CheckWallIntegrity();
        if (!objectStress) return;
        objectStress.limitCalc(1f); // Simulate taking 1 damage to the object stress when a voxel is broken

    }

    public void updateTexture(Sprite sprite)    //updates the texture of the voxel's faces
    {
        foreach(Transform child in transform)
        {
            if(child.GetComponent<SpriteRenderer>() != null)
            {
                child.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
    }

    public void updateTile(Sprite[] sprites)    //function used for 47 texture system
    {
        int mask = breakableWall.GetBitmask(this);

        if(breakableWall.bitMaskSprite.TryGetValue(mask, out int spriteIndex))
        {
            updateTexture(sprites[spriteIndex]);
        }
    }


    IEnumerator shouldFall()    //checks if this voxel should fall if no adjacent voxels
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (!breakableWall.CheckCardinal(this))
            {
                breakVoxel();
                yield break;
            }
        }
    }
}
