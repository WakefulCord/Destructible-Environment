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

    public virtual void OnToolAltUse()
    {
        //right click
            //aim
            //subtract
    }

    public virtual void OnToolCancelled() // what to do when use button has stopped being pressed - e.g reset terraform val
    {

    }
}

