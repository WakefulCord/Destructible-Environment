using UnityEngine;

public class Objectstress : MonoBehaviour
{
    public float stressLimit = 100f; // Maximum stress limit for the object
    public float currentStress = 1f; // Current stress on the object
    public float objStructIntegrity = 5f; //Integrity relating to value in structure

    [SerializeField] public GameObject parentStruct; // Reference to the parent structure

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example input to simulate damage, replace with actual damage trigger
        {
            limitCalc(10f); // Simulate taking 10 damage
        }
    }

    public void limitCalc(float damage)
    {
        currentStress = currentStress + damage; // Increase current stress by the damage amount
        if (currentStress >= (stressLimit * 0.4f) && currentStress <(stressLimit * 0.6f)) // If the current stress exceeds 40% of the stress limit
        {
            Debug.Log("Object at 60% integrity (damaged)");
            //destuct code here
        }
        else if (currentStress >= (stressLimit * 0.6f) && currentStress < (stressLimit * 0.8f)) // If the current stress 60% of the stress limit
        {
            Debug.Log("Object 40% integrity (nearly broken)");
            //destuct code here
        }
        else if (currentStress >= (stressLimit * 0.8f)) // If the current stress exceeds the stress limit
        {
            updateStructIntegrity(); // OnExplosiveUpdate the structure integrity before destroying the object
            Debug.Log("Object integrity failed (destroyed)");
            Destroy(gameObject); // Destroy the object
        }
    }

    private void updateStructIntegrity()
    {
        parentStruct.GetComponent<Structurestress>().structLimitCalc(objStructIntegrity); // Call the parent structure's structLimitCalc function to update its integrity
        Debug.Log("Object integrity value subtracted from structure integrity: " + objStructIntegrity);
    }
}
