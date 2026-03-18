using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] GameObject brokenWall;
    [SerializeField] GameObject wall;

    public void breakWall()
    {
        brokenWall.SetActive(true);
        wall.SetActive(false);
    }
}
