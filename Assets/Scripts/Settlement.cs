using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    
    // Instance Variables
    private string _name; // The name the player set for the Settlement.
    private Civilization _civilization; // Owner
    private List<GameTile> _territory; // Tiles a Settlement controls.
    private List<GameTile> _workedTiles; // Tiles in Territory that are being worked by a Population
    private int[] _yieldsPt; // [Food, Production, Gold, Culture, Science] -> [0,1,2,3,4] YieldsPT -> Yields Per Turn
    private int _population; // The size of Settlement and the units you can assign to Tiles
    private int _foodSurplus; // The buildup of extra food needed to grow a Settlement.
    private int _combatStrength;
    private List<Building> _buildings;
    private List<CityProject> _projects;
    private CityProject _currentCityProject;
    private GameTile _gameTile;
    
    // Constants
    private const int FoodSurplusRequirement = 15;
    private const int Food = 0;
    private const int Production = 1;
    private const int Gold = 2;
    private const int Culture = 3;
    private const int Science = 4;

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

    /* Calculate a Settlement's yields per turn by summing  */
    public void CalculateYields()
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


    /* Logic for when the turn ends */
    public void TurnEnded()
    {
        // Update Settlement Yields
        CalculateYields();
        
        // Add current Production per turn to the City Project's progress
        _currentCityProject.AddToProgress(_yieldsPt[Production]);
        
        // Updated Growth
        _foodSurplus += _yieldsPt[Food] - (_population * 2);


    }

    private void CalculateFoodSurplus()
    {
        
    }
    
    // Getter Methods
    public int[] GetYieldsPt()
    {
        return _yieldsPt;
    }
    
    
}
