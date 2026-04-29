using UnityEngine;

public class Objectstress : DestructableBehaviour
{
    public float stressLimit = 1f; // Maximum stress limit for the object
    public float currentStress = 1f; // Current stress on the object
    public float objStructIntegrity = 1f; 

    [SerializeField] public GameObject parentStruct; // Reference to the parent structure

    void Start()
    {
        currentStress = 0f;
    }

    public void limitCalc(float damage)
    {
        currentStress += damage; // Increase current stress by the damage amount
    
        if (currentStress >= (stressLimit * 0.4f) && currentStress < (stressLimit * 0.6f)) // If the current stress exceeds 40% of the stress limit
        {
            Debug.Log("Object at 60% integrity (damaged)");
        }
        else if (currentStress >= (stressLimit * 0.6f) && currentStress < (stressLimit * 0.8f)) // If the current stress 60% of the stress limit
        {
            Debug.Log("Object 40% integrity (nearly broken)");
        }
        else if (currentStress >= (stressLimit * 0.8f)) // If the current stress exceeds the stress limit
        {
            parentStruct.GetComponent<Structurestress>().structLimitCalc(objStructIntegrity);
            Debug.Log("Object integrity failed (destroyed)");
            Destroy(gameObject); // Destroy the object
        }
    }
    public override DestructionLayer GetLayer => DestructionLayer.ObjectStress;
    public override void ApplyDamage(DestructionHitData hitData)
    {

        float radiusOverDistance = hitData.radius;
        foreach (Collider collider in Physics.OverlapSphere(hitData.hitPoint, radiusOverDistance))
        {
            if (collider.GetComponent<Objectstress>() != null)
            {
                collider.GetComponent<Objectstress>().limitCalc(hitData.damage);
            }
        }
    }

}
