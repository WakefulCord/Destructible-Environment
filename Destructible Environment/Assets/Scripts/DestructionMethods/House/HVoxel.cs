using UnityEngine;

public class HVoxel : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private int z;

    public int X => x;
    public int Y => y;
    public int Z => z;

    public void Init(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
