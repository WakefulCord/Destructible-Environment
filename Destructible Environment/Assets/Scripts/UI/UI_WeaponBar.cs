using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI - Weapon Bar, responsible for displaying the player's currently equipped weapons and allowing them to switch between them.
/// It generates weapon icons using the UI_WeaponPreview component and highlights the active weapon slot when selected, from the player's loadout information provided by the PlayerLoadout component.
/// Also, it listens for input to change the active weapon slot and updates the UI accordingly.
/// </summary>

public class UI_WeaponBar : MonoBehaviour
{

    // ============= References & Fields =========
    [Header("References")]
    [SerializeField] private GameObject weaponTemplate;                                         // Drag the Weapon GameObject here (Weapon_00)
    [SerializeField] private PlayerLoadout playerLoadout;                                       // Drag the Player GameObject here (Player) to get the loadout info

    private UI_WeaponPreview _weaponPreview;                                                    // Reference to the UI_WeaponPreview component for generating weapon icons


    // ============= Internal State =============
    private readonly List<GameObject> _items = new();
    private int _activeSlot = -1;


    // ============= Unity Methods =============
    private void Start()
    {
        _weaponPreview = GetComponent<UI_WeaponPreview>();                                      // Get the UI_WeaponPreview component for generating weapon icons
        Initialise_WeaponSlots();
    }


    // ============= Class Methods =============
    private void Initialise_WeaponSlots()
    {
        // 1. Hide the weapon template (we'll clone it for each slot)
        weaponTemplate.SetActive(false);

        // 2. Create clones for each weapon slot based on the player's loadout
        for (int i = 0; i < playerLoadout.ToolCount; i++)
        {
            int slotIndex = i + 1;

            // Clone the template and set it up
            GameObject clone = Instantiate(weaponTemplate, weaponTemplate.transform.parent);
            clone.name = $"Item_{i:D2}";
            clone.SetActive(true);

            // Set the hotkey number ("1", "2", "3"...)
            TMP_Text label = clone.transform.Find("Hotkey/Text_Input").GetComponent<TMP_Text>();
            label.text = slotIndex.ToString();

            // Set the weapon icon
            Set_WeaponIcon(clone, i);

            // Ensure the highlight is off by default
            clone.transform.Find("Item/isSelected").gameObject.SetActive(false);

            // Add the clone to our list for easy access later
            _items.Add(clone);
        }
    }

    private void Set_WeaponIcon(GameObject clone, int index)
    {
        // Find the Icon image in the clone and set the sprite from the tool's icon
        Image icon = clone.transform.Find("Item/Icon").GetComponent<Image>();
        Sprite preview = _weaponPreview.Generate_WeaponPreview(playerLoadout.GetTools[index].GetPrefab);
        icon.sprite = preview;
    }

    // Call this when a weapon is selected 
    public void Select_WeaponSlot(int slotIndex)
    {
        // 1. Turn off previous highlight
        if (_activeSlot >= 1 && _activeSlot <= _items.Count)
            _items[_activeSlot - 1].transform.Find("Item/isSelected").gameObject.SetActive(false);

        // 2. Turn on new highlight
        _activeSlot = slotIndex;
        _items[_activeSlot - 1].transform.Find("Item/isSelected").gameObject.SetActive(true);
    }
}