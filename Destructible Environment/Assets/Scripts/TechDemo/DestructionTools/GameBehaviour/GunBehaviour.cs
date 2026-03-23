using UnityEngine;
using System.Collections.Generic;

public class GunBehaviour : ToolBehaviour
{

    public GunTool GetGunTool => (GunTool)GetToolData;


    [Header("Tracer Fields")]
    [SerializeField] private List<TracerEffect> activeTracers = new List<TracerEffect>();
    Vector3 lastTracerEnd;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

        
    }

 

    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

        if (GetGunTool.UseTracer)
        {

            List<TracerEffect> tracersToDestroy = new List<TracerEffect>();

            foreach (TracerEffect t in activeTracers)
            {
                bool reachedEnd = t.UpdateTracer(dt);
                if (reachedEnd)
                {
                    tracersToDestroy.Add(t);
                }
            }


            foreach (TracerEffect t in tracersToDestroy)
            {
                activeTracers.Remove(t);
                Destroy(t.gameObject);
            }
        }
    }


    //protected override void OnToolUseLogic()
    //{
    //    base.OnToolUseLogic();
    //    Shoot();
    //}

    protected override void ToolUseBehaviour()
    {
        base.ToolUseBehaviour();
        Shoot();
    }
    protected override void ToolAltUseBehaviour()
    {
        base.ToolAltUseBehaviour();
        //aim?
        Debug.Log("No alt Use logic yet!");

    }

    protected override void OnUseEffect()
    {
        base.OnUseEffect();

        if (GetGunTool.GetBulletFeedback == null) return;

        Vector3 origin = effectPoint != null ? effectPoint.position : Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        Vector3 tracerEnd = origin + direction * 1000f; // default far point

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
        {
            tracerEnd = hit.point;
        }

        if (GetGunTool.UseTracer)
        {

            GameObject tracer = Instantiate(
                GetGunTool.GetTracerPrefab,
                origin,
                Quaternion.identity,
                this.transform
            );
            TracerEffect tracerEffect = tracer.GetComponent<TracerEffect>();
            tracerEffect.OnInit(GetGunTool.GetTracerSpeed, origin, tracerEnd);
            activeTracers.Add(tracerEffect);
            lastTracerEnd = tracerEnd;
        }

        

    }

    protected override void OnAltUseEffect()
    {
        base.OnAltUseEffect();
        //zoom in soudns?   - might be nothing

    }

    private void Shoot() // ADD BUllet SPREAD
    {
        Debug.Log("BANG SHOOT");

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        RaycastHit hit;


        //tracer



        if (!Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
        {

            return;
        }

        DestructionHitData hitData = new DestructionHitData()
        {
            damage = GetToolData.Damage,
            radius = GetToolData.Radius,
            hitNormal = hit.normal,
            hitPoint = hit.point,
        };

        //bullet hit 

        OnHitEffect(hitData);

        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();
        if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
        {
            target.ApplyDamage(hitData);
        }

    }
    protected override void OnHitEffect(DestructionHitData hitData)
    {
        base.OnHitEffect(hitData);

        if (GetGunTool.GetBulletFeedback == null) return;

        if (GetGunTool.GetBulletFeedback.UseDecal)
        {
            //create decal at hit

        
            GameObject decal = Instantiate(
                GetGunTool.GetBulletFeedback.GetDecal,
                hitData.hitPoint,
                Quaternion.LookRotation(-hitData.hitNormal)
            );

            decal.GetComponent<DecalFadeOut>().OnInit(GetGunTool.GetBulletFeedback.GetDecalFadeTimer);
        }


        
    }
       
       
    

   
}
