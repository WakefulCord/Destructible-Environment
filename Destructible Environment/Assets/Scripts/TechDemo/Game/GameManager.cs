using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Class References
    private static GameManager _instance;

    PlayerManager playerManager;
    InputManager inputManager;
    PlayerUIManager playerUIManager;
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

        playerManager.OnAwake();
        //inputManager.OnAwake();
        playerUIManager.OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void OnStart()
    {
        playerManager.OnStart();
        playerUIManager.OnStart();
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
    }
    #endregion
}
