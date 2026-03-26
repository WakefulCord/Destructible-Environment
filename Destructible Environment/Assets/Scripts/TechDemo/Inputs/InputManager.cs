using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Class References
    private static InputManager _instance;

    PlayerControls playerControls;
    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Input Fields")]
    [SerializeField] private Vector2 movementVector;
    [SerializeField] private Vector2 lookVector;

    [Header("Input Flags")]
    [SerializeField] private bool isUseToolHeld;
    [SerializeField] private bool sprintFlag;
    [SerializeField] private bool jumpFlag;
    #endregion

    #region Properties
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputManager>();
                if(_instance == null )
                {
                    Debug.LogError("InputManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public Vector2 GetMovementInput => movementVector;
    public Vector2 GetLookInput => lookVector;

    public bool SprintFlag => sprintFlag;

<<<<<<< HEAD
    public bool JumpFlag => jumpFlag;
=======
    public bool IsUseToolHeld => isUseToolHeld;
>>>>>>> Player
    #endregion

    #region Start Up
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Movement.Move.performed += ctx => OnMovementInput(ctx);
            playerControls.Movement.Camera.performed += ctx => OnLookInput(ctx);

            playerControls.Movement.Sprint.started += ctx => OnSprintToggle(true);
            playerControls.Movement.Sprint.canceled += ctx => OnSprintToggle(false);

            playerControls.Movement.Jump.performed += ctx => OnJump();

            playerControls.Action.ToolSelect.performed += ctx => OnToolSelect(ctx);

<<<<<<< HEAD
            playerControls.Action.UseTool.performed += ctx => OnUseTool();
            playerControls.Action.AltUseTool.performed += ctx => OnAltUseTool();

            playerControls.Action.UseTool.canceled += ctx => OnToolCancelled();
            playerControls.Action.AltUseTool.canceled += ctx => OnToolCancelled();
=======
            playerControls.Action.UseTool.performed += ctx => { isUseToolHeld = true; OnUseTool(); };
            playerControls.Action.UseTool.canceled += ctx => { isUseToolHeld = false; OnToolCancel(); };
            playerControls.Action.UseToolAlt.performed += ctx => OnAltUseTool();
            playerControls.Action.UseToolAlt.canceled += ctx => OnToolCancel();
            
>>>>>>> Player

        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;

        ToggleCursor(false);
    }
    #endregion

    #region Class Methods
    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }
    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        lookVector = ctx.ReadValue<Vector2>();
    }

    private void OnSprintToggle(bool state)
    {
        sprintFlag = state;
    }

    private void OnToolSelect(InputAction.CallbackContext ctx)
    {
        int inputNum = Mathf.RoundToInt(ctx.ReadValue<float>());
        playerManager.Input_ToolSelect(inputNum);
    }

    private void OnUseTool()
    {
        playerManager.Input_UseTool(false);
    }
<<<<<<< HEAD
    private void OnAltUseTool()
    {
        playerManager.Input_UseTool(true);
    }

    private void OnToolCancelled()
    {
        playerManager.Input_CancelTool();
    }
    private void OnJump()
    {
        playerManager.Input_HandleJump();
    }

=======

    private void OnAltUseTool()
    {
        playerManager.Input_AltUseTool();
    }

    public void ToggleCursor(bool isEnabled)
    {
        Cursor.visible = isEnabled;
        Cursor.lockState = isEnabled ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnToolCancel()
    {
        playerManager.Input_CancelTool();
    }
>>>>>>> Player
    #endregion


}
