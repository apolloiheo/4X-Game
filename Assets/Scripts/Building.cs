using Newtonsoft.Json;

[System.Serializable]
public class Building 
{
    [JsonProperty] public string name;
    [JsonProperty] public int[] yields;
    
    public Building(string nameString, int[] yieldArray)
    {
        name = nameString;
        yields = yieldArray;
    }
}
