using System.Collections.Generic;
using UnityEngine;

public class ExplosiveDetonateBehaviour : ToolBehaviour
{
    
    private List<ExplosiveBehaviour> placedExplosives;

    public ExplosiveDetonateTool GetDetonateTool => (ExplosiveDetonateTool)GetToolData;

    public override void OnToolInit(DestructionTool t)
    {
        base.OnToolInit(t);

     
        placedExplosives = new List<ExplosiveBehaviour>();
    }

    public override void OnToolUpdate(float dt)
    {
        base.OnToolUpdate(dt);

    }

    protected override void PrimaryUseBehaviour()
    {
        base.PrimaryUseBehaviour();
        PlaceBomb();
    }

    protected override void SecondaryUseBehaviour()
    {
        base.SecondaryUseBehaviour();
        DetonateAll();
    }

    private void PlaceBomb()
    {
        

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
            return;

        GameObject bomb = Instantiate(
            GetDetonateTool.ExplosivePrefab,
            hit.point + hit.normal * 0.05f,
            Quaternion.LookRotation(hit.normal)
        );

        ExplosiveBehaviour explosive = bomb.GetComponent<ExplosiveBehaviour>();
        if (explosive != null)
        {
            explosive.OnExplosiveInit(GetDetonateTool);
            placedExplosives.Add(explosive);
        }
    }

    private void DetonateAll()
    {
        for (int i = placedExplosives.Count - 1; i >= 0; i--)
        {
            if (placedExplosives[i] != null)
            {
                placedExplosives[i].Detonate();
            }
        }
        placedExplosives.Clear();
    }
}
