using UnityEngine;

public class TerraToolBehaviour : ToolBehaviour
{
    //timer for perforamnce
    [SerializeField] private float terraTimer;
    [SerializeField] private bool canTerraform;
    private float terraCooldown;

    [SerializeField] private GameObject indicatorGO;

    public TerrainTool GetTerrainTool => (TerrainTool)GetToolData;

    private bool IsTerraforming => terraVal != 0f;


    private float terraVal;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

        canTerraform = false;
        terraTimer = 0;
        //data
        terraCooldown = GetTerrainTool.GetTerraData.GetTerraCooldown;
      
        terraVal = 0;
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        indicatorGO = Instantiate(GetTerrainTool.GetIndicatorPrefab, transform);
    }
    public override void OnToolUpdate()
    {
        base.OnToolUpdate();

        if (!canTerraform)
        {
            terraTimer += Time.deltaTime;
            if (terraTimer >= terraCooldown)
            {
                canTerraform = true;
                terraTimer = 0;
            }

        }

        UpdateIndicator();

        if (canTerraform && IsTerraforming)
        {
            base.OnToolUse();
            ModifyTerrain();
            canTerraform = false;
        }
    }

    public override void OnToolUse()
    {
        terraVal = 1f;
    }

    public override void OnToolAltUse()
    {
        base.OnToolAltUse();

        terraVal = -1f;
    }

    public override void OnToolCancelled()
    {
        base.OnToolCancelled();

        terraVal = 0f;
    }

    private void ToggleIndicator(bool state)
    {
        indicatorGO.SetActive(state);
    }

    private void UpdateIndicator()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraData.GetTerraRange))
        {
            ToggleIndicator(true);
            indicatorGO.transform.position = hit.point;
            indicatorGO.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            float radius = GetToolData.GetData.GetRadius;
        }
        else
        {
            ToggleIndicator(false);
        }
    }
    private void ModifyTerrain()
    {
        

        Debug.Log("Terraforming");

        RaycastHit hit;
        //shoot from camera - for now?
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraData.GetTerraRange))
        {
            return;
        }
        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();

        if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
        {
            DestructionHitData data = new DestructionHitData()
            {
                damage = GetToolData.GetData.GetDamage * terraVal,
                radius = GetToolData.GetData.GetRadius,
                hitNormal = hit.normal,
                hitPoint = hit.point,
            };

            target.ApplyDamage(data);
        }
    }
}
