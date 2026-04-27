using UnityEngine;

public class ExplosionTool : DestructionTool // shared base for explosion tools, holds data for the actual explosive 
{
    [Header("---Explosion Tool---")]
    [Tooltip("Explosive with Explosive Behaviour, usually triggered by launcher/detonater.Found in prefabs")][SerializeField] private GameObject explosivePrefab; // expolsive placed/launched
 

    public GameObject ExplosivePrefab => explosivePrefab;

    //destruction feedback ref
    public ExplosionFeedback GetExplosionFeedback => (ExplosionFeedback)GetDestructionFeedback;
}
