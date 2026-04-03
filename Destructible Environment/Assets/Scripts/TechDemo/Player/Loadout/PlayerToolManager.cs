using UnityEngine;

public class PlayerToolManager : MonoBehaviour // manages currently equipped tool
{
    #region Class References
    PlayerLoadout playerLoadout;
    #endregion

    #region Private Fields
   
    [Header(" Tool Maanager fields")]
    [SerializeField] private DestructionTool currentEquippedTool;
    [SerializeField] private ToolBehaviour activeToolBehaviour;

    [SerializeField] private Transform tooltransform;
    #endregion

    #region Properties
    public bool HasActiveTool => activeToolBehaviour != null;
    #endregion

    #region Start Up


    public void OnAwake()
    {
        playerLoadout = GetComponent<PlayerLoadout>();
    }
    public void OnStart()
    {


    }
    #endregion

    #region Class Methods 

    public void OnUpdate(bool isPrimaryHeld, bool isSecondaryHeld)
    {
        float dt = Time.deltaTime;
        if (HasActiveTool)
        {
            activeToolBehaviour.OnToolUpdate(dt);

            DestructionTool toolData = activeToolBehaviour.GetToolData;
            if (toolData == null)
            {
                return;
            }

            if (isPrimaryHeld && activeToolBehaviour.IsPrimaryHoldMode() && activeToolBehaviour.CanUseTool)
            {
                activeToolBehaviour.OnPrimaryUse();
            }

            if (isSecondaryHeld && activeToolBehaviour.IsSecondaryHoldMode() && activeToolBehaviour.CanSecondaryUseTool)
            {
                activeToolBehaviour.OnSecondaryUse();
            }
        }
    }
    

    private void UnequipCurrentTool()
    {
        
        if (activeToolBehaviour != null)
        {
            activeToolBehaviour.OnUnequip(); 
            Destroy(activeToolBehaviour.gameObject); // might have to replaec with safer version  
        }
        currentEquippedTool = null;
        activeToolBehaviour = null;
    }

    

    private void EquipNewTool(DestructionTool tool)
    {
        currentEquippedTool = tool;

        if (currentEquippedTool == null)
        {
            Debug.LogWarning($"Tool {currentEquippedTool.GetName}: Cannot equip.");
            return;
        }
        if (currentEquippedTool.GetPrefab == null)
        {
            Debug.LogWarning($"Tool {currentEquippedTool.GetName}: Does not have a prefab assigned, cannot equip.");
            return;
        }
        GameObject toolObj = Instantiate(currentEquippedTool.GetPrefab, tooltransform);
        toolObj.transform.localPosition = Vector3.zero;
        toolObj.transform.localRotation = Quaternion.identity;

        activeToolBehaviour = toolObj.GetComponent<ToolBehaviour>();
        if (activeToolBehaviour == null)
        {
            Debug.LogWarning($"Tool {currentEquippedTool.GetName}: Is missing behaviour scriptm cannot equip.");
            UnequipCurrentTool(); // clear tool data to prevent issues with broken tool
            return;
        }
        activeToolBehaviour.OnToolInit(tool);
    }
    #endregion

    #region Input
    public void HandleEquipTool(int slot)
    {
        UnequipCurrentTool();
        EquipNewTool(GetToolInSlot(slot));
    }
    public void HandleUseTool()
    {
        if (HasActiveTool && activeToolBehaviour.IsPrimarySingleClickMode())
        {
            activeToolBehaviour.OnPrimaryUse();
        }
    }

    public void HandleAltUseTool()
    {
        if (HasActiveTool && activeToolBehaviour.IsSecondarySingleClickMode())
        {
            activeToolBehaviour.OnSecondaryUse();
        }
    }

    public void HandleCancelPrimaryTool()
    {
        if (HasActiveTool)
        {
            activeToolBehaviour.OnPrimaryCancelled();
        }
    }

    public void HandleCancelSecondaryTool()
    {
        if (HasActiveTool)
        {
            activeToolBehaviour.OnSecondaryCancelled();
        }
    }
    #endregion

    #region Helper
    public DestructionTool GetToolInSlot(int slotNum)
    {
        return playerLoadout.GetTools[slotNum - 1];
    }
    #endregion
}
