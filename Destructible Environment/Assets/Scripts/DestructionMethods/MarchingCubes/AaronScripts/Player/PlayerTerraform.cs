
using UnityEngine;

public class PlayerTerraform : MonoBehaviour
{
    #region Class References
    Camera mainCam;
    TerrainManager terrainManager;
    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Terraform vals")]
    [SerializeField] private float digStrength;
    [SerializeField] private int digRadius;
    [SerializeField] private float terraDist = 10f;
    private float tVal;
    [Header("Terraform Cooldown ")]
    [SerializeField] private float terraformCooldown = 0.1f;
    private float lastTerraformTime;

    [SerializeField] private bool isTeraforming;
    [Header("Terraform Visuals")]
    [SerializeField] private GameObject terraformIndicator;
    [SerializeField] private LayerMask terrainLayer;

    #endregion

    #region Properties
    public bool InBuildMode => playerManager.InBuildMode;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        mainCam = Camera.main;
        terrainManager = TerrainManager.Instance;
        playerManager = PlayerManager.Instance;
    }

    public void OnStart()
    {
        isTeraforming = false;
        lastTerraformTime = 0f;
    }
    #endregion

    #region Class Methods
   
    public void HandleTerraform(float terraVal)
    {
        if (InBuildMode)
        {
            tVal = terraVal; // cache 1/-1 to know to add/subtract
            isTeraforming = (terraVal != 0f); // start/stop terraforming based on input
        }
        else
        {
            isTeraforming = false;
        }
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        if (InBuildMode)
        {
            Vector3 terraTargetPos = GetTerraformWorldPos(); // cache position for this frame

            if (terraTargetPos == Vector3.zero)
            {
                ToggleIndicator(false);
            }
            else
            {
                ToggleIndicator(true);
                terraformIndicator.transform.position = terraTargetPos;

                //terraform if cooldown has finish
                if (isTeraforming && Time.time - lastTerraformTime >= terraformCooldown)
                {
                    terrainManager.UpdateDensityAndRegenerate(terraTargetPos, tVal * digStrength, digRadius);
                    lastTerraformTime = Time.time;
                }
            }
        }
    }

    private Vector3 GetTerraformWorldPos()
    {
        Vector3 origin = playerManager.GetCamTarget.position;

        RaycastHit hit;
        if (Physics.Raycast(origin, mainCam.transform.forward, out hit, terraDist, terrainLayer))
        {
            return hit.point;
        }
        return Vector3.zero; // null
    }
    #endregion

    #region Other
    private void ToggleIndicator(bool state)
    {
        terraformIndicator.SetActive(state);
    }
    #endregion


}
