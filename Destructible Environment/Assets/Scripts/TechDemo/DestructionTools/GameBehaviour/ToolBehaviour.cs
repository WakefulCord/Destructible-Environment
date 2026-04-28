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
    [SerializeField] private bool isPrimaryInUse;

    [SerializeField] private float secondaryTimer;
    [SerializeField] private float secondaryCooldown;
    [SerializeField] private bool canSecondaryUse;
    [SerializeField] private bool isSecondaryInUse;

    [Header("Exposed Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float radius;

    protected Camera mainCam;

    public bool CanUseTool => canUse;
    public bool CanSecondaryUseTool => canSecondaryUse;

    #region Set Up
    public virtual void OnToolInit(DestructionTool t, Camera playerCam)
    {
        tool = t;

        SetUpToolStats(playerCam);
    }

    protected virtual void SetUpToolStats(Camera playerCam)
    {
        mainCam = playerCam;


        //set up
        canUse = true;
        useTimer = 0.0f;
        isPrimaryInUse = false;

        canSecondaryUse = true;
        secondaryTimer = 0.0f;
        isSecondaryInUse = false;

        //apply stats
        if (GetToolData == null)
        {
            Debug.LogError("No tool data assigned to " + gameObject.name + ", failed to set up.");
            return;
        }
        useCooldown = GetToolData.UseCooldown;
        secondaryCooldown = GetToolData.AltUseCooldown;

        damage = GetToolData.Damage;
        radius = GetToolData.Radius;
    }
    #endregion

    #region Update
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

        if (!canSecondaryUse)
        {
            secondaryTimer += dt;
            if (secondaryTimer >= secondaryCooldown)
            {
                canSecondaryUse = true;
                secondaryTimer = 0.0f;
            }
        }

        // Aim at cursor if enabled in tool data
        if (GetToolData != null && GetToolData.DoPointAtCursor)
        {
            if (mainCam != null)
            {
                Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
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
    #endregion

    #region Primary Tool Use
    public virtual void OnPrimaryUse()
    {
        if (!canUse) return; // exits out of any tool use
        if (UsesContinuousPrimaryUse() && isPrimaryInUse) return;

        PrimaryUseBehaviour();
        PrimaryUseFeedback();

        if (UsesContinuousPrimaryUse())
        {
            isPrimaryInUse = true;
            return;
        }

        if (UsesPrimaryCooldown())
        {
            canUse = false;
        }
    }

    protected virtual void PrimaryUseBehaviour()
    {
        //nothing - implemented in children classes like gun/launcher/terraformer
        // could run a ray cast here for hitscan weapons but for now just run it in the gun class

    }
    protected virtual void PrimaryUseFeedback() // Guns shoot, terraform drill, launcher shoot - handles visuals and audio
    {
        if (GetToolData == null)
        {
            Debug.LogError($"{name}: No Tool Data assigned - No feedback to show for primary use");
            return;
        }
        //play audio
        if (GetToolData.PrimaryAudio != null)
        {
            SoundManager.Instance.PlayClipAtPoint(GetToolData.PrimaryAudio, transform.position, GetToolData.GetUseVolume);
        }
        else
        {
            Debug.LogWarning($"{GetToolData.GetName}: No Primary Use Audio assigned");


        }

        // effects
        if (GetToolData.PrimaryEffect != null)
        {
            Instantiate(GetToolData.PrimaryEffect, effectPoint.position, Quaternion.identity, effectPoint);

        }
        else
        {
            Debug.LogWarning($"{GetToolData.GetName}: No Primary Use Effect assgined");
        }
    }
    #endregion

    #region Secondary Tool Use
    public virtual void OnSecondaryUse()
    {
        if (!canSecondaryUse) return; // exits out of any tool use
        if (UsesContinuousSecondaryUse() && isSecondaryInUse) return;

        SecondaryUseBehaviour();
        SecondaryUseFeedback();

        if (UsesContinuousSecondaryUse())
        {
            isSecondaryInUse = true;
            return;
        }

        if (UsesSecondaryCooldown())
        {
            canSecondaryUse = false;
        }
    }
    protected virtual void SecondaryUseBehaviour()
    {
        // no second behaviour yet - might be ads or line of fire visuals - for drill will be addign terrains
    }
    protected virtual void SecondaryUseFeedback()
    {
        // no second feedback yet - might be weapon moving sounds when aiming or for drill will be adding terrain
    }

    #endregion

    #region Fire Modes
    public bool IsPrimaryHoldMode()
    {
        return GetToolData != null && GetToolData.PrimaryFireMode == ToolFireMode.Hold;
    }

    public bool IsSecondaryHoldMode()
    {
        return GetToolData != null && GetToolData.SecondaryFireMode == ToolFireMode.Hold;
    }

    public bool IsPrimarySingleClickMode()
    {
        return GetToolData != null && GetToolData.PrimaryFireMode == ToolFireMode.SingleClick;
    }

    public bool IsSecondarySingleClickMode()
    {
        return GetToolData != null && GetToolData.SecondaryFireMode == ToolFireMode.SingleClick;
    }
    #endregion

    #region Use Type
    protected bool UsesPrimaryCooldown()
    {
        return GetToolData != null && GetToolData.PrimaryUsesCooldown;
    }

    protected bool UsesSecondaryCooldown()
    {
        return GetToolData != null && GetToolData.SecondaryUsesCooldown;
    }

    protected virtual bool UsesContinuousPrimaryUse()
    {
        return IsPrimaryHoldMode() && !UsesPrimaryCooldown();
    }

    protected virtual bool UsesContinuousSecondaryUse()
    {
        return IsSecondaryHoldMode() && !UsesSecondaryCooldown();
    }
    #endregion

    #region Destrustion Hit
    //not all tools implement - for example explives shoot explosives which handle their own stuff, get data from here tho

    protected virtual void OnHit(DestructionHitData hitData) // when the tools destruction happens like bullet hitting wall or explosion or terraform effect - ran from children classes like gun when bullet hits.
    {
        OnHitFeedback(hitData);
    }

    protected virtual void OnHitFeedback(DestructionHitData hitData) // when the tools destruction happens like bullet hitting wall or explosion or terraform effect
    {
        if (GetToolData == null || GetToolData.GetDestructionFeedback == null)
        {
            Debug.LogWarning("Tool " + GetToolData.GetName + " does not have destruction feedback assigned, skipping hit effects.");
            return;
        }

        //visual  + effects
        Instantiate(GetToolData.GetDestructionFeedback.DestructionEffect, hitData.hitPoint, Quaternion.identity);
        SoundManager.Instance.PlayClipAtPoint(GetToolData.GetDestructionFeedback.GetDestructionAudio, hitData.hitPoint, GetToolData.GetDestructionFeedback.GetDestructionVolume);

    }
    #endregion

    #region Other Tool Methods

    public virtual void OnPrimaryCancelled()
    {
        if (isPrimaryInUse)
        {
            isPrimaryInUse = false;
            if (UsesPrimaryCooldown())
            {
                canUse = false;
                useTimer = 0.0f;
            }
            else
            {
                canUse = true;
            }
        }
    }

    public virtual void OnSecondaryCancelled()
    {
        if (isSecondaryInUse)
        {
            isSecondaryInUse = false;
            if (UsesSecondaryCooldown())
            {
                canSecondaryUse = false;
                secondaryTimer = 0.0f;
            }
            else
            {
                canSecondaryUse = true;
            }
        }
    }

    public virtual void OnToolCancelled()
    {
        OnPrimaryCancelled();
        OnSecondaryCancelled();
    }

    public virtual void OnUnequip()
    {
        // any behaviour when unequipping tool - like resetting camera fov after ads or something
    }
    #endregion







}
