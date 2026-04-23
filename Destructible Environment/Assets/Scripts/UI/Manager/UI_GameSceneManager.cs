using UnityEngine;
using UnityEngine.SceneManagement;



// ========= Scene Reference Helper =========
[System.Serializable]
public class SceneReference
{
    [SerializeField] private UnityEngine.Object sceneAsset;                                                     // Drag and drop scene asset here in Inspector

    public string SceneName => sceneAsset != null ? sceneAsset.name : string.Empty;                             // Get scene name directly from asset at runtime
}
// ===========================================

public class UI_GameSceneManager : MonoBehaviour
{
    // ========= Getters & Setters =========
    private static UI_GameSceneManager sceneInstance;

    public static UI_GameSceneManager Instance
    {
        get
        {
            if (sceneInstance == null)
            {
                GameObject obj = new GameObject("UI_GameSceneManager");                                         // Create a new GameObject if no instance exists
                sceneInstance = obj.AddComponent<UI_GameSceneManager>();                                        // Add the component to the new GameObject
                DontDestroyOnLoad(obj);                                                                         // Persist across scenes
            }
            return sceneInstance;
        }
    }

    // ========= Unity Methods =============
    private void Awake()
    {
        if (sceneInstance != null && sceneInstance != this)
        {
            Destroy(this.gameObject);                                                                           // Ensure only one instance of UI_GameSceneManager exists
        }
        else
        {
            sceneInstance = this;                                                                               // Set the singleton instance
            DontDestroyOnLoad(this.gameObject);                                                                 // Persist across scenes if needed
        }
    }

    // ========= Custom Methods =============
    // 1. Method To Load Scene By Name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);                                                                      // Load New Scene by name
    }

    // 2. Method to Load Additional Scene Additively
    public void LoadSceneAdditive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);                                              // Load scene additively without unloading the current scene
    }

    // 3. Method to Unload Scene by Name
    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);                                                               // Unload Scene by name
    }

    // 4. Method to Reload Current Scene
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();                                                     // Get the currently active scene
        SceneManager.LoadScene(currentScene.name);                                                              // Reload the current scene by name
    }
}