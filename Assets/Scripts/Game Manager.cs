using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    private int _gameTurns; // The number of turns so far.
    private Civilization[] _civilization; // All Civilizations (players and NPC)

    public void StartGame()
    {
        
    }

    public void NewTurn()
    {
        ++_gameTurns;
    }

    public void EndGame()
    {
        
    }
    
}
