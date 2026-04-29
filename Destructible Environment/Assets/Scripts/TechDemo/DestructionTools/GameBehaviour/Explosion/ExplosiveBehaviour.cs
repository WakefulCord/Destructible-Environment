using System.Collections.Generic;
using System;
using UnityEngine;

//Might need an explosive manager as explosives break when tool is switched

public class ExplosiveBehaviour : MonoBehaviour // actual explosion - runs logic and communicates with destruction, not a "useable tool"
{
    public event Action<ExplosiveBehaviour> Exploded; // so it can talk to whatever spawned it if it needs to, like for remote detonator to know when its explosive has gone off

    [SerializeField] private bool isProjectile;

    private float damage;
    private float radius;
    private DestructionLayer compatibleLayers;
    private ExplosionFeedback explosionData;

    private float fuseDuration;
    private float fuseTimer;
    private bool hasExploded;

    private ExplosionTool GetTool;
   
    private bool isActive; // is explosive active, start timers/explode

    public bool HasExploded => hasExploded;

    public void OnRemoteExplosiveInit(ExplosionTool tool)
    {
        SetExplosionStats(tool);
        



        
        

     
    }

    public void OnFuseExplosiveInit(ExplosionTool tool, float fuseTimer)
    {
        SetExplosionStats(tool);
        


        fuseDuration = fuseTimer;
        
       
    }

    public void OnImpactExplosiveInit(ExplosionTool tool)
    {
        SetExplosionStats(tool);
        isProjectile = true;
        
        fuseDuration = -1f;
    }

    private void SetExplosionStats(ExplosionTool tool)
    {
        GetTool = tool;
        explosionData = GetTool.GetExplosionFeedback;
        isActive = false;
        
        hasExploded = false;
        isProjectile = false;
        fuseDuration = -1f; // instant explosion, no fuse - default
        damage = GetTool.Damage;
        radius = GetTool.Radius;
        compatibleLayers = GetTool.GetCompatibleLayers;
        explosionData = GetTool.GetExplosionFeedback;

        fuseTimer = 0f;
    }

    public void OnExplosiveUpdate()
    {
        if (!isActive || hasExploded) return;

        fuseTimer += Time.deltaTime;
        if (fuseTimer >= fuseDuration)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision) //launcher 
    {
        if (isProjectile && !hasExploded)
        {
            Explode(); // straight to explode - nothing is managing it
        }
    }

    public void ActivateExplosive()
    {
        isActive = true;
    }

   

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        Exploded?.Invoke(this);
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

                Vector3 hitPoint = hit.ClosestPoint(transform.position);

                float dist = Vector3.Distance(transform.position, hitPoint);
                float t = 1f - (dist / radius);
                float falloffDamage = damage * Mathf.Clamp01(t);

                DestructionHitData data = new DestructionHitData()
                {
                    damage = falloffDamage,
                    radius = radius,
                    hitPoint = hitPoint,
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
