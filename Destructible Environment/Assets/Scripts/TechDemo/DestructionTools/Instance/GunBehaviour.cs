using UnityEngine;

public class GunBehaviour : ToolBehaviour
{
    [SerializeField] private float fireTimer;
    [SerializeField] private bool canFire;

    [Header("Data Fields")]
    [SerializeField] private float fireRate;

    public GunTool GetGunTool => (GunTool)GetToolData;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

        canFire = false;
        fireTimer = 0;
        //data
        fireRate = GetGunTool.FireRate;
    }
    public override void OnToolUpdate()
    {
        base.OnToolUpdate();

        if (!canFire)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                canFire = true;
                fireTimer = 0;
            }
        }
    }

    public override void OnToolUse()
    {
       if (canFire)
        {
            Shoot();
        }

        base.OnToolUse();

    }

    private void Shoot()
    {
        //shoot behaviour
        
        Debug.Log("BANG SHOOT");

        RaycastHit hit;
        //shoot from camera - for now?
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            return;
        }
        IDestructable target = hit.collider.GetComponentInParent<IDestructable>(); // root object has the destruction manager script (hopefully)
            

        if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
        {
            DestructionHitData data = new DestructionHitData()
            {
                damage = GetGunTool.GetData.GetDamage,
                radius = GetGunTool.GetData.GetRadius,
                hitNormal = hit.normal,
                hitPoint = hit.point,
            };

            target.ApplyDamage(data);
        }
    }
}
