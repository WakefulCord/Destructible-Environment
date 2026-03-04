using UnityEngine;

public class PlayerTerraform : MonoBehaviour
{
    #region Class References
    Camera mainCam;
    TerrainManager terrainManager;
    #endregion

    #region Private Fields
    
    #endregion

    #region Properties

    #endregion

    #region Start Up
    public void OnAwake()
    {
        mainCam = Camera.main;
        terrainManager = TerrainManager.Instance;
    }

    public void OnStart()
    {

    }
    #endregion

    #region Class Methods
    
    
    #endregion
        
    
}
