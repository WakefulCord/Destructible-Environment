using UnityEngine;

public class TerraToolBehaviour : ToolBehaviour
{
    //timer for perforamnce
    
    [SerializeField] private GameObject indicatorGO;

    public TerrainTool GetTerrainTool => (TerrainTool)GetToolData;

    private bool IsTerraforming => terraVal != 0f;


    private float terraVal;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);


        terraVal = 0;
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        indicatorGO = Instantiate(GetTerrainTool.GetIndicatorPrefab, transform);
    }
    public override void OnToolUpdate(float dt)
    {

        UpdateIndicator();
       

        base.OnToolUpdate(dt);

        if (CanUseTool && IsTerraforming)
        {            
            ModifyTerrain();
            //play use effect each frame while using
            if (GetToolData.PrimaryEffect != null)
            {
                Instantiate(GetToolData.PrimaryEffect, effectPoint.position, Quaternion.identity, effectPoint);

            }
            // resest can use
        }
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
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraRange))
        {
            ToggleIndicator(true);
            indicatorGO.transform.position = hit.point;
            indicatorGO.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            float radius = GetToolData.Radius;
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
        //shoot from mainCam - for now?
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraRange))
        {
            return;
        }
        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();

        if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
        {
            DestructionHitData data = new DestructionHitData()
            {
                damage = GetToolData.Damage * terraVal,
                radius = GetToolData.Radius,
                hitNormal = hit.normal,
                hitPoint = hit.point,
            };

            target.ApplyDamage(data);
            OnHitFeedback(data);

        }
    }
}
