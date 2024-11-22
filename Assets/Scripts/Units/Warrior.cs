using System.Collections.Generic;

namespace Units
{
    public class Warrior : Unit
    {
        public Warrior(GameTile tile, Civilization civilization) : base(tile, civilization)
        {
            _name = "Warrior";
            _baseMP = 2;
            _combatStrength = 20;
            _supplies = 10;
            _attackRange = 0;
            _experience = 0;
            _hasOrder = false;
            _exhausted = false;
            _fortified = false;
            _gameTile = tile;
            _civilization = civilization;
            _promotions = new List<Promotion>();
        }
    }
}
