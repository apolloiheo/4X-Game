using System;
using System.Collections.Generic;
using City_Projects;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Settlement : ISerialization
{
    // Serializable Instance Variables
    [JsonProperty]
    private string _name; // The name the player set for the Settlement.
    [JsonProperty]
    private int[] _yieldsPt; // [Food, Production, Gold, Culture, Science] -> [0,1,2,3,4] YieldsPT -> Yields Per Turn
    [JsonProperty]
    private int _population; // The size of Settlement and the units you can assign to Tiles
    [JsonProperty]
    private int _foodSurplus; // The buildup of extra food needed to grow a Settlement.
    [JsonProperty]
    private int _combatStrength;
    [JsonProperty]
    private List<Building> _buildings;
    [JsonProperty]
    private List<CityProject> _projects;
    [JsonProperty]
    private CityProject _currentCityProject;
    [JsonProperty]
    private int _tier; // Settlement tier. 1 = Village, 2 = Town, 3 = City
    [JsonProperty]
    public Point[] _territoryPoints;
    [JsonProperty]
    public Point[] _workedTilesPoints;
    [JsonProperty] public Point[] _lockedTilesPoints;
    [JsonProperty] public Point _settlementPoint;
    
    // Circular Instance References
    public Civilization _civilization; // Owner
    public GameTile _gameTile;
    public List<GameTile> _territory; // Tiles a Settlement controls.
    public List<GameTile> _workedTiles; // Tiles in Territory that are being worked by a Population
    public List<GameTile> _lockedTiles;
    
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
        _yieldsPt = new int[5];
        _population = 1;
        _foodSurplus = 0;
        _combatStrength = 10;
        _buildings = new List<Building>();
        _projects = GetBaseProjects();
        _currentCityProject = null;
        _gameTile = gameTile;
        _civilization = civilization;
        _territory = StartingTerritory(gameTile);
        _workedTiles = new List<GameTile>();
        _workedTiles.Add(gameTile);
        _lockedTiles = new List<GameTile>();
        _lockedTiles.Add(gameTile);
        AutoAssignWorkedTiles();
        UpdateYields();
        _tier = 1;
        
        /* Adds all adjacent Tiles to territory */
        List<GameTile> StartingTerritory(GameTile gameTile)
        {
            List<GameTile> territory = new List<GameTile>();
            territory.Add(gameTile);

            foreach (GameTile t in gameTile.GetNeighbors())
            {
                territory.Add(t);
            }
            return territory;
        }
        
    }
    
    // End the turn
    public void OnTurnEnd()
    {
        // Update Settlement Production
        ProgressCityProject();
        
        // Update Settlement Growth
        ProgressFoodSurplus();
        
        // Update Settlement Yields
        UpdateYields();

        // Update Settlement Tier
        UpdateTier();
        
        void ProgressCityProject()
        {
            // Add current Production per turn to the City Project's progress
            _currentCityProject.AddToProgress(_yieldsPt[Production]);
        }

        void ProgressFoodSurplus()
        {
            // Updated Growth
            _foodSurplus += _yieldsPt[Food] - (_population * 2);
            
            // Check if Settlement will grow
            if (_foodSurplus >= FoodSurplusRequirement)
            {
                // If so, 50% of remaining food surplus is carried over.
                _foodSurplus = (_foodSurplus - FoodSurplusRequirement) / 2 ;
            
                // Increase population
                _population += 1;
            }
        }

        void UpdateCivilizationYields()
        {
            
        }
    }

    /* Called frequently to make sure Settlement updates yields in GUI */
    public void UpdateYields()
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

    /* Calculate a settlement's tier */
    private void UpdateTier()
    {
        if (_population <= 3)
        {
            _tier = 0;
        }
        else if (_population <= 7)
        {
            _tier = 1;
        }
        else
        {
            _tier = 2;
        }
    }

    /* Add a city project to settlement */
    public void AddProject(string name, int cost)
    {
        
    }

    /* Switch the project the settlement is working on */
    public void SwitchProject(int index)
    {
        _currentCityProject = _projects[index];
    }

    /* Gives every Settlement the base projects. */
    private List<CityProject> GetBaseProjects()
    {
        List<CityProject> baseProjects = new List<CityProject>();
        
        baseProjects.Add(new SettlerProject());
        baseProjects.Add(new ScoutProject());
        baseProjects.Add(new WarriorProject());
        baseProjects.Add(new MonumentProject());
        
        return baseProjects;
    }

    /* Auto Assigns Worked Tiles that haven't been locked in by player */
    public void AutoAssignWorkedTiles()
    {
        // If there are exactly the same amount of locked tiles as there can possibly be worked
        if (_lockedTiles.Count == _population + 1)
        {
            // Assign them, and return.
            _workedTiles = _lockedTiles;
            Debug.Log("All tiles already assigned. Returning.");
            return;
        }
        
        // Sort tiles in territory according to best yield value
        List<GameTile> bestTilesInTerritory = _territory;
        // Best file in the front
        bestTilesInTerritory.Sort((tile1,tile2) => tile2.TileValue().CompareTo(tile1.TileValue()));
        
        // If there are more locked tiles than the Settlement can work, add the best Locked Tiles to Worked Tiles
        if (_lockedTiles.Count > _population + 1)
        {
            // Sort locked tiles by highest value
            _lockedTiles.Sort((tile1,tile2) => tile2.TileValue().CompareTo(tile1.TileValue()));

            foreach (GameTile t in _lockedTiles)
            {
                // If we are at max capacity
                if (_workedTiles.Count >= _population + 1)
                {
                    Debug.Log("Assigned all locked tiles until full.");
                    return;
                }
                
                // If this tile isn't already being worked
                if (!_workedTiles.Contains(t))
                {
                    // Add it
                    _workedTiles.Add(t);
                }
            } 
        }
        
        // If there are less locked tiles than possible number of worked tiles, Assign them to worked Tiles.
        if (_lockedTiles.Count < _population + 1)
        {
            _workedTiles = _lockedTiles;
        }
        
        // Fill any remaining tiles (population) with the best valued tiles
        foreach (GameTile territoryTile in bestTilesInTerritory)
        {
            // If we are at max capacity
            if (_population + 1 <= _workedTiles.Count)
            {
                Debug.Log("Auto Assigned tiles until full.");
                return;
            }
            
            if (!_workedTiles.Contains(territoryTile))
            {
                _workedTiles.Add(territoryTile);
            }
        }
    }
    
    // Getter Methods
    public int[] GetYieldsPt()
    {
        return _yieldsPt;
    }

    public string GetName()
    {
        return _name;
    }

    public Civilization GetCivilization()
    {
        return _civilization;
    }

    public int GetPopulation()
    {
        return _population;
    }

    public List<GameTile> GetTerritory()
    {
        return _territory;
    }

    public List<GameTile> GetWorkedTiles()
    {
        return _workedTiles;
    }
    
    public int GetFoodSurplus()
    {
        return _foodSurplus;
    }
    
    public int GetCombatStrength()
    {
        return _combatStrength;
    }
    
    public List<Building> GetBuildings()
    {
        return _buildings;
    }
    
    public List<CityProject> GetProjects()
    {
        return _projects;
    }
    
    public CityProject GetCurrentCityProject()
    {
        return _currentCityProject;
    }
    
    public GameTile GetTile()
    {
        return _gameTile;
    }
    
    public int GetTier()
    {
        return _tier;
    }

    public void StageForSerialization()
    {
        // Civilization will restore this for each settlement.
        _civilization = null;
        
        StageSettlementTile();
        StageTerritoryTiles();
        StageWorkedTiles();
        StageLockedTiles();
        
        // Store the Settlement's Tile
        void StageSettlementTile()
        {
            _settlementPoint = new Point(_gameTile.GetXPos(), _gameTile.GetYPos());
            _gameTile = null;
        }
        // Transfer all _territory Tiles into an array of Points for serialization
        void StageTerritoryTiles()
        {
            if (_territory is not null)
            {
                _territoryPoints = new Point[_territory.Count];
                int index = 0;
                foreach (GameTile t in _territory)
                {
                    _territoryPoints[index] = new Point(t.GetXPos(), t.GetYPos());
                    index++;
                }
                _territory = null;
            }    
        }
        // Transfer _workedTiles List into an array of Points for serialization
        void StageWorkedTiles()
        {
            if (_workedTiles is not null)
            {
                _workedTilesPoints = new Point[_workedTiles.Count];
                int index = 0;
                foreach (GameTile t in _workedTiles)
                {
                    _workedTilesPoints[index] = new Point(t.GetXPos(), t.GetYPos());
                    index++;
                }
                _workedTiles = null;
            }
        }
        // Transfer all _lockedTiles List into an array of Points for serialization
        void StageLockedTiles()
        {
            _lockedTilesPoints = new Point[_lockedTiles.Count];
            int index = 0;
            foreach (GameTile t in _lockedTiles)
            {
                _lockedTilesPoints[index] = new Point(t.GetXPos(), t.GetYPos());
                index++;
            }
            _lockedTiles = null;
        }
        
    }

    public void RestoreAfterDeserialization(Game game)
    {
        RestoreSettlementTile();
        RestoreTerritory();
        RestoreWorkedTiles();
        RestoreLockedTiles();

        // Restore GameTile Reference to Settlement (Settlement Location)
        void RestoreSettlementTile()
        {
            _gameTile = game.world.GetTile(_settlementPoint);
            _gameTile.SetSettlement(this);
        }
        // Restore Territory Tile References to Settlement
        void RestoreTerritory()
        {
            // Reinitialize
            _territory = new List<GameTile>();
            
            foreach (Point point in _territoryPoints)
            {
                _territory.Add(game.world.GetTile(point));
            }
        }
        // Restore Worked Tile References to Settlement
        void RestoreWorkedTiles()
        {
            // Reinitialize
            _workedTiles = new List<GameTile>();
            
            foreach (Point point in _workedTilesPoints)
            {
                _workedTiles.Add(game.world.GetTile(point));
            }
        }
        // Restore Locked Tiles references to Settlement
        void RestoreLockedTiles()
        {
            // Reinitialize
            _lockedTiles = new List<GameTile>();

            foreach (Point point in _lockedTilesPoints)
            {
                _lockedTiles.Add(game.world.GetTile(point));
            }
        }
    }

    public int GetProduction()
    {
        return _yieldsPt[1];
    }

    public int GetFood()
    {
        return _yieldsPt[0];
    }

    // Returns a string (integer) to display the turns left to grow.
    public string TurnsToGrow()
    {
        //int food_surplus_per_turn = _yieldsPt[0] - (_population * 2);
        int food_surplus_per_turn = _yieldsPt[0];

        if (food_surplus_per_turn <= 0)
        {
            return "-";
        }

        return Math.Ceiling((double)(15 - _foodSurplus / food_surplus_per_turn)).ToString();
    }
    
    // Returns a string (integer) to display the turns left to produce.
    public string TurnsToProduce()
    {
        if (_currentCityProject is not null)
        {
            return Math.Ceiling((decimal)((_currentCityProject.projectCost - _currentCityProject.currentProductionProgress) / _yieldsPt[1])).ToString();
        }

        return "-";
    }

    public void SetCityProject(CityProject project)
    {
        _currentCityProject = project;   
    }
    
}
    
    
