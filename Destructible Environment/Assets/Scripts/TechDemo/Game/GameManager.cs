using System.Collections.Generic;
using System.Linq;
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
    private Camera mainCam;

    [SerializeField] private List<DestructableBehaviour> destructables = new List<DestructableBehaviour>();
    private readonly List<DestructableBehaviour> pendingAdd = new List<DestructableBehaviour>();
    private readonly List<DestructableBehaviour> pendingRemove = new List<DestructableBehaviour>();
    private bool isUpdatingDestructables;
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

    public Camera GetMainCam
    {
        get
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            return mainCam;
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

        cameraManager.OnAwake();
        playerManager.OnAwake();
        //inputManager.OnAwake();
        playerUIManager.OnAwake();
    }

    private void Start()
    {
        OnStart();
        
        
        destructables.Clear();

        DestructableBehaviour[] initialDestruction = FindObjectsByType<DestructableBehaviour>(FindObjectsSortMode.None);

        for (int i = 0; i < initialDestruction.Count(); i++)
        {
            RegisterDestructable(initialDestruction[i]);
        }


        //temp till better fix

        HVoxelHouse house = FindFirstObjectByType<HVoxelHouse>();
        house.OnStart();
        
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

        playerUIManager.OnUpdate();

        FlushDestructionQueues();

        isUpdatingDestructables = true;
        foreach (DestructableBehaviour detruct in destructables)
        {
            if (detruct == null)
            {
                continue;
            }

            detruct.UpdateDestruction();
        }
        isUpdatingDestructables = false;

        FlushDestructionQueues();
    }


    #endregion

    #region Destruction Process
    public void RegisterDestructable(DestructableBehaviour destructable)
    {
        if (destructable == null)
        {
            return;
        }

        if (pendingRemove.Contains(destructable))
        {
            pendingRemove.Remove(destructable);
        }

        if (destructables.Contains(destructable) || pendingAdd.Contains(destructable))
        {
            return;
        }

        pendingAdd.Add(destructable);
        
        if (!isUpdatingDestructables)
        {
            FlushDestructionQueues();
        }
    }

    public void UnregisterDestructable(DestructableBehaviour destructable)
    {
        if (destructable == null)
        {
            return;
        }

        if (pendingAdd.Contains(destructable))
        {
            pendingAdd.Remove(destructable);
        }

        if (pendingRemove.Contains(destructable))
        {
            return;
        }

        pendingRemove.Add(destructable);

        if (!isUpdatingDestructables)
        {
            FlushDestructionQueues();
        }
    }

    private void FlushDestructionQueues()
    {
        //remove waiting destruction
        if (pendingRemove.Count > 0)
        {
            List<DestructableBehaviour> removalsToProcess = new List<DestructableBehaviour>(pendingRemove);
            pendingRemove.Clear();

            foreach (DestructableBehaviour destructable in removalsToProcess)
            {
                destructables.Remove(destructable);
            }
        }


        //add waiting destruction 
        if (pendingAdd.Count > 0)
        {
            List<DestructableBehaviour> additionsToProcess = new List<DestructableBehaviour>(pendingAdd);
            pendingAdd.Clear();

            foreach (DestructableBehaviour destructable in additionsToProcess)
            {
                if (destructable != null && !destructables.Contains(destructable))
                {
                    destructables.Add(destructable);
                    destructable.InitializeDestruction();
                }
            }
        }
    }
    #endregion
}
