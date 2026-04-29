using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Class References
    private static PlayerManager _instance;

    GameManager gameManager;

    InputManager inputManager;

    PlayerMovement playerMovement;
    PlayerToolManager playerToolManager;
    PlayerLoadout playerLoadout;
    #endregion

    #region Private Fields
    [SerializeField] private bool isGrounded;
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

    public bool IsGrounded => isGrounded;
    public PlayerLoadout GetLoadout => playerLoadout;

    public Camera GetMainCam => gameManager.GetMainCam;
    #endregion

    #region Start Up


    public void OnAwake()
    {
        gameManager = GameManager.Instance;
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
    public void TeleportPlayer(Transform t)
    {
        transform.position = t.position;
        transform.rotation = t.rotation;
    }

    public void OnUpdate()
    {
        isGrounded = playerMovement.OnUpdate(inputManager.GetMovementInput.x, inputManager.GetMovementInput.y, IsSprinting);
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

        // Notify the UI of the tool change so it can update the icon and cooldown display on UI - WeaponHotbar
        PlayerUIManager.Instance?.NotifyToolSelected(slotNum);
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

    public void Input_Jump()
    {
        if (isGrounded)
            playerMovement.HandleJump();
    }
    #endregion
}
