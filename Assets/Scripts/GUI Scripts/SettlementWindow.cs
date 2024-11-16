using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class SettlementWindow : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject settlementWindow;
    public RectTransform settlementCanvas;
    public RectTransform unitsContent;
    public RectTransform buildingsContent;
    public Director director;
    public TMP_Text settlementName;
    public GameObject citizenOn;
    public GameObject citizenOff;
    public GameObject citizenLocked;
    public Settlement _settlement;
    public Tilemap tilemap;
    
    private Dictionary<GameObject, GameTile> citizenUIs = new Dictionary<GameObject, GameTile>();

    public void Start()
    {
        director = FindObjectOfType<Director>();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestoryWindow();
        }
    }

    public void ToggleManageCitizens()
    {
        // Render once
        foreach (GameTile tile in _settlement._territory)
        {
            // Get a Vector3 Position of that Tile
            Vector3Int tilePosition = new Vector3Int(tile.GetYPos(), tile.GetXPos(), 0);
            Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
            
            // Convert Vector3 World Position to screen position
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            
            if (_settlement._workedTiles.Contains(tile))
            {
                // Spawn Citizen Image
                GameObject citizenUI = Instantiate(citizenOn, settlementCanvas.transform);
                
                // Position it relative to screen space
                RectTransform rectTransform = citizenUI.GetComponentInChildren<RectTransform>();
                rectTransform.position = screenPosition;
            }
            else if (_settlement._lockedTiles.Contains(tile))
            {
                // Spawn Locked Image
                GameObject lockedCitizen = Instantiate(citizenLocked, settlementCanvas.transform);
                
                // Position it relative to screen space
                RectTransform rectTransform = lockedCitizen.GetComponentInChildren<RectTransform>();
                rectTransform.position = screenPosition;
                
            }
            else if (!_settlement._workedTiles.Contains(tile))
            {
                // Spawn Unused Image
                GameObject offCitizen = Instantiate(citizenOff, settlementCanvas.transform);
                
                // Position it relative to screen space
                RectTransform rectTransform = offCitizen.GetComponentInChildren<RectTransform>();
                rectTransform.position = screenPosition;;
            }
        }
    }
    
    private void DestoryWindow()
    {
        settlementWindow.transform.SetParent(null);
        director._zoomedIn = false;
        director.RestoreCameraState();
        Destroy(settlementWindow);
    }
}
