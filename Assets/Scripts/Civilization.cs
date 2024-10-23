using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour
{
    // NPC or Player
    private bool IsNPC;
    
    // Civilization Traits
    private TechnologyTree _technology; // ** WIP - Technology tree for each Civilization
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
    private const int Gold = 2;
    private const int Culture = 3;
    private const int Science = 4;
    
    
    
    
    
    
    
    
    

}
