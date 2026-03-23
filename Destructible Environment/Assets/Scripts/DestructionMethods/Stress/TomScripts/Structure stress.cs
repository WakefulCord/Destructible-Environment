using System;
using UnityEngine;

public class Structurestress : MonoBehaviour
{
    public float stressLimit = 0f; // Maximum stress limit for the structure
    public float currentStress = 1f; // Current stress on the structure

    // Start is called once before the first execution of OnExplosiveUpdate after the MonoBehaviour is created
    void Start()
    {
        stressLimit = CalculateStructIntegrity(); // Calculate the initial structure integrity at the start
        Debug.Log("Initial Structure Integrity: " + stressLimit); // Log the initial structure integrity
    }

    public void structLimitCalc(float damage)
    {
        currentStress = currentStress + damage; // Reduce the stress limit by the damage amount
        Debug.Log("Current Structure Stress: " + currentStress); // Log the current structure stress
        if (currentStress >= (stressLimit * 0.4f) && currentStress < (stressLimit * 0.6f)) // If the current stress exceeds 40% of the stress limit
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
            Debug.Log("Object integrity failed (destroyed)");
            //call parent function to destuct structure here
            Destroy(gameObject); // Destroy the object
        }

    }

    float CalculateStructIntegrity()
    {
        float structIntegrity = 0f; // Calculate the structure integrity based on the current stress and stress limit
        Objectstress[] objectIntegrities = GetComponentsInChildren<Objectstress>(); // Get all ObjectIntegrity components in the children of the structure
        foreach (var child in objectIntegrities)
        {
            structIntegrity += child.objStructIntegrity; // Add the integrity of each object to the total structure integrity
        }
        return structIntegrity; // Return the total structure integrity
    }
}
