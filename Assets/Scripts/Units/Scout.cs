using System.Collections.Generic;

namespace Units
{
    public class Scout : Unit
    {
        public Scout(GameTile tile, Civilization civilization) : base(tile, civilization)
        {
            _name = "Scout";
            _baseMP = 3;
            _currMP = _baseMP;
            _combatStrength = 10;
            _supplies = 8;
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
