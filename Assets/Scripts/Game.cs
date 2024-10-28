using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class Game 
{
    [JsonProperty]
    public World world;
    [JsonProperty]
    public List<Civilization> civilizations;
    [JsonProperty]
    public int gameTurn;

    /* Constructor */
    public Game()
    {
        gameTurn = 0;
    }
}
