using UnityEngine;

public class TerraToolBehaviour : ToolBehaviour
{
    [SerializeField] private GameObject indicatorGO;
    private MeshRenderer indicatorRenderer;

    public TerrainTool GetTerrainTool => (TerrainTool)GetToolData;

    private bool IsTerraforming => terraVal != 0f;
    private bool IsRemovingTerrain => terraVal < 0f;
    private bool CanModifyTerrain => IsRemovingTerrain ? CanUseTool : CanSecondaryUseTool;


    private float terraVal;
    public override void OnToolInit(DestructionTool t, Camera playerCam)
    {
        base.OnToolInit(t, playerCam);


        terraVal = 0;
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        indicatorGO = Instantiate(GetTerrainTool.GetIndicatorPrefab, transform);
        indicatorRenderer = indicatorGO.GetComponent<MeshRenderer>();   
    }
    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

        if (CanModifyTerrain && IsTerraforming)
        {            
            ModifyTerrain();
        }

        UpdateIndicator();
    }

    protected override void OnHitFeedback(DestructionHitData hitData)
    {
        base.OnHitFeedback(hitData);
    }

    protected override void PrimaryUseBehaviour()
    {
        base.PrimaryUseBehaviour();
        terraVal = -1f;

        
    }

    protected override void SecondaryUseBehaviour()
    {
        base.SecondaryUseBehaviour();
        terraVal = 1f;

    }

    public override void OnPrimaryCancelled()
    {
        base.OnPrimaryCancelled();

        terraVal = 0f;
    }

    public override void OnSecondaryCancelled()
    {
        base.OnSecondaryCancelled();

        terraVal = 0f;
    }

    private void ToggleIndicator(bool state)
    {
        indicatorGO.SetActive(state);
    }

    private void UpdateIndicator()
    {
        if (TryGetTerraformHit(out RaycastHit hit, out bool hasValidTarget))
        {
            ToggleIndicator(true);
            indicatorGO.transform.position = hit.point;
            indicatorGO.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Color indicatorColour = hasValidTarget ? Color.green : Color.red;
            indicatorRenderer.material.color = indicatorColour;
        }
        else
        {
            ToggleIndicator(false);
        }
    }
    private void ModifyTerrain()
    {
        if (!TryGetTerraformHit(out RaycastHit hit, out bool hasValidTarget) || !hasValidTarget)
        {
            return;
        }

        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();
        DestructionHitData data = new DestructionHitData()
        {
            damage = damage * terraVal,
            radius = radius,
            hitNormal = hit.normal,
            hitPoint = hit.point,
        };

        target.ApplyDamage(data);
        OnHitFeedback(data);
    }

    private bool TryGetTerraformHit(out RaycastHit hit, out bool hasValidTarget)
    {
        hasValidTarget = false;

        if (mainCam == null)
        {
            hit = default;
            return false;
        }

        if (!Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, GetTerrainTool.GetTerraRange))
        {
            return false;
        }

        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();
        hasValidTarget = target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0;
        return true;
    }
}
