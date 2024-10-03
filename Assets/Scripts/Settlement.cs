using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    private string _name;
    private Civilization _civilization; // Owner
    private List<Tile> _territory;
    private List<Tile> _workedTiles;
    private int[] _yieldsPt; // [Food, Production, Gold, Culture, Science] -> [0,1,2,3,4]
    private int _population;
    private int _foodSurplus;
    private int _combatStrength;
    private List<Building> _buildings;
    private List<CityProject> _projects;
    private CityProject _currentCityProject;
    private Tile _tile;

    /* New Settlement Constructor - for Gameplay */ 
    public Settlement(string name, Civilization civilization, Tile tile)
    {
        _name = name;
        _tile = tile;
        _civilization = civilization;
        _territory = StartingTerritory(tile);
        _workedTiles = new List<Tile>();
        _workedTiles.Add(tile);
        CalculateYields();

    }

    /* Adds all adjacent Tiles to territory */
    private List<Tile> StartingTerritory(Tile tile)
    {
        List<Tile> territory = new List<Tile>();
        territory.Add(tile);

        foreach (Tile t in tile.GetNeighbors())
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
            foreach (Tile t in _workedTiles)
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
