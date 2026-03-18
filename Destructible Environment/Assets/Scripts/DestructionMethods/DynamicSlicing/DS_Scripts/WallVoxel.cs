using System.Collections;
using UnityEngine;

public class WallVoxel : MonoBehaviour
{
    public int x;
    public int y;

    BreakableWall breakableWall;

    private void Start()
    {
        breakableWall = transform.GetComponentInParent<BreakableWall>();
        breakableWall.addToArray(this);
        StartCoroutine(shouldFall());
    }

    public void breakVoxel()
    {
        breakableWall.checkAdjacent(this);
        breakableWall.spawnDebris(this);
        Destroy(gameObject);
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

    IEnumerator shouldFall()    //checks if this voxel should fall if no adjacent voxels
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (!breakableWall.CheckCardinal(this))
            {
                //breakVoxel();
                yield break;
            }
        }
    }
}
