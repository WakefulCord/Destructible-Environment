using System;
using UnityEngine;

public class Structurestress : MonoBehaviour
{
    public float stressLimit = 0f;
    public float currentStress = 0f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*// Get all Objectstress components in children (including inactive if needed)
        Objectstress[] childStresses = GetComponentsInChildren<Objectstress>();
        float totalIntegrity = 0f;

        foreach (var objStress in childStresses)
        {
            totalIntegrity += objStress.objStructIntegrity;
        }
*/
        stressLimit = GetComponentsInChildren<BreakableWall>().Length;
        Debug.Log("Total Structure Stress Limit: " + stressLimit);
    }


    public void structLimitCalc(float damage)
    {
        currentStress += damage; 
        Debug.Log("Current Structure Stress: " + currentStress); // Log the current structure stress
        if (currentStress >= (stressLimit * 0.4f) && currentStress < (stressLimit * 0.6f)) // If the current stress exceeds 40% of the stress limit
        {
            Debug.Log("Object at 60% integrity (damaged)");
        }
        else if (currentStress >= (stressLimit * 0.6f) && currentStress < (stressLimit * 0.8f)) // If the current stress 60% of the stress limit
        {
            Debug.Log("Object 40% integrity (nearly broken)");
        }
        else if (currentStress >= (stressLimit * 0.74f)) // If the current stress exceeds the stress limit
        {
            Debug.Log("Object integrity failed (destroyed)");
            Destroy(gameObject);
        }

    }
}
