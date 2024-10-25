using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    // Instance Properties
    private string _name; // The Unit's name.
    private int _health; // A Unit's current Health Points (Default of 100)
    private int _movementPoints; // A Unit's current Movement Points per turn.
    private int _currentMovementPoints; // A Unit's remaining movement points this turn.
    private int _combatStrength; // A Unit's base Combat Strength Stat - used to determine Attack Damage and Defense Damage.
    private int _supplies; // A Unit's current Supplies stat - Determines how many turns it can stay out of your territory before taking damage.
    private int _attackRange; // The range of Tiles a Unit can attack from. (Melee: 0, Ranged: 1 - X).
    private int _experience; // A Unit's current XP. Needs X amount for a Promotion.
    private bool _hasOrder; // Determines whether a Unit has already been given an order for this turn.
    private bool _exhausted; // Determines if a Unit still has moves to make this turn.
    private bool _fortified; // Determines if a Unit was ordered to Fortify this turn.
    private List<Promotion> _promotions; // Unlocked Promotions
    
    // References
    private GameTile _gameTile; //The Tile this Unit is on. 
    private Civilization _civilization; // The Civilization that owns this Unit.

    // Constants
    private const int Zero = 0;
    private const int TotalPromotions = 5;
    private const int UnitMaxHealth = 100;
    private const int HillAttackDebuf = 10;
    private const int RiverAttackDebuf = 10;

    /* Custom Unit Constructor (For testing) */
    public Unit(string name, int movementPoints, int combatStrength, int supplies, int attackRange)
    {
        _name = name;
        _health = UnitMaxHealth;
        _movementPoints = movementPoints;
        _combatStrength = combatStrength;
        _supplies = supplies;
        _attackRange = attackRange;
        _experience = Zero;
        _exhausted = false;
        _hasOrder = false;
        _fortified = false;
        _gameTile = null;
        _civilization = null;
        _promotions = new List<Promotion>();
    }
    
    public void OnTurnEnd()
    {
        if (_fortified)
        {
            _health += 10;
        }
        
        _exhausted = false;
        _hasOrder = false;
        _currentMovementPoints = _movementPoints;
    }

    public void UpdateUnit()
    {
        if (_currentMovementPoints <= 0)
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
        List<Tuple<GameTile,int>> path = Pathfinder.UnitAstar(_gameTile, target);
        foreach (var node in path)
        {
            _gameTile.SetUnit(null);
            _gameTile = node.Item1;
        }
        
        Debug.Log("Ended at" + _gameTile.GetXPos() +  _gameTile.GetYPos());
        Debug.Log("Was trying to arrive at" + target.GetXPos() +  target.GetYPos());
    }
    
    /* Move a Unit to one of its adjacent tiles */
    public void MoveOneTile(GameTile nextGameTile)
    {
        if (nextGameTile.GetMovementCost() <= GetMovementPoints() && IsExhausted()) // Check if the Unit has enough MP and isn't exhausted
        {
            SetTile(nextGameTile);
            SetMovementPoints(GetMovementPoints() - _gameTile.GetMovementCost()); // Reduce Unit's MP by tile's MC
        }
    }
    
    /* Attack a Unit */
    public void Attack(Unit target)
    {
        int unitStrength = GetCombatStrength(); // Unit's base Combat Strength
        int targetStrength = target.GetCombatStrength(); // Target's base Combat Strength

        if (target.GetTile().GetTerrain() == 1)
        {
            unitStrength -= HillAttackDebuf;
        }
        
        // Check If target is across a River edge from Unit
        //   unitStrength -= RiverAttackDebuf
        
        // Subtract the Unit's Combat Strength from the Health of the enemy.
        target.SetHealth(target.GetHealth() - unitStrength);

        // If Unit is melee, Unit receives damage too.
        if (GetAttackRange() == 0)
        {
            SetHealth(GetHealth() - target.GetCombatStrength());
        }
    }

    /* Attack a Settlement */
    public void Attack(Settlement target)
    {
        // To be implemented
    }
    public void SetName(string name)
    {
        _name = name;
    }
    public void SetHealth(int health)
    {
        _health = health;
    }

    public void Damage(int damage)
    {
        _health -= damage;
    }

    public void SetMovementPoints(int movementPoints)
    {
        _movementPoints = movementPoints;
    }

    public void SetCombatStrength(int combatStrength)
    {
        _combatStrength = combatStrength;
    }

    public void SetSupplies(int supplies)
    {
        _supplies = supplies;
    }

    public void SetAttackRange(int range)
    {
        _attackRange = range;
    }

    public void SetExperience(int experience)
    {
        _experience = experience;
    }

    public void SetExhausted(bool exhausted)
    {
        _exhausted = true;
    }

    public void SetFortified(bool fortified)
    {
        _fortified = fortified;
    }

    public void SetTile(GameTile gameTile)
    {
        _gameTile = gameTile;
    }

    public void SetCivilization(Civilization civilization)
    {
        _civilization = civilization;
    }
    
    // Getter Methods

    public int GetHealth()
    {
        return _health;
    }

    public int GetMovementPoints()
    {
        return _movementPoints;
    }

    public int GetCombatStrength()
    {
        return _combatStrength;
    }

    public int GetSupplies()
    {
        return _supplies;
    }

    public int GetAttackRange()
    {
        return _attackRange;
    }

    public int GetExperience()
    {
        return _experience;
    }

    public bool HasOrder()
    {
        return _hasOrder;
    }

    public bool IsExhausted()
    {
        return _exhausted;
    }

    public bool IsFortified()
    {
        return _fortified;
    }

    public GameTile GetTile()
    {
        return _gameTile;
    }
    
}
