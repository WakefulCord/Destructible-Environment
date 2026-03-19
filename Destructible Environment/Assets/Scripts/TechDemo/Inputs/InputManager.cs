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

    public bool JumpFlag => jumpFlag;
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

            playerControls.Action.UseTool.performed += ctx => OnUseTool();

        }
        playerControls.Enable();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
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
        playerManager.Input_UseTool();
    }

    private void OnJump()
    {
        playerManager.Input_HandleJump();
    }

    public void OnLateUpdate()
    {
        ResetJumpFlag();
    }

    private void ResetJumpFlag()
    {
        jumpFlag = false;
    }
    #endregion


}
