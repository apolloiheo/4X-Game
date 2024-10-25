using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class Game 
{
    public World world;
    public List<Civilization> civilizations;
    public int gameTurn;

    /* Constructor */
    public Game()
    {
        gameTurn = 0;
    }
}
