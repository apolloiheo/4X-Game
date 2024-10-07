using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour
{
    private int _goldPt; // A Civilization's Gold income per turn.
    private int _gold; // A Civilization's current gold (accumulated across turns and spent to buy Units/Buildings).
    private int _culturePt; // A Civilization's culture generation per turn.
    private int _culture; // A Civilization's current culture (accumulated across turns and spent on culture skills).
    private int _sciencePt; // A Civilization's science generation per turn.
    private List<Settlement> _settlements; // A List of the Settlements this Civilization owns.
    private List<Unit> _units; // A List of the Units this Civilization owns.
    private TechnologyTree _technology; // ** WIP - Technology tree for each Civilization
    private Tree _cultureTree; // ** WIP - Cultural tree for each Civilization

    
    // Constant
    private const int Gold = 2;
    private const int Culture = 3;
    private const int Science = 4;

    private void Start()
    {
        // Listen to GameManager OnTurnEnd event
        GameManager.Instance.OnTurnEnd += Instance_OnTurnEnd;
    }

    // End the turn
    private void Instance_OnTurnEnd(object sender, System.EventArgs e)
    {
        _gold += _goldPt;
        _culture += _culturePt;

        // To be implemented
        // Science per turn gets added to the current Technology being researched
        _technology.AddToProgress(_sciencePt);



    }

    public void CollectYieldsPt()
    {
        _goldPt = 0;
        _culturePt = 0;
        _sciencePt = 0;
        
        // Add up Gold Per Turn
        foreach (Settlement settlement in _settlements)
        {
            _goldPt += settlement.GetYieldsPt()[Gold];
            _culturePt += settlement.GetYieldsPt()[Culture];
            _sciencePt += settlement.GetYieldsPt()[Science];
        }
        
    }
    
    
    
    
    
    
    

}
