using UnityEngine;

public class TerraToolBehaviour : ToolBehaviour
{
    //timer for perforamnce
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
    [SerializeField] private float terraTimer;
    [SerializeField] private bool canTerraform;
    private float terraCooldown;

=======
    
>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
    [SerializeField] private GameObject indicatorGO;

    public TerrainTool GetTerrainTool => (TerrainTool)GetToolData;

    private bool IsTerraforming => terraVal != 0f;


    private float terraVal;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
        canTerraform = false;
        terraTimer = 0;
        //data
        terraCooldown = GetTerrainTool.GetTerraData.GetTerraCooldown;
      
=======

>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
        terraVal = 0;
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        indicatorGO = Instantiate(GetTerrainTool.GetIndicatorPrefab, transform);
    }
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
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
=======
    public override void OnToolUpdate(float dt)
    {

        UpdateIndicator();
       

        base.OnToolUpdate(dt);

        if (CanUseTool && IsTerraforming)
        {            
            ModifyTerrain();
            //play use effect each frame while using
            if (GetToolData.GetUseEffect != null)
            {
                Instantiate(GetToolData.GetUseEffect, effectPoint.position, Quaternion.identity, effectPoint);

            }
            // resest can use
        }
    }

    protected override void OnHitEffect(DestructionHitData hitData)
    {
        base.OnHitEffect(hitData);
    }


    protected override void ToolUseBehaviour()
    {
        base.ToolUseBehaviour();
        terraVal = -1f;

        
    }

    protected override void ToolAltUseBehaviour()
    {
        base.ToolAltUseBehaviour();
        terraVal = 1f;

>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
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
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraData.GetTerraRange))
=======
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraRange))
>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
        {
            ToggleIndicator(true);
            indicatorGO.transform.position = hit.point;
            indicatorGO.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
            float radius = GetToolData.GetData.GetRadius;
=======
            float radius = GetToolData.Radius;
>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
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
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraData.GetTerraRange))
=======
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GetTerrainTool.GetTerraRange))
>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
        {
            return;
        }
        IDestructable target = hit.collider.GetComponentInParent<IDestructable>();

        if (target != null && (GetToolData.GetCompatibleLayers & target.GetLayer) != 0)
        {
            DestructionHitData data = new DestructionHitData()
            {
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
                damage = GetToolData.GetData.GetDamage * terraVal,
                radius = GetToolData.GetData.GetRadius,
=======
                damage = GetToolData.Damage * terraVal,
                radius = GetToolData.Radius,
>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
                hitNormal = hit.normal,
                hitPoint = hit.point,
            };

            target.ApplyDamage(data);
<<<<<<< HEAD:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/Instance/TerraToolBehaviour.cs
=======
            OnHitEffect(data);

>>>>>>> Player:Destructible Environment/Assets/Scripts/TechDemo/DestructionTools/GameBehaviour/TerraToolBehaviour.cs
        }
    }
}
