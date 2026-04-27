using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    #region Class References
    private static PlayerUIManager _instance;

    // UI References
    private UI_WeaponBar weaponBar;

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
        // On Awake, get references to the UI elements that will be used by the PlayerManager to update the UI when needed.
        weaponBar = GetComponentInChildren<UI_WeaponBar>();
    }
    public void OnStart()
    {
        
    }
    #endregion

    #region Class Methods

    public void NotifyToolSelected(int slotIndex)
    {
        // This method will be called by PlayerManager for updating the UI when a new tool is selected.
        // It will pass the slot index (1-based) of the selected tool, and the UI can then update the highlight on the weapon bar accordingly.
        weaponBar.Select_WeaponSlot(slotIndex);
    }

    #endregion

    #region Update Methods
    public void OnUpdate()
    {

    }
    #endregion


}
