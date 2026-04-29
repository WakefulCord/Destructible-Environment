using UnityEngine;

public class EscapeToggle : MonoBehaviour
{
    public GameObject target;
    bool paused;

    void Start()
    {
        target.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            target.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
        }
    }
}
