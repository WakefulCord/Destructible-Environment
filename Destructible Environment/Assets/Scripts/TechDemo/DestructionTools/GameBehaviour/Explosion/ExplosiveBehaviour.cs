using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBehaviour : MonoBehaviour // actual explosion - runs logic and communicates with destruction, not a "useable tool"
{
    [SerializeField] private bool isProjectile;

    private float damage;
    private float radius;
    private DestructionLayer compatibleLayers;
    private ExplosionFeedback explosionData;

    private float fuseTimer;
    private bool hasExploded;

    private ExplosionTool GetTool;

    public void OnExplosiveInit(ExplosionTool tool)
    {
        damage = tool.Damage;
        radius = tool.Radius;
        compatibleLayers = tool.GetCompatibleLayers;
        explosionData = tool.GetExplosionFeedback;
        fuseTimer = 0f;
        hasExploded = false;

        GetTool = tool;
    }

    private void OnExplosiveUpdate()
    {
        if (hasExploded) return;

        if (explosionData != null && explosionData.FuseTime > 0f)
        {
            fuseTimer += Time.deltaTime;
            if (fuseTimer >= explosionData.FuseTime)
            {
                Explode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProjectile && !hasExploded)
        {
            Explode();
        }
    }

    public void Detonate()
    {
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
         // explosion visuals
        if (explosionData != null && explosionData.DestructionEffect != null) 
        {
            //visual  + effects
            Instantiate(explosionData.DestructionEffect, transform.position, Quaternion.identity);
            SoundManager.Instance.PlayClipAtPoint(explosionData.GetDestructionAudio, transform.position, explosionData.GetDestructionVolume);
        }

        //collsion logic with destruction

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        HashSet<IDestructable> damagedTargets = new HashSet<IDestructable>();

        foreach (Collider hit in hits)
        {
            IDestructable target = hit.GetComponentInParent<IDestructable>();
            if (target != null && !damagedTargets.Contains(target) && (compatibleLayers & target.GetLayer) != 0)
            {
                damagedTargets.Add(target);

                Vector3 direction = (hit.transform.position - transform.position).normalized;

                DestructionHitData data = new DestructionHitData()
                {
                    damage = damage,
                    radius = radius,
                    hitPoint = hit.ClosestPoint(transform.position),
                    hitNormal = direction,
                };

                target.ApplyDamage(data);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && explosionData != null)
            {
                rb.AddExplosionForce(explosionData.DestructiveForce, transform.position, radius);
            }
        }

        Destroy(gameObject);
    }
}
