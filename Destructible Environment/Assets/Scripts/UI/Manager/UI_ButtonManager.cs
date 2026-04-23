using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ButtonManager : MonoBehaviour
{
    // ========= Serialized Fields =========
    [SerializeField] private SceneReference targetScene;                                                       // Reference to the target scene to load when the button is clicked


    // ========= Button Methods =============

    // 1. Method to Load Target Scene on Button Click
    public void OnSceneLoad()
    {
        UI_GameSceneManager.Instance.LoadScene(targetScene.SceneName);                                         // Load the target scene
    }

    // 2. Method to Quit the Game on Button Click
    public void OnQuitGame()
    {
        Application.Quit();                                                                                     // Quit the application
    }

    // 3. Method to Reload the Current Scene on Button Click
    public void OnReloadScene()
    {
        UI_GameSceneManager.Instance.ReloadCurrentScene();                                                      // Reload the current scene
    }
}