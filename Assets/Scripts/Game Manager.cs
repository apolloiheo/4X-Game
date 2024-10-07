using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    // Property to handle the GameManager instance
    public static GameManager Instance { get; private set; }

    public event EventHandler OnTurnEnd;

    private int _gameTurns; // The number of turns so far.
    private Civilization[] _civilization; // All Civilizations (players and NPC)

    private void Awake()
    {
        // Set GameManager instance to this object
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager instance");
        }
        Instance = this;
    }

    public void StartGame()
    {
        
    }

    public void NewTurn()
    {
        ++_gameTurns;

        // End the turn (invokes all functions associated with this event)
        OnTurnEnd?.Invoke(this, EventArgs.Empty);

    }

    public void EndGame()
    {

    }
    
}
