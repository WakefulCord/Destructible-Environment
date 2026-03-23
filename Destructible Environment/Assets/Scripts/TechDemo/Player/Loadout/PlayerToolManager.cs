using UnityEngine;

public class PlayerToolManager : MonoBehaviour // manages currently equipped tool
{
    #region Class References
    PlayerLoadout playerLoadout;
    #endregion

    #region Private Fields
   
    [Header(" Tool Maanager fields")]
    [SerializeField] private DestructionTool currentEquippedTool;
    [SerializeField] private ToolBehaviour activeTool;

    [SerializeField] private Transform tooltransform;
    #endregion

    #region Properties
    public bool HasActiveTool => activeTool != null;
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

    public void OnUpdate(bool isUseHeld)
    {
        float dt = Time.deltaTime;
        if (HasActiveTool)
        {
            activeTool.OnToolUpdate(dt);

            // Automatic fire logic for any tool
            DestructionTool toolData = activeTool.GetToolData;
            if (toolData != null && toolData.IsAutomatic)
            {
                if (isUseHeld && activeTool.CanUseTool)
                {
                    activeTool.OnToolUse();
                }
            }
        }
    }
    

    private void UnequipCurrentTool()
    {
        currentEquippedTool = null;
        DestroyTool();
    }

    private void DestroyTool()
    {
        if (activeTool != null)
        {

            Destroy(activeTool.gameObject); // might have to replaec with safer version  
        }
    }

    private void EquipNewTool(DestructionTool tool)
    {
        currentEquippedTool = tool;

        GameObject toolObj = Instantiate(currentEquippedTool.GetPrefab, tooltransform);
        toolObj.transform.localPosition = Vector3.zero;
        toolObj.transform.localRotation = Quaternion.identity;

        activeTool = toolObj.GetComponent<ToolBehaviour>();
        activeTool.OnToolInit(tool);
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
        if (HasActiveTool)
        {
            activeTool.OnToolUse();
        }
    }

    public void HandleAltUseTool()
    {
        if (HasActiveTool)
        {
            activeTool.OnToolAltUse();
        }
    }

    public void HandleCancelTool()
    {
        if (HasActiveTool)
        {
            activeTool.OnToolCancelled();
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
