using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject optionsUI;
    public GameObject controlsUI;

    bool paused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsUI.activeSelf || controlsUI.activeSelf)
            {
                OpenPauseMenu();
                return;
            }

            paused = !paused;

            if (paused)
                OpenPauseMenu();
            else
                CloseAllMenus();
        }
    }

    public void OpenPauseMenu()
    {
        pauseUI.SetActive(true);
        optionsUI.SetActive(false);
        controlsUI.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        paused = true;
    }

    public void OpenOptionsMenu()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(true);
        controlsUI.SetActive(false);
    }

    public void OpenControlsMenu()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(false);
        controlsUI.SetActive(true);
    }

    public void ResumeButton()
    {
        CloseAllMenus();
    }

    void CloseAllMenus()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(false);
        controlsUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        paused = false;
    }
}
