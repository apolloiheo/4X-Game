using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    private string _name;
    private Civilization _civilization; // Owner
    private List<GameTile> _territory;
    private List<GameTile> _workedTiles;
    private int[] _yieldsPt; // [Food, Production, Gold, Culture, Science] -> [0,1,2,3,4]
    private int _population;
    private int _foodSurplus;
    private int _combatStrength;
    private List<Building> _buildings;
    private List<CityProject> _projects;
    private CityProject _currentCityProject;
    private GameTile _gameTile;

    /* New Settlement Constructor - for Gameplay */ 
    public Settlement(string name, Civilization civilization, GameTile gameTile)
    {
        _name = name;
        _gameTile = gameTile;
        _civilization = civilization;
        _territory = StartingTerritory(gameTile);
        _workedTiles = new List<GameTile>();
        _workedTiles.Add(gameTile);
        CalculateYields();

    }

    /* Adds all adjacent Tiles to territory */
    private List<GameTile> StartingTerritory(GameTile gameTile)
    {
        List<GameTile> territory = new List<GameTile>();
        territory.Add(gameTile);

        foreach (GameTile t in gameTile.GetNeighbors())
        {
            territory.Add(t);
        }
        return territory;
    }

    private void CalculateYields()
    {
        // Reset Yields
        _yieldsPt = new int[5];
        
        // Tiles
        for (int yield = 0; yield < _yieldsPt.Length; yield++)
        {
            foreach (GameTile t in _workedTiles)
            {
                _yieldsPt[yield] += t.GetYields()[yield];
            }
        }
        
        // Buildings
        for (int yield = 0; yield < _yieldsPt.Length; yield++)
        {
            foreach (Building b in _buildings)
            {
                _yieldsPt[yield] += b.GetYields()[yield];
            }
        }
    }

    private void CalculateFoodSurplus()
    {
        
    }
}
