using System.Collections.Generic;

namespace Units
{
    public class Settler : Unit
    {
        public Settler(GameTile tile, Civilization civilization) : base(tile, civilization)
        {
            _name = "Settler";
            _baseMP = 2;
            _currMP = _baseMP;
            _combatStrength = 5;
            _supplies = 15;
            _attackRange = 0;
            _experience = 0;
            _hasOrder = false;
            _exhausted = false;
            _fortified = false;
            _gameTile = tile;
            _position = new Point(_gameTile.GetYPos(), _gameTile.GetXPos());
            _civilization = civilization;
            _promotions = new List<Promotion>();
        }
    }
}
