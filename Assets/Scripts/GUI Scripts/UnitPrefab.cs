using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitPrefab : MonoBehaviour
{
    public Unit unit;
    public Director director;
    
    public void UpdatePrefab()
    {
        // Update its own values and visuals on the world    
        
        // To be implemented
    }

    public void OnClick()
    {
        director.SelectUnit(unit);
    }
}
