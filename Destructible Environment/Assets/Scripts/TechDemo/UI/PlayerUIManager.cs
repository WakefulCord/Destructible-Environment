using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    #region Class References
    private static PlayerUIManager _instance;
    
    #endregion

    #region Private Fields

    #endregion

    #region Properties
    public static PlayerUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerUIManager>();
                if (_instance == null)
                {
                    Debug.LogError("PlayerUIManager has not been assigned");
                }
            }
            return _instance;
        }
    }


    #endregion

    #region Start Up
   
    public void OnAwake()
    {
        
    }
    public void OnStart()
    {
        
    }
    #endregion

    #region Class Methods
    public void SetUpToolbarUI()
    {

    }
    #endregion

   

   
}
