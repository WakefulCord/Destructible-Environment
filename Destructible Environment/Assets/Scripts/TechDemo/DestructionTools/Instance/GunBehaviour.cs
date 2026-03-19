using System.Collections;
using UnityEngine;

public class GunBehaviour : ToolBehaviour
{
    [SerializeField] private float fireTimer;
    [SerializeField] private bool canFire;

    [Header("Data Fields")]
    [SerializeField] private float fireRate;

    private LineRenderer lineRenderer;

    public GunTool GetGunTool => (GunTool)GetToolData;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

        canFire = false;
        fireTimer = 0;
        //data
        fireRate = GetGunTool.FireRate;

        SetupLineRenderer();
    }

    private void SetupLineRenderer() // basic visuals
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
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
        Debug.Log("BANG SHOOT");

        canFire = false;

        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
        {
            ShowTracer(origin, hit.point);

            IDestructable target = hit.collider.GetComponentInParent<IDestructable>();

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
        else
        {
            ShowTracer(origin, origin + direction * 100f);
        }
    }

    private void ShowTracer(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
        StartCoroutine(HideTracer());
    }

    private IEnumerator HideTracer()
    {
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }
}
