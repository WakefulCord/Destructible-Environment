using UnityEngine;

public class ToolBehaviour : MonoBehaviour
{
    DestructionTool tool;
    [SerializeField] protected Transform effectPoint; // for effects and start pos of bullet
    [SerializeField] protected Transform toolPivot;
    public DestructionTool GetToolData => tool;

    [Header("---Tool Behaviour---")]
    [Header("Cooldown")]
    [SerializeField] private float useTimer;
    [SerializeField] private float useCooldown;
    [SerializeField] private bool canUse;

    [SerializeField] private float altUseTimer;
    [SerializeField] private float altUseCooldown;
    [SerializeField] private bool canAltUse;

    public bool CanUseTool => canUse;
    public virtual void OnToolInit(DestructionTool t)
    {
        tool = t;

        SetUpToolStats();
    }

    protected virtual void SetUpToolStats()
    {
        //set up
        canUse = true;
        useTimer = 0.0f;

        canAltUse = true;
        altUseTimer = 0.0f;
        //apply stats
        useCooldown = GetToolData.UseCooldown;
        altUseCooldown = GetToolData.AltUseCooldown;
    }

    public virtual void OnToolUpdate(float dt)
    {
        if (!canUse)
        {
            useTimer += dt;

            if (useTimer >= useCooldown)
            {
                canUse = true;
                useTimer = 0.0f;
            }
        }

        if (!canAltUse)
        {
            altUseTimer += dt;
            if (altUseTimer >= altUseCooldown)
            {
                canAltUse = true;
                altUseTimer = 0.0f;
            }
        }

        // Aim at cursor if enabled in tool data
        if (GetToolData != null && GetToolData.DoAimAtCursor)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                RaycastHit hit;
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = ray.GetPoint(1000f);
                }
                Transform aimTransform = toolPivot != null ? toolPivot : transform;
                aimTransform.LookAt(targetPoint);
            }
        }

       
    }

    

    public virtual void OnToolUse()
    {
        if (!canUse) return; // exits out of any tool use
        
        ToolUseBehaviour();
        OnUseEffect();
        canUse = false;
    }

    protected virtual void ToolUseBehaviour()
    {
        Debug.Log("Used " + GetToolData.GetName);
        //play audio
        SoundManager.Instance.PlayClipAtPoint(GetToolData.GetUseAudio, transform.position, GetToolData.GetUseVolume);

        //use effects like muzzle flash for guns/launcher or drill particles? for terraformer
        if (GetToolData.GetUseEffect != null)
        {
            Instantiate(GetToolData.GetUseEffect, effectPoint.position, Quaternion.identity, effectPoint);

        }

    }

    public virtual void OnToolAltUse()
    {
        if (!canAltUse) return; // exits out of any tool use

        ToolAltUseBehaviour();
        OnAltUseEffect();

        canAltUse = false;
    }

    protected virtual void ToolAltUseBehaviour()
    {
        //play audio
        SoundManager.Instance.PlayClipAtPoint(GetToolData.GetUseAudio, transform.position, GetToolData.GetUseVolume);

        //use effects like muzzle flash for guns/launcher or drill particles? for terraformer
        if (GetToolData.GetUseEffect != null)
        {
            Instantiate(GetToolData.GetUseEffect, effectPoint.position, Quaternion.identity, effectPoint);

        }
    }
    public virtual void OnToolCancelled()
    {

    }
    
    protected virtual void OnUseEffect()
    {

    }
    protected virtual void OnAltUseEffect()
    {

    }
    protected virtual void OnHitEffect(DestructionHitData hitData) // when the tools destruction happens like bullet hitting wall or explosion or terraform effect
    {
        if (GetToolData == null || GetToolData.GetDestructionFeedback == null) return;

        if (GetToolData != null && GetToolData.GetDestructionFeedback != null)
        {
            //visual  + effects
            Instantiate(GetToolData.GetDestructionFeedback.DestructionEffect, hitData.hitPoint, Quaternion.identity);
            SoundManager.Instance.PlayClipAtPoint(GetToolData.GetDestructionFeedback.GetDestructionAudio, hitData.hitPoint, GetToolData.GetDestructionFeedback.GetDestructionVolume);
        }
    }

   
}

