using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Class References
    private static PlayerManager _instance;
    InputManager inputManager;

    PlayerMovement playerMovement;
    PlayerToolManager playerToolManager;
    PlayerLoadout playerLoadout;
    #endregion

    #region Private Fields

    #endregion

    #region Properties
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerManager>();
                if (_instance == null)
                {
                    Debug.LogError("PlayerManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public bool IsSprinting => inputManager.SprintFlag;
    #endregion

    #region Start Up
    private void Awake() // temp
    {
        OnAwake();
    }
    private void Start() // temp
    {
        OnStart();
    }

    public void OnAwake()
    {
        inputManager = InputManager.Instance;

        playerMovement = GetComponent<PlayerMovement>();
        playerLoadout = GetComponent<PlayerLoadout>();
        playerToolManager = GetComponent<PlayerToolManager>();
        playerMovement.OnAwake();

        playerToolManager.OnAwake();
    }
    public void OnStart()
    {
        playerMovement.OnStart();
        playerToolManager.OnStart();
    }
    #endregion

    #region Class Methods
    private void Update() // temp
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        playerMovement.OnUpdate(inputManager.GetMovementInput.x, inputManager.GetMovementInput.y, IsSprinting);
        playerToolManager.OnUpdate(inputManager.IsPrimaryToolHeld, inputManager.IsSecondaryToolHeld);
    }
    #endregion

    #region Input Methods
    public void Input_ToolSelect(int slotNum) // 1-5 rn
    {
        if (slotNum > playerLoadout.ToolCount)
        {
            Debug.Log("No tool in slot " + slotNum + "!");
            return;
        }
        playerToolManager.HandleEquipTool(slotNum);
    }

    public void Input_UseTool()
    {
        playerToolManager.HandleUseTool();
    }

    public void Input_AltUseTool()
    {
        playerToolManager.HandleAltUseTool();
    }

    public void Input_CancelPrimaryTool()
    {
        playerToolManager.HandleCancelPrimaryTool();
    }

    public void Input_CancelSecondaryTool()
    {
        playerToolManager.HandleCancelSecondaryTool();
    }
    #endregion
}
