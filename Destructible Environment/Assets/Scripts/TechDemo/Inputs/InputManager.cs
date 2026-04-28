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
    [SerializeField] private bool isPrimaryToolHeld;
    [SerializeField] private bool isSecondaryToolHeld;
    [SerializeField] private bool sprintFlag;
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

    public bool IsPrimaryToolHeld => isPrimaryToolHeld;
    public bool IsSecondaryToolHeld => isSecondaryToolHeld;
    #endregion

    #region Start Up
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Movement.Move.performed += ctx => OnMovementInput(ctx);
            playerControls.Movement.Camera.performed += ctx => OnLookInput(ctx);

            playerControls.Movement.Move.canceled += ctx => OnMovementCancel();
            playerControls.Movement.Camera.canceled += ctx => OnLookCancel();


            playerControls.Movement.Sprint.started += ctx => OnSprintToggle(true);
            playerControls.Movement.Sprint.canceled += ctx => OnSprintToggle(false);

            playerControls.Movement.Jump.performed += _ => OnJumpInput();

            playerControls.Action.ToolSelect.performed += ctx => OnToolSelect(ctx);

            playerControls.Action.UseTool.performed += ctx => { isPrimaryToolHeld = true; OnUseTool(); };
            playerControls.Action.UseTool.canceled += ctx => { isPrimaryToolHeld = false; OnPrimaryToolCancel(); };
            playerControls.Action.UseToolAlt.performed += ctx => { isSecondaryToolHeld = true; OnAltUseTool(); };
            playerControls.Action.UseToolAlt.canceled += ctx => { isSecondaryToolHeld = false; OnSecondaryToolCancel(); };
            

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

    private void OnMovementCancel()
    {
        movementVector = Vector2.zero;
    }

    private void OnLookCancel()
    {
        lookVector = Vector2.zero;

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
        playerManager.Input_UseTool();
    }

    private void OnAltUseTool()
    {
        playerManager.Input_AltUseTool();
    }

    public void ToggleCursor(bool isEnabled)
    {
        Cursor.visible = isEnabled;
        Cursor.lockState = isEnabled ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    private void OnPrimaryToolCancel()
    {
        playerManager.Input_CancelPrimaryTool();
    }

    private void OnSecondaryToolCancel()
    {
        playerManager.Input_CancelSecondaryTool();
    }

    private void OnJumpInput()
    {
        playerManager.Input_Jump();
    }
    #endregion


}
