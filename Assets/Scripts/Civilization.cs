using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Civilization 
{
    // NPC or Player
    private bool IsNPC;
    
    // Civilization Traits
    private TechnologyTree _technology; // ** WIP - Technology tree for each Civilization
    private TechnologyTree _currentTechnology;
    private Tree _cultureTree; // ** WIP - Cultural tree for each Civilization
    
    // Yields
    private int _goldPt; // A Civilization's Gold income per turn.
    private int _gold; // A Civilization's current gold (accumulated across turns and spent to buy Units/Buildings).
    private int _culturePt; // A Civilization's culture generation per turn.
    private int _culture; // A Civilization's current culture (accumulated across turns and spent on culture skills).
    private int _sciencePt; // A Civilization's science generation per turn.
    
    // Property
    private List<Settlement> _settlements; // A List of the Settlements this Civilization owns.
    private List<Unit> _units; // A List of the Units this Civilization owns.
    
    // Constant
    private const int Gold = 3;
    private const int Culture = 4;
    private const int Science = 5;

    public void OnTurnEnded()
    {
        // Units
        UpdateUnits();
        
        // Settlements
        UpdateSettlements();
        
        // Confirm that current Yields are up to date
        UpdateYields();
        
        // Yields Per Turn -> Total Yields + Technology Progress
        AddUpYields();
        
        void UpdateUnits()
        {
            if (_units is not null)
            {
                foreach (Unit unit in _units)
                {
                    unit.OnTurnEnd();
                }
            }
        }

        void UpdateSettlements()
        {
            if (_settlements is not null)
            {
                foreach (Settlement settlement in _settlements)
                {
                    settlement.OnTurnEnd();
                }
            }
        }

        void AddUpYields()
        {
            _gold += _goldPt;
            _culture += _culturePt;
            _currentTechnology.AddToProgress(_sciencePt);
        }
    }
    
    /* Called frequently to ensure Civilization displays the proper yields in GUI */
    public void UpdateYields()
    {
        if (_settlements is not null)
        {
            _goldPt = 0;
            _culturePt = 0;
            _sciencePt = 0;
            
            foreach (Settlement settlement in _settlements)
            {
                // Update each Settlement's Yields first 
                settlement.UpdateYields();

                // Add up all Civilization-wide yields from all Settlements
                _goldPt += settlement.GetYieldsPt()[Gold];
                _culturePt += settlement.GetYieldsPt()[Culture];
                _sciencePt += settlement.GetYieldsPt()[Science];
            }
        }
    }

    public void BuyTile(GameTile tile)
    {
        // To be implemented
    }

    public void ManageTiles()
    {
        // To be implemented
    }
}
