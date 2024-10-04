using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour
{
    private int _goldPt; // A Civilization's Gold income per turn.
    private int _gold; // A Civilization's current gold (accumulated across turns and spent to buy Units/Buildings).
    private int _culturePt; // A Civilization's culture generation per turn.
    private int _culture; // A Civilization's current culture (accumulated across turns and spent on culture skills).
    private List<Settlement> _settlements; // A List of the Settlements this Civilization owns.
    private List<Unit> _units; // A List of the Units this Civilization owns.
    
}
