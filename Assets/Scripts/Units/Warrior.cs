using System.Collections.Generic;

namespace Units
{
    public class Warrior : Unit
    {
        public Warrior(GameTile tile, Civilization civilization) : base(tile, civilization)
        {
            _name = "Warrior";
            _baseMP = 2;
            _currMP = _baseMP;
            _combatStrength = 20;
            _supplies = 10;
            _health = 100;
            _attackRange = 0;
            _experience = 0;
            _hasOrder = false;
            _exhausted = false;
            _camping = false;
            _gameTile = tile;
            _position = new Point(_gameTile.GetYPos(), _gameTile.GetXPos());
            _civilization = civilization;
            _promotions = new List<Promotion>();
        }
    }
}
