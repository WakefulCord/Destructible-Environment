using UnityEngine;

public class ToolBehaviour : MonoBehaviour
{
    DestructionTool tool;

    protected DestructionTool GetToolData => tool;

    public virtual void OnToolInit(DestructionTool t)
    {
        tool = t;
    }

    public virtual void OnToolUpdate()
    {

    }

    public virtual void OnToolUse()
    {
        Debug.Log("Used " + GetToolData.GetName);
    }
}

