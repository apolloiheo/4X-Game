using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementWindow : MonoBehaviour
{
    public GameObject settlementWindow;
    public RectTransform unitsContent;
    public RectTransform buildingsContent;

    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestoryWindow();
        }
    }
    
    private void DestoryWindow()
    {
        settlementWindow.transform.SetParent(null);
        Destroy(settlementWindow);
    }
}
