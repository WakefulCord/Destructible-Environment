using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	#region Class Referenecs
	private static InputManager _instance;

    PlayerManager playerManager;
    PlayerControls playerControls;
    #endregion

    #region Private Fields
    [Header("Input Fields")]
    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 lookInput;
    [SerializeField] private float terraInput;
    [SerializeField] private float flyInput;

    [Header("Flags")]
    [SerializeField] private bool buildFlag;
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

                if (_instance == null )
                {
                    Debug.LogError("Input manager has not been assgined");
                }

            }
            return _instance;
        }
    }

    public float Horizontal => movementInput.x;
    public float Vertical => movementInput.y;

    public float MouseX => lookInput.x;
    public float MouseY => lookInput.y;

    public float FlyInput => flyInput;
    public float TerraformInput => terraInput;

    public bool BuildFlag => buildFlag;
    public bool SprintFlag => sprintFlag;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        playerManager = PlayerManager.Instance;
    }

    public void OnStart()
    {

    }
    #endregion

    #region Class References
    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Movement.Move.performed += ctx => OnMovementInput(ctx);
            playerControls.Movement.Look.performed += ctx => OnLookInput(ctx);

            playerControls.Movement.Sprint.started += ctx => OnToggleSprint();
            playerControls.Movement.Sprint.canceled += ctx => OnToggleSprint();

            playerControls.Movement.Fly.performed += ctx => OnFlyInput(ctx);

            playerControls.Actions.ToggleTerraMode.performed += ctx => OnTerraformToggle();
            playerControls.Actions.Terraform.performed += ctx => OnTerraformInput(ctx);

        }
        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }
    private void OnMovementInput(InputAction.CallbackContext ctx) { movementInput = ctx.ReadValue<Vector2>(); }

    private void OnLookInput(InputAction.CallbackContext ctx) { lookInput = ctx.ReadValue<Vector2>();  }

    private void OnFlyInput(InputAction.CallbackContext ctx) { flyInput = ctx.ReadValue<float>(); }
    private void OnTerraformInput(InputAction.CallbackContext ctx)
    {
        terraInput = ctx.ReadValue<float>();
        playerManager.Input_OnTerraform(terraInput);
    }

    private void OnTerraformToggle()
    {
        buildFlag = !buildFlag;

        playerManager.Input_ToggleBuildMode(buildFlag);
    }

    private void OnToggleSprint()
    {
        sprintFlag = !sprintFlag;
    }
    #endregion

}
