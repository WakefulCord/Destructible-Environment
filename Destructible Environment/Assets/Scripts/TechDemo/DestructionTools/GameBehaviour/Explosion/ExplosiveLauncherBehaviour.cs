using UnityEngine;

public class ExplosiveLauncherBehaviour : ToolBehaviour
{
  

    public ExplosiveLauncherTool GetLauncherTool => (ExplosiveLauncherTool)GetToolData;

    public override void OnToolInit(DestructionTool t, Camera playerCam)
    {
        base.OnToolInit(t, playerCam);

      
    }

    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

      
    }

    

    protected override void PrimaryUseBehaviour()
    {
        base.PrimaryUseBehaviour();
        Launch();
    }

    protected override void SecondaryUseBehaviour()
    {
        base.SecondaryUseBehaviour();

        //Show projectile prediction line?
        Debug.Log("No alt Use logic yet!");
    }

    private void Launch()
    {
        
        
        Vector3 origin = mainCam.transform.position + mainCam.transform.forward;

        if (effectPoint != null) // tip of launcher
        { 
            origin = effectPoint.position + effectPoint.forward * 0.01f; 
        }
        // calculate launch direction use mainCam forward so in line with hit marker
        Vector3 direction = mainCam.transform.forward;

        GameObject projectile = Instantiate(
            GetLauncherTool.ExplosivePrefab,
            origin,
            Quaternion.LookRotation(direction)
        );

        ExplosiveBehaviour explosive = projectile.GetComponent<ExplosiveBehaviour>();
        if (explosive != null)
        {
            explosive.OnImpactExplosiveInit(GetLauncherTool);
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * GetLauncherTool.LaunchSpeed;
        }
    }
}
