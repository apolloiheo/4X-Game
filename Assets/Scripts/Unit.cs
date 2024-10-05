using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
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
    private GameTile _gameTile; //The Tile this Unit is on. 
    private Civilization _civilization; // The Civilization that owns this Unit.
    private bool[] _promotions; // Promotions are Unit powers/abilities - Array index determines whether a promotion/power has been unlocked. WILL BE REDONE INTO A NODE TREE LATER

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
        _promotions = new bool[TotalPromotions]; // No Promotions (booleans are initialized to False)
    }

    private void Start()
    {
        // Listen to GameManager OnTurnEnd event
        GameManager.Instance.OnTurnEnd += Instance_OnTurnEnd;
    }

    // End the turn
    private void Instance_OnTurnEnd(object sender, System.EventArgs e)
    {
        if (_fortified)
        {
            _health += 10;
        }
        
        _exhausted = false;
        _hasOrder = false;
        _currentMovementPoints = _movementPoints;
    }

    // Public methods

    /* Move a Unit across Tiles */
    public void Move()
    {
        // To be implemented
        
    }
    
    /* Move a Unit to one of it's adjacent tiles */
    public void MoveOneTile(GameTile nextGameTile)
    {
        if (nextGameTile.GetMovementCost() <= GetMovementPoints() && IsExhausted()) // Check if the Unit has enough MP and isn't exhausted
        {
            SetTile(nextGameTile);
            SetMovementPoints(GetMovementPoints() - _gameTile.GetMovementCost()); // Reduce Unit's MP by tile's MC
        }
    }

    
    /* Attack another Unit */
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

    /* Attack another Settlement */
    public void Attack(Settlement target)
    {
        // To be implemented
    }
    
    // Private Methods
    
    
    
    //Setter Methods
    public void SetName(string name)
    {
        _name = name;
    }
    public void SetHealth(int health)
    {
        _health = health;
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

    public void SetPromotions(bool[] promotions)
    {
        // Will need to add new promotions without removing previous ones
        _promotions = promotions;
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
    
    public bool[] GetPromotions()
    {
        return _promotions;
    }
    
}
