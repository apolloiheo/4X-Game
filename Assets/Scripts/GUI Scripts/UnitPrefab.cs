using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitPrefab : MonoBehaviour
{
    public Unit Unit;
    public Director director;
    
    public void UpdatePrefab()
    {
        // Update its own values and visuals on the world    
        
        // To be implemented
    }

    public void OnClick()
    {
        Debug.Log("Clicked button!");
        director.selectedUnit = Unit;
        director.OpenUnitWindow();
    }
}
