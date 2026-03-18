using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Class References
    private static CameraManager _instance;

    InputManager inputManager;
    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Camera Fields")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField][Range(0f, 1f)] private float mouseSens;

    private float xRot;
    private float yRot;
    #endregion

    #region Properties
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraManager>();
                if (_instance == null )
                {
                    Debug.LogError("Camera Handler has not been assigned");
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
    public void OnAwake() 
    {
        inputManager = InputManager.Instance;
        playerManager = PlayerManager.Instance;
    }
    #endregion

    #region Class Methods
    private void Update()
    {
        OnUpdate();
    }
    public void OnUpdate()
    {
        FollowTarget();
        HandleRotation(inputManager.GetLookInput.x, inputManager.GetLookInput.y);
    }

    private void FollowTarget()
    {
        Vector3 targetPos = targetTransform.position;
        transform.position = targetPos;

    }

    private void HandleRotation(float x,float y)
    {
        xRot -= y * mouseSens;
        yRot += x * mouseSens;
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        Vector3 targetRot = new Vector3(xRot, yRot, 0); // camera rotation

        cameraTransform.rotation = Quaternion.Euler(targetRot);

        //player roation - temp
        targetRot = new Vector3(0, yRot, 0);
        playerManager.transform.rotation = Quaternion.Euler(targetRot);
    }
    #endregion
}
