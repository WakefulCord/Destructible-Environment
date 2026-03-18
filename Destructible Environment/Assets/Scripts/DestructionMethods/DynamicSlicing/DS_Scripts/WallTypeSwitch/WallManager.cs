using Unity.VisualScripting;
using UnityEngine;

public class WallManager : MonoBehaviour, IDestructable
{
    [SerializeField] GameObject brokenWall;
    [SerializeField] GameObject wall;

    public DestructionLayer GetLayer => DestructionLayer.WallSwitch;

    public void ApplyDamage(DestructionHitData hitData)
    {
        breakWall(hitData);
    }

    public void breakWall(DestructionHitData h)
    {
        brokenWall.SetActive(true);
        wall.SetActive(false);

        ExplodeWall(h);
    }
    private void ExplodeWall(DestructionHitData h)
    {
        brokenWall.GetComponentInChildren<Rigidbody>().AddExplosionForce(h.damage, h.hitPoint, h.radius);
    }
}
