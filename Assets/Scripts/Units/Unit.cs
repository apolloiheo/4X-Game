using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class Unit : ISerialization
{
    // Instance Properties
    [JsonProperty]
    public string _name; // The Unit's name.
    [JsonProperty]
    public int _health; // A Unit's current Health Points (Default of 100)
    [JsonProperty]
    public int _baseMP; // A Unit's current Movement Points per turn.
    [JsonProperty]
    public int _currMP; // A Unit's remaining movement points this turn.
    [JsonProperty]
    public int _combatStrength; // A Unit's base Combat Strength Stat - used to determine Attack Damage and Defense Damage.
    [JsonProperty]
    public int _supplies; // A Unit's current Supplies stat - Determines how many turns it can stay out of your territory before taking damage.
    [JsonProperty]
    public int _attackRange; // The range of Tiles a Unit can attack from. (Melee: 0, Ranged: 1 - X).
    [JsonProperty]
    public int _experience; // A Unit's current XP. Needs X amount for a Promotion.
    [JsonProperty]
    public bool _hasOrder; // Determines whether a Unit has already been given an order for this turn.
    [JsonProperty]
    public bool _exhausted; // Determines if a Unit still has moves to make this turn.
    [JsonProperty]
    public bool _fortified; // Determines if a Unit was ordered to Fortify this turn.
    [JsonProperty]
    public List<Promotion> _promotions; // Unlocked Promotions
    [JsonProperty]
    public Point _position;
    
    // Private Variables
    private List<Point> possible_moves;
    
    // References
    public GameTile _gameTile; //The Tile this Unit is on. 
    public Civilization _civilization; // The Civilization that owns this Unit.

    // Constants
    private const int Zero = 0;
    private const int TotalPromotions = 5;
    private const int UnitMaxHealth = 100;
    private const int HillAttackDebuf = 10;
    private const int RiverAttackDebuf = 10;

    public Unit( GameTile tile, Civilization civilization)
    {
        _gameTile = tile;
        _civilization = civilization;
    }

    public void OnTurnEnd()
    {
        if (_fortified)
        {
            _health += 10;
        }
        
        _exhausted = false;
        _hasOrder = false;
        _currMP = _baseMP;
    }

    public void UpdateUnit()
    {
        if (_currMP <= 0)
        {
            _exhausted = true;
            _hasOrder = true;
        }
    }
    
    /* Move a Unit across Tiles */
    public void Move(GameTile target)
    {
        // To be implemented
        Debug.Log("Started at" + _gameTile.GetXPos() +  _gameTile.GetYPos());
        foreach (var node in Pathfinder.UnitAstar(_gameTile, target))
        {
            _gameTile.SetUnit(null);
            _gameTile = node.Item1;
        }
        
        Debug.Log("Ended at" + _gameTile.GetXPos() +  _gameTile.GetYPos());
        Debug.Log("Was trying to arrive at" + target.GetXPos() +  target.GetYPos());
    }

    /* Attack a Settlement */
    public void Attack(Settlement target)
    {
        // To be implemented
    }

    public void Damage(int damage)
    {
        _health -= damage;
    }

    public List<Point> GetPossibleMoves(GameTile currTile, int movementPoints, bool isInitial)
    {
        if (isInitial)
        {
            possible_moves = new List<Point>();
        }
        
        if (movementPoints == 0)
        {
            possible_moves.Add(new Point(currTile.GetXPos(), currTile.GetYPos()));
        }
        
        if (movementPoints < 0)
        {
            return null;
        }

        foreach (GameTile neighbor in currTile.GetNeighbors())
        {
            if (neighbor.GetMovementCost() <= movementPoints && neighbor.IsWalkable())
            {
                List<Point> possibleMoves = GetPossibleMoves(neighbor, movementPoints - neighbor.GetMovementCost(), false);
                
                possible_moves.AddRange(possibleMoves);
            }
        }
        
        // String Building
        /*string pointsString = "";
        foreach (Point point in possible_moves)
        {
            pointsString += point.ToString() + "\n";
        }

        Debug.Log(pointsString);*/
        
        return possible_moves;
    }
    
    public List<GameTile> PossibleMoves()
    {
        List<GameTile> possibleMoves = new List<GameTile>();
        Queue<(GameTile, int)> queue = new Queue<(GameTile, int)>();
        HashSet<GameTile> visited = new HashSet<GameTile>();
        
        // Start with current tile
        queue.Enqueue((_gameTile, _currMP));
        visited.Add(_gameTile);

        while (queue.Count > 0)
        {
            var (tile, remainingPoints) = queue.Dequeue();

            foreach (GameTile neighbor in _gameTile.GetNeighbors())
            {
                // Skip if already vistied
                if (visited.Contains(neighbor))
                {
                    continue;
                }
                
                // Calculate remaining movement points after moving to this neighbor
                int movementCost = neighbor.GetMovementCost();
                int newRemainingPoints = remainingPoints - movementCost;
                
                // If the unit can move to this tile
                if (newRemainingPoints >= 0)
                {
                    possibleMoves.Add(neighbor);
                    queue.Enqueue((neighbor, newRemainingPoints));
                    visited.Add(neighbor);
                }
            }
        }
        
        return possibleMoves;
    }
    
    public void StageForSerialization()
    {
        StageCurrentTile();

        // Turn the current Tile into a Point (location)
        void StageCurrentTile()
        {
            _position = new Point(_gameTile.GetXPos(), _gameTile.GetYPos());
            _gameTile = null;
        }
        
        // Set its owner to null (this will be restored by the Civilization)
        _civilization = null;
    }

    public void RestoreAfterDeserialization(Game game)
    {
        RestoreCurrentTile();

        // Restore this Unit, and it's Tile's references to each other.
        void RestoreCurrentTile()
        {
            _gameTile = game.world.GetTile(_position);
            _gameTile.SetUnit(this);
        }
    }
}
