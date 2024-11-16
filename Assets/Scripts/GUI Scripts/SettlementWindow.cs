using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class SettlementWindow : MonoBehaviour
{
    public GameObject settlementWindow;
    public Canvas _worldCanvas;
    public RectTransform unitsContent;
    public RectTransform buildingsContent;
    public Director director;
    public TMP_Text settlementName;
    public GameObject citizenOn;
    public GameObject citizenOff;
    public GameObject citizenLocked;
    public Settlement _settlement;
    public Tilemap tilemap;
    
    private HashSet<GameObject> citizenUIs = new HashSet<GameObject>();

    public void Start()
    {
        director = FindObjectOfType<Director>();
        citizenUIs = new HashSet<GameObject>();
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
        if (citizenUIs.Count <= 0)
        {
            // Initialize
            citizenUIs = new HashSet<GameObject>();
            
            // Render once
            foreach (GameTile tile in _settlement._territory)
            {
                // Get a Vector3 Position of that Tile
                Vector3Int tilePosition = new Vector3Int(tile.GetYPos(), tile.GetXPos(), 0);
                Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
            
                if (_settlement._workedTiles.Contains(tile))
                {
                    // Spawn Citizen Image
                    GameObject citizenUI = Instantiate(citizenOn, _worldCanvas.transform);
                
                    // Position it relative to screen space
                    RectTransform rectTransform = citizenUI.GetComponentInChildren<RectTransform>();
                    rectTransform.position = worldPosition;
                    
                    // Add it to Hash Set to keep track of it
                    citizenUIs.Add(citizenUI);
                }
                else if (_settlement._lockedTiles.Contains(tile))
                {
                    // Spawn Locked Image
                    GameObject lockedCitizen = Instantiate(citizenLocked, _worldCanvas.transform);
                
                    // Position it relative to screen space
                    RectTransform rectTransform = lockedCitizen.GetComponentInChildren<RectTransform>();
                    rectTransform.position = worldPosition;
                    
                    // Add it to Hash Set to keep track of it
                    citizenUIs.Add(lockedCitizen);
                
                }
                else if (!_settlement._workedTiles.Contains(tile))
                {
                    // Spawn Unused Image
                    GameObject offCitizen = Instantiate(citizenOff, _worldCanvas.transform);
                
                    // Position it relative to screen space
                    RectTransform rectTransform = offCitizen.GetComponentInChildren<RectTransform>();
                    rectTransform.position = worldPosition;;
                    
                    // Add it to Hash Set to keep track of it
                    citizenUIs.Add(offCitizen);
                }
            } 
        }
        else
        {
            foreach (GameObject ui in citizenUIs)
            {
                Destroy(ui);
            }

            citizenUIs = new HashSet<GameObject>();
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
