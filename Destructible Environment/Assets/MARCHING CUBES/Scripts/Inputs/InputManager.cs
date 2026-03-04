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

            playerControls.Actions.AddTerrain.performed += ctx => OnAddTerrain();
            playerControls.Actions.SubTerrain.performed += ctx => OnSubTerrain();

        }
        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }
    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnAddTerrain()
    {

    }
    public void OnSubTerrain()
    {

    }
    #endregion

}
