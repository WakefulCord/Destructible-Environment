using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Class References
    private static GameManager _instance;

    CameraManager cameraManager;
    PlayerManager playerManager;
    InputManager inputManager;
    PlayerUIManager playerUIManager;
    #endregion

    #region Private Fields
    [SerializeField] private DestructableBehaviour[] destructables;
    #endregion

    #region Properties
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance != null)
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
        OnAwake();
    }

    private void OnAwake()
    {
        playerManager = PlayerManager.Instance;
        inputManager = InputManager.Instance;
        playerUIManager = PlayerUIManager.Instance;
        cameraManager = CameraManager.Instance;

        playerManager.OnAwake();
        //inputManager.OnAwake();
        playerUIManager.OnAwake();
        cameraManager.OnAwake();
    }

    private void Start()
    {
        OnStart();

        destructables = FindObjectsByType<DestructableBehaviour>(FindObjectsSortMode.None);

        foreach (DestructableBehaviour detruct in destructables)
        {
            detruct.InitializeDestruction();
        }
    }

    private void OnStart()
    {
        playerManager.OnStart();
        playerUIManager.OnStart();
        cameraManager.OnStart();
    }
    #endregion

    #region Class Methods
    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        playerManager.OnUpdate();
        cameraManager.OnUpdate();
    }
    #endregion
}
