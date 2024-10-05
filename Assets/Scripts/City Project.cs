using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityProject : MonoBehaviour
{
    private int _cost;
    private int _currentProductionProgress;


    public void AddToProgress(int production)
    {
        _currentProductionProgress += production;
        
        if (_currentProductionProgress >= _cost)
        {
            Complete();
        }
    }

    public void Complete()
    {
        
    }
}
