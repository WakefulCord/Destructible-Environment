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

    //public

    public GameObject GetTracerPrefab => tracerPrefab;
    public float GetTracerSpeed => tracerSpeed;
    public bool UseTracer => useTracer;

}
