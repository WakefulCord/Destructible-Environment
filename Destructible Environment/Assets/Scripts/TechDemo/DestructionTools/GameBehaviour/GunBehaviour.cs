using UnityEngine;
using System.Collections.Generic;

public class GunBehaviour : ToolBehaviour
{
    private const float DefaultShotDistance = 1000f;

    private struct PelletShotData
    {
        public Vector3 endPoint;
    }
    private struct DebugData
    {
        public Vector3 position;
        public Vector3 normal;
        public float radius;
        public float expireTime;
    }
    public GunTool GetGunTool => (GunTool)GetToolData;


    [Header("Tracer Fields")]
    [SerializeField] private List<TracerEffect> activeTracers = new List<TracerEffect>();
    private readonly List<PelletShotData> pendingShotVisuals = new List<PelletShotData>();
    Vector3 lastTracerEnd;

    [Header("Debug Fields")]
    [SerializeField] private bool showDebug;
    List<DebugData> pendingDebug;
    [SerializeField] private float debugLifetime = 1.0f;
    [SerializeField] private float normalDebugRayLength = 0.5f;
    [SerializeField] private Color normalDebugColour = Color.red;
    [SerializeField] private Color sphereHitDebugColour = Color.red;

    bool isAiming = false;

    #region Set Up 
    public override void OnToolInit(DestructionTool t, Camera playerCam)
    {
        base.OnToolInit(t, playerCam);
        isAiming = false;

        
        pendingDebug = new List<DebugData>();
        

    }
    #endregion

    #region Update
    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

        UpdateTracers(dt);

        CleanupExpiredDebug();
    }
    
    private void UpdateTracers(float dt)
    {
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

    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!showDebug || pendingDebug == null || pendingDebug.Count == 0) return;

        float now = Application.isPlaying ? Time.time : 0f;

        for (int i = 0; i < pendingDebug.Count; i++)
        {
            DebugData data = pendingDebug[i];

            if (Application.isPlaying && now > data.expireTime)
                continue;

            Gizmos.color = sphereHitDebugColour;
            Gizmos.DrawSphere(data.position, Mathf.Max(0.01f, data.radius));

            Gizmos.color = normalDebugColour;
            Gizmos.DrawLine(
                data.position,
                data.position + data.normal.normalized * normalDebugRayLength
            );
        }
    }

    private void AddDebugData(Vector3 pos, Vector3 norm, float radius)
    {
        pendingDebug.Add(new DebugData
        {
            position = pos,
            normal = norm,
            radius = radius,
            expireTime = Time.time + debugLifetime,
        });
    }
    private void CleanupExpiredDebug()
    {
        if (!showDebug || pendingDebug == null || pendingDebug.Count == 0) return;

        float now = Time.time;
        pendingDebug.RemoveAll(data => now > data.expireTime);
    }
    #endregion




    protected override void PrimaryUseBehaviour()
    {
        base.PrimaryUseBehaviour(); // handles visuals
        Shoot();
    }
    protected override void SecondaryUseBehaviour()
    {
        base.SecondaryUseBehaviour();

        if (GetGunTool.CanAim)
        {
            ToggleAim(true);
        }
    }

    public override void OnSecondaryCancelled()
    {
        base.OnSecondaryCancelled();

        if (GetGunTool.CanAim)
        {
            ToggleAim(false);
        }
    }

    private void ToggleAim(bool state)
    {
        isAiming = state;

        if (isAiming)
        {
            Debug.Log($"{GetGunTool.GetName}: Is Aiming");
            CameraManager.Instance.EnableADS(GetGunTool.AimFOV);
        }
        else
        {
            CameraManager.Instance.DisableADS();
        }
    }



    protected override void PrimaryUseFeedback()
    {
        base.PrimaryUseFeedback();

        if (!GetGunTool.UseTracer || GetGunTool.GetTracerPrefab == null) return;

        Vector3 origin = effectPoint != null ? effectPoint.position : mainCam.transform.position;

        foreach (PelletShotData pellet in pendingShotVisuals)
        {
            GameObject tracer = Instantiate(
                GetGunTool.GetTracerPrefab,
                origin,
                Quaternion.identity,
                this.transform
            );
            TracerEffect tracerEffect = tracer.GetComponent<TracerEffect>();
            tracerEffect.OnInit(GetGunTool.GetTracerSpeed, origin, pellet.endPoint);
            activeTracers.Add(tracerEffect);
            lastTracerEnd = pellet.endPoint;
        }
        pendingShotVisuals.Clear();
    }

    protected override void SecondaryUseFeedback()
    {
        base.SecondaryUseFeedback();
        //zoom in soudns?   - might be nothing

    }

    private void Shoot() // ADD BUllet SPREAD
    {

        pendingShotVisuals.Clear();

        Vector3 origin = mainCam.transform.position;
        Vector3 direction = mainCam.transform.forward;
        int bulletCount = Mathf.Max(1, GetGunTool.BulletCount);

        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 pelletDirection = GetPelletDirection(direction, bulletCount);
            Vector3 pelletEnd = origin + pelletDirection * DefaultShotDistance;

            RaycastHit hit;
            if (Physics.Raycast(origin, pelletDirection, out hit, Mathf.Infinity))
            {
                pelletEnd = hit.point;

                DestructionHitData hitData = new DestructionHitData()
                {
                    damage = damage,
                    radius = radius,
                    hitNormal = hit.normal,
                    hitPoint = hit.point,
                };

                OnBulletHit(hitData);

                if (showDebug)
                {
                    AddDebugData(hitData.hitPoint, hitData.hitNormal, hitData.radius);
                }
                DestructableBehaviour target = hit.collider.GetComponentInParent<DestructableBehaviour>();
                if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
                {
                    if (showDebug)
                    {
                        Debug.Log($"Sending {gameObject.name} to {target.gameObject.name}");
                    }
                    target.ApplyDamage(hitData);
                }
            }

            pendingShotVisuals.Add(new PelletShotData
            {
                endPoint = pelletEnd
            });
        }

    }

    private Vector3 GetPelletDirection(Vector3 baseDirection, int bulletCount)
    {
        float countSpreadAngle = Mathf.Max(0f, bulletCount - 1) * 1.5f;
        float spreadAngle = Mathf.Max(GetGunTool.SpreadAngle, countSpreadAngle);

        if (spreadAngle <= 0f || bulletCount <= 1)
        {
            return baseDirection;
        }

        float pitch = Random.Range(-spreadAngle, spreadAngle);
        float yaw = Random.Range(-spreadAngle, spreadAngle);

        return (Quaternion.Euler(pitch, yaw, 0f) * baseDirection).normalized;
    }

    private void OnBulletHit(DestructionHitData hitData) // runs tool behaviour OnHit
    {
        OnHit(hitData);
    }
    protected override void OnHitFeedback(DestructionHitData hitData)
    {
        base.OnHitFeedback(hitData);

        if (GetGunTool.GetBulletFeedback == null) return;

        if (GetGunTool.GetBulletFeedback.UseDecal)
        {
            //create decal at hit

            
            GameObject decal = Instantiate(
                GetGunTool.GetBulletFeedback.GetDecal,
                hitData.hitPoint,
                Quaternion.LookRotation(-hitData.hitNormal)
            );

            

            decal.GetComponent<DecalEffect>().OnInit(GetGunTool.GetBulletFeedback.DecalScale,GetGunTool.GetBulletFeedback.GetDecalFadeTimer);
        }


        
    }


    public override void OnUnequip()
    {
        base.OnUnequip();

        if (GetGunTool != null && GetGunTool.CanAim)
        {
            ToggleAim(false);
        }
    }

   
}
