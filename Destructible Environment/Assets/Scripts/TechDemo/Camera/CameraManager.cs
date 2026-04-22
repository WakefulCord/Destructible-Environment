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
    [SerializeField] private Camera mainCam;

    [SerializeField][Range(0f, 1f)] private float mouseSens;

    private float xRot;
    private float yRot;

    [SerializeField] private int defaultFOV = 60;
    
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
  
    public void OnAwake() 
    {
        mainCam = Camera.main;
        inputManager = InputManager.Instance;
        playerManager = PlayerManager.Instance;
    }

    public void OnStart()
    {

    }
    #endregion

    #region Class Methods
   
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

        Vector3 targetRot = new Vector3(xRot, yRot, 0); // mainCam rotation

        mainCam.transform.rotation = Quaternion.Euler(targetRot);

        //player roation - temp
        targetRot = new Vector3(0, yRot, 0);
        playerManager.transform.rotation = Quaternion.Euler(targetRot);
    }

    public void EnableADS(float newFOV)
    {
        mainCam.fieldOfView = newFOV;
    }

    public void DisableADS()
    {
        mainCam.fieldOfView = defaultFOV;
    }
    #endregion
}
