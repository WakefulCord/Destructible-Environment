using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Feedback", menuName = "Scriptable Objects/New Destruction Feedback/Explosion Feedback")]
public class ExplosionFeedback : DestructionFeedback
{
    [Header("---Explosion Feedback---")]
   
    [SerializeField] private float fuseTime;

   

    public float FuseTime => fuseTime;

}
