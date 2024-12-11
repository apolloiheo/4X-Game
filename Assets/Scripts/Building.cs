using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Building 
{
    [JsonProperty] public string name;
    [JsonProperty] public int[] yields;
    
    public Building(string nameString, int[] yieldArray)
    {
        this.name = nameString;
        yields = yieldArray;
    }
}
