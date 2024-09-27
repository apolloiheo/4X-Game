using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Instance Properties
    private string _name; // The Unit's name.
    private int _health; // A Unit's current Health Points (Default of 100)
    private int _movementPoints; // A Unit's current Movement Points per turn.
    private int _combatStrength; // A Unit's base Combat Strength Stat - used to determine Attack Damage and Defense Damage.
    private int _supplies; // A Unit's current Supplies stat - Determines how many turns it can stay out of your territory before taking damage.
    private int _attackRange; // The range of Tiles a Unit can attack from. (Melee: 0, Ranged: 1 - X).
    private int _experience; // A Unit's current XP. Needs X amount for a Promotion.
    private bool _hasOrder; // Determines whether a Unit has already been given an order for this turn.
    private bool _exhausted; // Determines if a Unit still has moves to make this turn.
    private bool _fortifed; // Determines if a Unit was ordered to Fortify this turn.
    private Tile _tile; //The Tile this Unit is on. 
    private Civilization _civilization; // The Civilization that owns this Unit.
    private bool[] _promotions; // Promotions are Unit powers/abilities - Array index determines whether a promotion/power has been unlocked. WILL BE REDONE INTO A NODE TREE LATER

    // Constants
    private const int Zero = 0;
    private const int TotalPromotions = 5;
    private const int UnitMaxHealth = 100;

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
        _fortifed = false;
        _tile = null;
        _civilization = null;
        _promotions = new bool[TotalPromotions]; // No Promotions (booleans are initialized to False)
    }

    /* Move a Unit across Tiles */
    public void Move()
    {
        // To be implemented
        
    }
    
    /* Attack another Unit */
    public void Attack(Unit target)
    {
        // To be implemented
    }

    /* Attack another Settlement */
    public void Attack(Settlement settlement)
    {
        // To be implemented
    }
}
