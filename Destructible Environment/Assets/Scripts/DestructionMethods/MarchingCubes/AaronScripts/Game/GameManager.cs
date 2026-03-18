using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Class References
    private static GameManager _instance;

    InputManager inputManager;
    PlayerManager playerManager;
    CameraHandler cameraHandler;
    #endregion

    #region Private Fields

    #endregion

    #region Properties
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();

                if (_instance == null)
                {
                    Debug.LogError("GameManager has not been assgined");
                }

            }
            return _instance;
        }
    }
    #endregion

    #region Start Up
    private void Awake()
    {
        inputManager = InputManager.Instance;
        playerManager = PlayerManager.Instance;
        cameraHandler = CameraHandler.Instance;


        inputManager.OnAwake();
        cameraHandler.OnAwake();
        playerManager.OnAwake();
    }

    private void Start()
    {
        inputManager.OnStart();
        playerManager.OnStart();
        cameraHandler.OnStart();
    }
    #endregion

    #region Class Methods

    #endregion

    #region Update Methods
    private void Update()
    {
        playerManager.OnUpdate();
        cameraHandler.OnUpdate(inputManager.MouseX, inputManager.MouseY);
    }
    #endregion
}
