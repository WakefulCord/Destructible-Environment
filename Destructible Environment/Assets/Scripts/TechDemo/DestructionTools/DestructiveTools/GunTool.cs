using UnityEngine;

[CreateAssetMenu(fileName = "Gun Tool", menuName = "Scriptable Objects/Tools/Gun Tool")]
public class GunTool : DestructionTool
{
    //destruction feedback ref
    public BulletFeedback GetBulletFeedback => (BulletFeedback)GetDestructionFeedback;

    [Header("---Gun Tool----")]
    [Header("Tracer Settings")]
    [SerializeField] private bool useTracer;
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private float tracerSpeed = 200f;

    [Header("Spread Settings")]
    [Range(1, 10)][SerializeField] private int bulletCount;
    [Range(0, 15)][SerializeField] private float spreadAngle;

    [Header("Aim Settings")] // seeconady  fire
    [SerializeField] private bool canAim;
    
    [SerializeField] private int aimFOV;
    [SerializeField] private float aimSpeed;

    [SerializeField] private GameObject aimOverlay;
    //public

    public GameObject GetTracerPrefab => tracerPrefab;
    public float GetTracerSpeed => tracerSpeed;
    public bool UseTracer => useTracer;
    public int BulletCount => bulletCount;
    public float SpreadAngle => spreadAngle;

    public bool CanAim => canAim;
    public int AimFOV => aimFOV;
    public float AimSpeed => aimSpeed;
    public GameObject AimOverlay => aimOverlay;
}
