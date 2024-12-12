using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitPrefab : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite warrior;
    public Sprite settler;
    public Unit unit;
    public Director director;
    
    public void UpdatePrefab()
    {
        if (unit._name == "Warrior")
        {
            spriteRenderer.sprite = warrior;
        } else if (unit._name == "Settler")
        {
            spriteRenderer.sprite = settler;
        }
    }

    public void OnClick()
    {
        if (!director._zoomedIn)
        {
            director.SelectUnit(unit);
            director.selectedUnitPrefab = this.gameObject;
        }
    }
}
