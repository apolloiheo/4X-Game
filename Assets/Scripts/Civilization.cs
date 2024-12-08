using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Civilization : ISerialization
{
    // NPC or Player
    [JsonProperty]
    public bool IsNPC;
    
    // Civilization Traits
    [JsonProperty]
    private TechnologyTree _technology; // ** WIP - Technology tree for each Civilization
    
    // Yields
    [JsonProperty]
    private int _goldPt; // A Civilization's Gold income per turn.
    [JsonProperty]
    private int _gold; // A Civilization's current gold (accumulated across turns and spent to buy Units/Buildings).
    [JsonProperty]
    private int _culturePt; // A Civilization's culture generation per turn.
    [JsonProperty]
    private int _culture; // A Civilization's current culture (accumulated across turns and spent on culture skills).
    [JsonProperty]
    private int _sciencePt; // A Civilization's science generation per turn.
    
    // Property
    [JsonProperty]
    public List<Settlement> _settlements; // A List of the Settlements this Civilization owns.
    [JsonProperty]
    public List<Unit> _units; // A List of the Units this Civilization owns.
    
    public Color32 _color;
    //[JsonProperty]
    //private Tree _cultureTree; // ** WIP - Cultural tree for each Civilization

    public Civilization()
    {
        _technology = new TechnologyTree();
        //_cultureTree = new Tree();
        _settlements = new List<Settlement>();
        _units = new List<Unit>();
    }
    
    public Civilization(Color32 color)
    {
        _technology = new TechnologyTree();
        //_cultureTree = new Tree();
        _settlements = new List<Settlement>();
        _units = new List<Unit>();
        _color = color;
    }
    
    // Constant
    private const int Gold = 3;
    private const int Culture = 4;
    private const int Science = 5;

    public void OnTurnEnded()
    {
        Debug.Log("test");
        
        // Units
        foreach (Unit unit in _units)
        {
            unit.OnTurnEnd();
            Debug.Log(unit._name);
        }
        
        // Settlements
        foreach (Settlement settlement in _settlements)
        {
            settlement.OnTurnEnd();
        }
        
        // Confirm that current Yields are up to date
        UpdateYields();
        
        // Yields Per Turn -> Total Yields + Technology Progress
        _gold += _goldPt;
        _culture += _culturePt;
        _technology._currentlyResearching.AddToProgress(_sciencePt);
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
    
    public void StageForSerialization()
    {
        if (_settlements is not null)
        {
            foreach (Settlement settlement in _settlements)
            {
                settlement.StageForSerialization();
            }
        }

        if (_units is not null)
        {
            foreach (Unit unit in _units)
            {
                unit.StageForSerialization();
            }
        }
        
        _technology = null;
    }

    public void RestoreAfterDeserialization(Game game)
    {
        if (_settlements is not null)
        {
            // Restore each Settlement's Owner Civilization 
            foreach (Settlement settlement in _settlements)
            {
                settlement._civilization = this;
                settlement.RestoreAfterDeserialization(game);
            }
        }

        if (_units is not null)
        {
            // Restore each Unit's Owner Civilization
            foreach (Unit unit in _units)
            {
                unit._civilization = this;
                unit.RestoreAfterDeserialization(game);
            }
        }
    }

    public void AddSettlement(Settlement settlement)
    {
        _settlements.Add(settlement);
    }

    public List<Settlement> GetSettlements()
    {
        return _settlements;
    }
}
