using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CityProject
{
    [JsonProperty]
    public int projectName;
    [JsonProperty]
    public int cost;
    [JsonProperty]
    public int currentProductionProgress;

    public CityProject(int name, int cost)
    {
        projectName = name;
        this.cost = cost;
        currentProductionProgress = 0;
    }
    
    public void AddToProgress(int production)
    {
        currentProductionProgress += production;
        
        if (currentProductionProgress >= cost)
        {
            Complete();
        }
    }

    public void Complete()
    {
        
    }

    public int GetName()
    {
        return projectName;
    }

}
