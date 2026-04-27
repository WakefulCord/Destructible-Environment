using UnityEngine;

/// <summary>
/// Provides functionality for rendering and displaying a weapon preview within the user interface.
/// It uses a dedicated camera to render the weapon prefab to a RenderTexture, which is then converted to a Sprite for display in the UI.
/// This can be used to generate icons for weapons in the player's loadout or inventory, allowing for dynamic previews based on the actual weapon models.
/// </summary>

public class UI_WeaponPreview : MonoBehaviour
{
    // ================= Serialize Fields ==============

    [Header("Settings")]                        
    [SerializeField] private int resolution = 128;                                                                             // The resolution of the generated weapon preview sprite (e.g., 128x128 pixels)
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -3f);                                                    // The position offset of the preview camera relative to the weapon prefab (adjust as needed to get the desired framing)
    [SerializeField] private Vector3 cameraRotation = new Vector3(0f, 0f, 0f);                                                 // The rotation of the preview camera (adjust as needed to get the desired angle on the weapon)
    [SerializeField] private float orthographicSize = 0.8f;                                                                    // The orthographic size of the preview camera (adjust as needed to fit the weapon within the frame)


    // ================= Reference Fields ==============
    private Camera _previewCamera;

    // ================= Unity Methods =================
    private void Awake()
    {
        Setup_PreviewCamera();
    }

    // ================= Custom Methods ================

    // 1. Setup the preview camera with appropriate settings for rendering the weapon preview.
    private void Setup_PreviewCamera()
    {
        // Create a new GameObject to hold the preview camera and set it as a child of this UI element.
        GameObject camObj = new GameObject("UI_PreviewCamera");
        camObj.transform.SetParent(transform);
        camObj.transform.position = new Vector3(0, -1000, 0);

        // Add a Camera component to the new GameObject and configure its settings for rendering the weapon preview.
        _previewCamera = camObj.AddComponent<Camera>();
        _previewCamera.enabled = false; // ← add here
        _previewCamera.backgroundColor = Color.clear;
        _previewCamera.clearFlags = CameraClearFlags.SolidColor;
        _previewCamera.orthographic = true;
        _previewCamera.orthographicSize = 1f;
        _previewCamera.cullingMask = LayerMask.GetMask("UI_WeaponPreview");
    }

    public Sprite Generate_WeaponPreview(GameObject prefab)
    {
        // 1. Spawn the prefab off-screen on our preview layer
        GameObject previewObj = Instantiate(prefab, new Vector3(0, -1000, 0), Quaternion.identity);
        Set_PreviewLayer(previewObj);

        // 2. Position the camera to look at the preview object
        _previewCamera.transform.position = new Vector3(0, -1000, 0) + cameraOffset;
        _previewCamera.transform.rotation = Quaternion.Euler(cameraRotation);
        _previewCamera.orthographicSize = orthographicSize;

        // 3. Render to a RenderTexture
        RenderTexture rt = new RenderTexture(resolution, resolution, 16);
        _previewCamera.targetTexture = rt;
        _previewCamera.Render();

        // 4. Convert to Texture2D
        RenderTexture.active = rt;
        Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        texture.Apply();

        // 5. Clean up
        RenderTexture.active = null;
        _previewCamera.targetTexture = null;
        Destroy(previewObj);
        Destroy(rt);

        // 6. Return as Sprite
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), Vector2.one * 0.5f);
    }

    private void Set_PreviewLayer(GameObject obj)
    {
        int layer = LayerMask.NameToLayer("UI_WeaponPreview");
        obj.layer = layer;
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            child.gameObject.layer = layer;
    }
}