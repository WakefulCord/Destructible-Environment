using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Class References
    private static CameraHandler _instance;
    #endregion

    #region Private Fields
    [Header("Camera Fields")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform targetTransform; 
    [SerializeField] private float xRot;
    [SerializeField] private float yRot;
    [SerializeField] private float mouseSensitivity;
    #endregion

    #region Properties
    public static CameraHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraHandler>();

                if (_instance == null)
                {
                    Debug.LogError("CameraHandler has not been assgined");
                }

            }
            return _instance;
        }
    }
    #endregion

    #region Start Up
    public void OnAwake()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    public void OnStart()
    {
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region Class Methods
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }
    private void FollowTarget()
    {
        if (targetTransform != null)
        {
            Vector3 targetPos = targetTransform.position;
            transform.position = targetPos;
        }
    }

    private void HandleRotation(float mouseX, float mouseY)
    {
        
        yRot += mouseX * mouseSensitivity;
        xRot -= mouseY * mouseSensitivity;

      
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        
        mainCam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);

      
        PlayerManager.Instance.RotatePlayer(new Vector3(0, yRot, 0));
    }
    #endregion

    #region Update Methods
    public void OnUpdate(float mouseX, float mouseY)
    {
        FollowTarget();
        HandleRotation(mouseX, mouseY);
    }

    
    #endregion
}
