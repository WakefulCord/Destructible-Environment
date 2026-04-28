using UnityEngine;

public class DestructableBehaviour : MonoBehaviour, IDestructable
{
    public virtual DestructionLayer GetLayer => throw new System.NotImplementedException();

    public virtual void InitializeDestruction()
    {
       
    }

    public virtual void UpdateDestruction()
    {

    }
    public virtual void ApplyDamage(DestructionHitData hitData)
    {
        
    }


    
}
