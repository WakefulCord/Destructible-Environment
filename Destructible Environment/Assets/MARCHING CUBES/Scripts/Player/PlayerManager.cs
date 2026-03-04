using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Class References
    private static PlayerManager _instance;

    PlayerMovement playerMovement;
    PlayerTerraform playerTerraform;
    #endregion

    #region Private Fields

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
    #endregion

    #region Start Up
    public void OnAwake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerTerraform = GetComponent<PlayerTerraform>();
        playerMovement.OnAwake();
        playerTerraform.OnAwake();
    }

    public void OnStart()
    {
        playerMovement.OnStart();
        playerTerraform.OnStart();
    }
    #endregion

    #region Class Methods
    public void RotatePlayer(Vector3 targetRot)
    {
        transform.rotation = Quaternion.Euler(targetRot);
    }
    #endregion

    #region Update Methods
    public void OnUpdate(float hor,float vert)
    {
        playerMovement.OnUpdate(hor,vert);

    }
    #endregion

    #region Input Fields
    public void HandleAddTerrain()
    {
        

    }

    public void HandleRemoveTerrain()
    {
        
    }
    #endregion
}
