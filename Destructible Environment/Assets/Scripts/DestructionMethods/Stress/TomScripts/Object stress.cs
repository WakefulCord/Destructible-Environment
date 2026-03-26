using UnityEngine;

public class Objectstress : MonoBehaviour
{
    public float stressLimit = 1f; // Maximum stress limit for the object
    public float currentStress = 1f; // Current stress on the object
    public float objStructIntegrity = 1f; //Integrity relating to value in structure

    [SerializeField] public GameObject parentStruct; // Reference to the parent structure

    void Start()
    {
        stressLimit = GetComponentsInChildren<WallVoxel>().Length;
        Debug.Log("Initial Object Stress Limit: " + stressLimit); // Log the initial object stress limit
        currentStress = 0f;
    }

    public void limitCalc(float damage)
    {
        currentStress += damage; // Increase current stress by the damage amount
        if (currentStress >= (stressLimit * 0.4f) && currentStress <(stressLimit * 0.6f)) // If the current stress exceeds 40% of the stress limit
        {
            Debug.Log("Object at 60% integrity (damaged)");
        }
        else if (currentStress >= (stressLimit * 0.6f) && currentStress < (stressLimit * 0.8f)) // If the current stress 60% of the stress limit
        {
            Debug.Log("Object 40% integrity (nearly broken)");
        }
        else if (currentStress >= (stressLimit * 0.74f)) // If the current stress exceeds the stress limit
        {
<<<<<<< HEAD
            parentStruct.GetComponent<Structurestress>().structLimitCalc(objStructIntegrity); 
=======
            updateStructIntegrity(); // OnExplosiveUpdate the structure integrity before destroying the object
>>>>>>> Player
            Debug.Log("Object integrity failed (destroyed)");
            Destroy(gameObject); // Destroy the object
        }
    }

}
