using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFlag : MonoBehaviour
{
    public Director director;
    public Unit unit;

    public void OnClick()
    {
        director.selectedUnit = unit;
        director.OpenUnitWindow();
    }
}
