using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Class References
    private static PlayerManager _instance;

    PlayerMovement playerMovement;
    PlayerTerraform playerTerraform;

    InputManager inputManager;
    #endregion

    #region Private Fields
    [Header("Fields")]
    [SerializeField] private Transform playerCamRef;

    [Header("Player Bools")]
    [SerializeField] private bool inBuildMode;
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
                    Debug.LogError("PlayerManager has not been assgined");
                }

            }
            return _instance;
        }
    }

    public Transform GetCamTarget => playerCamRef;
    public bool InBuildMode => inBuildMode;
    public bool IsSprinting => inputManager.SprintFlag;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        inputManager = InputManager.Instance;

        playerMovement = GetComponent<PlayerMovement>();
        playerTerraform = GetComponent<PlayerTerraform>();
        playerMovement.OnAwake();
        playerTerraform.OnAwake();
    }

    public void OnStart()
    {
        playerMovement.OnStart();
        playerTerraform.OnStart();

        CameraHandler.Instance.SetTarget(GetCamTarget);
    }
    #endregion

    #region Class Methods
    public void RotatePlayer(Vector3 targetRot)
    {
        transform.rotation = Quaternion.Euler(targetRot);
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        playerMovement.OnUpdate(inputManager.Horizontal, inputManager.Vertical, inputManager.FlyInput, IsSprinting);
        playerTerraform.OnUpdate();
    }
    #endregion

    #region Input Fields
    public void Input_OnTerraform(float terraVal)
    {
        if (!InBuildMode) return;
        playerTerraform.HandleTerraform(terraVal);
    }

    public void Input_ToggleBuildMode(bool isBuild)
    {
        inBuildMode = isBuild;

    }
    #endregion
}
