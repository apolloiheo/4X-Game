using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Building : MonoBehaviour
{
    [JsonProperty]
    private int[] _yields;
    
    public int[] GetYields()
    {
        return _yields;
    }
}
