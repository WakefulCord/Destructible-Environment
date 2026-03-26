using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Feedback", menuName = "Scriptable Objects/New Destruction Feedback/Bullet Feedback")]
public class BulletFeedback : DestructionFeedback
{
    [Header("---Bullet Feedback---")]
    [Header("Decal Settings")]
    [SerializeField] private bool useDecal;
    [SerializeField] private GameObject bulletDecalPrefab;
    [SerializeField] private float decalFadeTimer = 2f;

   

    //public
    public bool UseDecal => useDecal;
    public GameObject GetDecal => bulletDecalPrefab;
    public float GetDecalFadeTimer => decalFadeTimer;
    
}
