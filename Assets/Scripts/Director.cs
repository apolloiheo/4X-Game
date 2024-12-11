using System;
using System.Collections.Generic;
using System.Linq;
using City_Projects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Director : MonoBehaviour
{
    // Serialized Variables
    [Header("Game Manager")] public GameManager gm;
    [Header("Cameras")] public Camera mainCam;
    [Header("Canvases")] public Canvas worldCanvas;
    public GameObject menuCanvas;
    public GameObject guiCanvas;
    public GameObject saveCanvas;
    public GameObject unitWindowCanvas;
    public TMP_InputField saveIF;
    public GameObject settlementWindowCanvas;
    [Header("Owner")] public Civilization civilization;
    [Header("Tilemaps")] public Tilemap baseTilemap;
    public Tilemap hillsTilemap;
    public Tilemap mountainTilemap;
    public Tilemap featureTilemap;
    public Tilemap shadingTilemap;
    [Header("Flat Tiles")] public Tile tile;
    public Tile prairieTile;
    public Tile grassTile;
    public Tile tundraTile;
    public Tile desertTile;
    public Tile oceanTile;
    public Tile coastTile;
    public Tile snowTile;
    public Tile lakeTile;
    [Header("Hills Tiles")] public Tile prairieHillsTile;
    public Tile grassHillsTile;
    public Tile tundraHillsTile;
    public Tile desertHillsTile;
    public Tile snowHillsTile;
    [Header("Features")] public Tile woodsTile_grassland;
	public Tile woodsTile_plains;
    public Tile floodplainsTile;
    public Tile marshTile;
    public Tile rainforestTile;
    public Tile oasisTile;
    [Header("Terrain")] 
    public Tile mountain;
    [Header("Shading Tiles")] 
    public Tile highlight;
    public Tile unexplored;
    public Tile darkened;
    [Header("Rivers")] 
    public GameObject riversParent;
    public GameObject riverSegment;
    [Header("Settlements")] public Tile village;
    [Header("Units")] public Tile warrior;
    [Header("UI Prefabs")] public GameObject settlementUI;
    public GameObject settlementWindow;
    public GameObject territoryParent;
    public GameObject territoryLines;
    [Header("Unit Selection")] 
    public Unit selectedUnit;
    public GameObject selectedUnitPrefab;
    public TMP_Text selectedUnitName;
    public TMP_Text selectedUnitMovementPoints;
    public GameObject unitPrefab;
    public GameObject unitsParent;

    // Private Instance Attributes
    private bool _needsDirection;
    private Dictionary<GameTile, GameObject> settlementUIs = new Dictionary<GameTile, GameObject>();
    private Dictionary<Unit, GameObject> units = new Dictionary<Unit, GameObject>();
    private HashSet<Point> highlightedTiles = new HashSet<Point>();

    // Camera Values
    public bool _zoomedIn;
    private Vector3 _prevPos;
    private float _prevSize;

    // Camera Constants 
    private const float dragSpeed = 10f;
    private const float zoomSpeed = 2f;
    private const float minZoom = 2f;

    private const float maxZoom = 15f;

    // Grid Dimensions
    private const double tileHeight = 0.95f;
    private const double tileWidth = 1f;

    // Camera References
    private Vector3 dragOrign;

    // Start is called before the first frame update
    void Start()
    {
        // Find GM objects
        gm = FindObjectOfType<GameManager>();

        // If Game is Single player
        if (gm.game.singlePlayer)
        {
            foreach (Civilization civ in gm.game.civilizations)
            {
                if (!civ.IsNPC)
                {
                    civilization = civ;
                }
            }
        }

        // Situate the World Canvas for UI (based on tilemap size)
        SetUpWorldCanvas();

        // Render the game (once)
        RenderGame();

        // Player needs to take action -> True
        _needsDirection = true;
    }

    // Update is called every frame
    void Update()
    {
        // Pressing Esc - Deselected Unit
        if (Input.GetKeyDown(KeyCode.Escape) && !_zoomedIn)
        {
            // Toggle Unit Selected
            if (unitWindowCanvas.activeSelf)
            {
                // Deactive it
                unitWindowCanvas.SetActive(false);
                // Unselect Unit
                selectedUnit = null;
                selectedUnitPrefab = null;
                //Remove possibleMoves() highlighted tiles
                RemovePossibleMoves();
                return;
            }

            // Toggle Menu Canvas
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            // Ensure that save canvas starts off
            saveCanvas.SetActive(false);
            return;
        }

        if (!_zoomedIn)
        {
            CameraControl();
        }

        // Unit Movement
        MoveSelectedUnit();
    }

    private void MoveSelectedUnit()
    {
        if (selectedUnit is not null)
        {
            if (Input.GetMouseButtonDown(1)) // 1 - Right Mouse button
            {
                Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                
                Vector3Int cellPosition = shadingTilemap.WorldToCell(mousePos);

                // If that location is highlighted
                if (shadingTilemap.HasTile(cellPosition))
                {
                    // Move Unit in GM
                    gm.MoveUnit(selectedUnit, new Point(cellPosition.y, cellPosition.x));
                    
                    // Reposition Selected Unit prefab
                    RenderUnits();
                    
                    // Reset previous moves
                    RemovePossibleMoves();
                    
                    // Display New Possible Moves
                    DisplayPossibleMoves(selectedUnit);
                    
                    // Update Unit Window Values
                    OpenUnitWindow();

                    // If Selected Unit can no longer move (out of movement points)
                    if (!selectedUnit.canMove())
                    {
                        RemovePossibleMoves();
                    }
                }
            }
        }
    }

    public void OpenUnitWindow()
    {
        // Activate the Unit Window Canvas
        unitWindowCanvas.SetActive(true);

        // Update Window Values to Selected Unit Properties
        
        // Name
        selectedUnitName.text = selectedUnit._name;
        // Movement Points
        selectedUnitMovementPoints.text = selectedUnit._currMP.ToString();
    }
    
    public void ToggleSettlementWindow(Settlement settlement)
    {
        // Tell the director we are zoom in
        _zoomedIn = true;

        // Turn off the Base Canvas
        guiCanvas.SetActive(false);
        
        // Store previous camera size and positions
        _prevPos = mainCam.transform.position;
        _prevSize = mainCam.orthographicSize;
        
        // Deactivate the settlementUI
        settlementUIs[settlement.GetTile()].SetActive(false);
        
        // Instantiate Settlement Window Canvas
        settlementWindow = Instantiate(settlementWindowCanvas);
        
        // Ensure it is active
        settlementWindow.SetActive(true);
    
        // Give Settlement Window Script info necessary references
        settlementWindow.GetComponent<SettlementWindow>()._settlement = settlement;
        settlementWindow.GetComponent<SettlementWindow>().tilemap = baseTilemap;
        settlementWindow.GetComponent<SettlementWindow>()._worldCanvas = worldCanvas;
        settlementWindow.GetComponent<SettlementWindow>().settlementName.text = settlement.GetName();
        
        ZoomCameraAtSettlement();
        
        void ZoomCameraAtSettlement()
        {
            // Settlement's Game Tile
            GameTile tile = settlement.GetTile();
            
            // Settlement's position
            Vector3 settlementPosition = baseTilemap.CellToWorld(new Vector3Int(tile.GetYPos(), tile.GetXPos(), -1));
            
            // Move camera into the Settlement Window view
            mainCam.transform.position = settlementPosition;
            mainCam.orthographicSize = 4f;
        }
    }

    public void RestoreCameraState()
    {
        mainCam.transform.position = _prevPos;
        mainCam.orthographicSize = _prevSize;

        foreach (GameObject uiPrefab in settlementUIs.Values)
        {
            uiPrefab.SetActive(true);
        }
        
        RenderSettlementUI(gm.game.world);
    }

    // Load Main Menu Scene
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    // Open the Save Game Menu
    public void OpenSaveGameCanvas()
    {
        saveCanvas.SetActive(true);
    }

    // Send Save Game Command to Game
    public void SendSaveToGm()
    {
        gm.SaveGame(saveIF.text);

        saveCanvas.SetActive(false);
    }

    public void SendEndTurnToGM()
    {
        gm.EndTurn();

        // Update Settlement UIs
        foreach (GameTile tile in settlementUIs.Keys)
        {
            UpdateUIFields(settlementUIs[tile], tile.GetSettlement());
        }

        // Update Selected Unit
        if (selectedUnit != null)
        {
            DisplayPossibleMoves(selectedUnit);
            OpenUnitWindow();
        }
        
        RenderUnits();
    }
    
    /* Selects the Unit in the parameter, open's the unit window. */
    public void SelectUnit(Unit unit)
    {
        // Assign Selected Unit
        selectedUnit = unit;
        
        // Set UnitWindow Active and Update it's text
        OpenUnitWindow();

        // Display that Unit's possible moves through the Shading Tilemap (highlight tiles)
        DisplayPossibleMoves(unit);
    }

    /* Resizes World Canvas to match Tilemap size (should work with different world sizes or grid/cell sizes) */
    void SetUpWorldCanvas()
    {
        // Set exact dimensions based on editor observations
        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        canvasRect.transform.position = new Vector3(worldCanvas.transform.position.x / 2,
            worldCanvas.transform.position.y / 2, worldCanvas.transform.position.z);
        canvasRect.sizeDelta = new Vector2(74.2f, 47);
        canvasRect.pivot = new Vector2(74.2f / 2, 47f / 2);

        // float tileWidth = baseTilemap.cellSize.x;
        // float tileHeight = baseTilemap.cellSize.y;
        // float tileCountX = gm.game.world.GetLength();
        // float tileCountY = gm.game.world.GetHeight();
        //
        // // Calculate the canvas dimensions based on tilemap size and tile dimensions
        // float canvasWidth = tileWidth * (tileCountX * .75f + 0.25f);
        // float canvasHeight = tileHeight * (tileCountY + 0.5f);
        //
        // // Adjust the RectTransform of the Canvas to match these dimensions
        // RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        // canvasRect.sizeDelta = new Vector2(canvasWidth, canvasHeight);
        //
        // // Position the Canvas in the center of the tilemap.
        // float offsetX = canvasWidth / 2f;
        // float offsetY = canvasHeight / 2f;
        // canvasRect.position = new Vector3(offsetX, offsetY, 0);
    }

    /* Render everything in the Game. */
    void RenderGame()
    {
        World gameWorld = gm.game.world;
        RenderTilemaps(gameWorld);
        RenderSettlementUI(gameWorld);
        RenderTerritoryLines();
        RenderUnits();
    }

    /* Renders Settlement UI above Settlement Tiles */
    void RenderSettlementUI(World world)
    {
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                GameTile currTile = world.GetTile(x, y);

                // if a Tile has a Settlement
                if (currTile.GetSettlement() is not null)
                {
                    // If a prefab for that Tile's settlement doesn't exist, instantiate it.
                    if (!settlementUIs.ContainsKey(currTile))
                    {
                        // Instantiate UI Prefab
                        GameObject uiInstance = Instantiate(settlementUI, worldCanvas.transform);

                        // Store the Settlement within Prefab
                        SettlementUI ui = uiInstance.GetComponent<SettlementUI>();
                        ui.settlement = currTile.GetSettlement();

                        // Update the UI's fields
                        UpdateUIFields(uiInstance, currTile.GetSettlement());

                        // Add this UI Prefab to our active SettlementUIs
                        settlementUIs.Add(currTile, uiInstance);

                        // Get the tile's world position
                        Vector3Int cellPosition = new Vector3Int(y, x, 0);
                        Vector3 tileWorldPosition = baseTilemap.CellToWorld(cellPosition);

                        // Make the UI Instance appear slightly above the Tile
                        tileWorldPosition += new Vector3(0, 0.7f, 0);

                        // Set the position of the UI element
                        uiInstance.transform.position = tileWorldPosition;
                    }
                    else
                    {
                        // Update that UI prefab's text yields.
                        UpdateUIFields(settlementUIs[currTile], currTile.GetSettlement());
                    }
                }
            }
        }
    }
    
    // Helper method to update UI fields for a given settlement
    void UpdateUIFields(GameObject uiObject, Settlement settlement)
    {
        // Access TMP_Text Objs - These need to be access through a script transform. Find is too expensive 
        TMP_Text population = uiObject.transform.Find("Population Text").GetComponent<TMP_Text>();
        TMP_Text growth = uiObject.transform.Find("Growth Text").GetComponent<TMP_Text>();
        TMP_Text production = uiObject.transform.Find("Production Text").GetComponent<TMP_Text>();
        TMP_Text name = uiObject.transform.Find("Name Text").GetComponent<TMP_Text>();

        // Update the text fields with the settlement's data
        population.text = settlement.GetPopulation().ToString();
        growth.text = settlement.TurnsToGrow();
        production.text = settlement.TurnsToProduce();
        name.text = settlement.GetName();
    }

    /* Spawns Territory Lines on Tile Edges around Settlement's territory. */
    void RenderTerritoryLines()
    {
        foreach (Civilization civ in gm.game.civilizations)
        {
            foreach (Settlement settlement in civ.GetSettlements())
            {
                foreach (GameTile tile in settlement._territory)
                {
                    int edge = 0;
                    foreach (GameTile neighbor in tile.GetNeighbors())
                    {
                        if (neighbor is not null)
                        {
                            // Tile Position Variables - Jason knows how they work don't ask me.
                            double bigX = tileWidth * tile.GetXPos() * .75f;
                            double bigY =
                                (float)(tile.GetYPos() * tileHeight + (tileHeight / 2) * (tile.GetXPos() % 2));

                            // If this edge is at the end of the Settlement's territory 
                            if (!settlement._territory.Contains(neighbor))
                            {
                                // Put a Territory on that edge// Instiate Vector3 for Position at Formula for River Position
                                Vector3 territoryPosition = new Vector3((float)(bigX +
                                        Math.Pow(-1f,
                                            Math.Pow(0f,
                                                (5f - edge) * (4f - edge))) *
                                        Math.Pow(0f, Math.Pow(0f, edge % 3f)) *
                                        tileWidth * 3 / 8),
                                    (float)(bigY + Math.Pow(-1f,
                                            Math.Pow(0f, Math.Abs((edge - 2f) * (edge - 3f) * (edge - 4f)))) *
                                        (tileHeight / 4f + tileHeight / 4f *
                                            Math.Abs(Math.Pow(0f, Math.Pow(0f, edge % 3f)) - 1f))),
                                    0f);

                                // Declare riverRotation variable
                                Quaternion territoryRotation;

                                if (edge == 1 || edge == 4)
                                {
                                    // Set the rotation of the river based on it's edge
                                    territoryRotation = Quaternion.Euler(0f, 0f, -63f);
                                }
                                else if (edge == 5 || edge == 2)
                                {
                                    // Set the rotation of the river based on it's edge
                                    territoryRotation = Quaternion.Euler(0f, 0f, 63f);
                                }
                                else
                                {
                                    // Set the rotation of the river based on it's edge
                                    territoryRotation = Quaternion.Euler(0f, 0f, 0f);
                                }

                                // Instantiate as part of the Rivers obj in order to not clog up hierarchy
                                GameObject territoryLine =
                                    Instantiate(territoryLines, territoryPosition, territoryRotation);
                                territoryLine.GetComponent<TerritroyLine>().color = civ._color;
                                territoryLine.transform.SetParent(territoryParent.transform);
                            }
                        }

                        edge++;
                    }
                }
            }
        }
    }

    /* Renders all Tilemaps */
    void RenderTilemaps(World world)
    {
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                GameTile currTile = world.GetTile(x, y);

                // Tile Position Variables - Jason knows how they work don't ask me.
                double bigX = tileWidth * x * .75f;
                double bigY = (float)(y * tileHeight + (tileHeight / 2) * (x % 2));

                /* Render Base Tiles */
                // Plains
                if (currTile.GetBiome() == 1)
                {
                    // Base Flat Tile
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), prairieTile);

                    // Hills
                    if (currTile.GetTerrain() == 1)
                    {
                        hillsTilemap.SetTile(new Vector3Int(y, x, 0), prairieHillsTile);
                    }
                    //tile.color = new Color32(145, 158, 11, 255);
                }
                // Grassland
                else if (currTile.GetBiome() == 2)
                {
                    // Flat Tile
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), grassTile);

                    // Hills
                    if (currTile.GetTerrain() == 1)
                    {
                        hillsTilemap.SetTile(new Vector3Int(y, x, 0), grassHillsTile);
                    }
                    //tile.color = new Color32(92, 128, 82, 255);
                }
                // Tundra
                else if (currTile.GetBiome() == 3)
                {
                    // Flat Tile
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), tundraTile);

                    // Hills
                    if (currTile.GetTerrain() == 1)
                    {
                        hillsTilemap.SetTile(new Vector3Int(y, x, 0), tundraHillsTile);
                    }

                    //tile.color = new Color32(144, 158, 141, 255);
                }
                // Desert
                else if (currTile.GetBiome() == 4)
                {
                    // Flat Tile
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), desertTile);

                    // Hills
                    if (currTile.GetTerrain() == 1)
                    {
                        hillsTilemap.SetTile(new Vector3Int(y, x, 0), desertHillsTile);
                    }

                    //tile.color = new Color32(255, 217, 112, 255);
                }
                // Snow
                else if (currTile.GetBiome() == 5)
                {
                    // Flat Tile
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), snowTile);

                    // Hills
                    if (world.GetTile(x, y).GetTerrain() == 1)
                    {
                        hillsTilemap.SetTile(new Vector3Int(y, x, 0), snowHillsTile);
                    }
                    //tile.color = Color.white;
                }
                // Coast
                else if (currTile.GetBiome() == 6)
                {
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), coastTile);
                    //tile.color = new Color32(110, 187, 255, 255);
                }
                // Ocean
                else if (currTile.GetBiome() == 7)
                {
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), oceanTile);
                    //tile.color = new Color32(20, 102, 184, 255);
                }
                // Lake
                else if (currTile.GetBiome() == 8)
                {
                    baseTilemap.SetTile(new Vector3Int(y, x, 0), lakeTile);
                }

                // Mountains
                if (currTile.GetTerrain() == 2)
                {
                    mountainTilemap.SetTile(new Vector3Int(y, x, 0), mountain);
                }

                /* Render Rivers */
                if (currTile.GetFreshWaterAccess())
                {
                    // For testing FreshWaterAccess
                    /*baseTilemap.SetTile(new Vector3Int(y, x, 0), tile);
                    tile.color = Color.white;*/

                    for (int index = 0; index < 6; index++)
                    {
                        if (currTile.GetRiverEdge(index))
                        {
                            // Instiate Vector3 for Position at Formula for River Position
                            Vector3 riverPosition = new Vector3((float)(bigX +
                                                                        Math.Pow(-1f,
                                                                            Math.Pow(0f,
                                                                                (5f - index) * (4f - index))) *
                                                                        Math.Pow(0f, Math.Pow(0f, index % 3f)) *
                                                                        tileWidth * 3 / 8),
                                (float)(bigY + Math.Pow(-1f,
                                        Math.Pow(0f, Math.Abs((index - 2f) * (index - 3f) * (index - 4f)))) *
                                    (tileHeight / 4f + tileHeight / 4f *
                                        Math.Abs(Math.Pow(0f, Math.Pow(0f, index % 3f)) - 1f))),
                                0f);
                            // Declare riverRotation variable
                            Quaternion riverRotation;

                            if (index == 1 || index == 4)
                            {
                                // Set the rotation of the river based on it's edge
                                riverRotation = Quaternion.Euler(0f, 0f, -63f);
                            }
                            else if (index == 5 || index == 2)
                            {
                                // Set the rotation of the river based on it's edge
                                riverRotation = Quaternion.Euler(0f, 0f, 63f);
                            }
                            else
                            {
                                // Set the rotation of the river based on it's edge
                                riverRotation = Quaternion.Euler(0f, 0f, 0f);
                            }

                            // Instantiate as part of the Rivers obj in order to not clog up hierarchy
                            GameObject riverPiece = Instantiate(riverSegment, riverPosition, riverRotation);
                            riverPiece.transform.SetParent(riversParent.transform);

                        }
                    }
                }

                /* Render Features */
                // Woods
                if (currTile.GetFeature() == 1)
                {
					if (currTile.GetBiome() == 1)
					{
                    	featureTilemap.SetTile(new Vector3Int(y, x, 0), woodsTile_plains);
					}
					else 
					{
						featureTilemap.SetTile(new Vector3Int(y, x, 0), woodsTile_grassland);
					}
                }
                // Floodplains
                else if (currTile.GetFeature() == 2)
                {
                    featureTilemap.SetTile(new Vector3Int(y, x, 0), floodplainsTile);
                }
                // Marshes
                else if (currTile.GetFeature() == 3)
                {
                    featureTilemap.SetTile(new Vector3Int(y, x, 0), marshTile);
                }
                // Rainforest
                else if (currTile.GetFeature() == 4)
                {
                    featureTilemap.SetTile(new Vector3Int(y, x, 0), rainforestTile);
                }
                // Oasis
                else if (currTile.GetFeature() == 5)
                {
                    featureTilemap.SetTile(new Vector3Int(y, x, 0), oasisTile);
                }

                /* Render Settlements */
                if (currTile.GetSettlement() is not null)
                {
                    featureTilemap.SetTile(new Vector3Int(y, x, 0), village);
                }
            }
        }
    }

    /* Render Units */
    void RenderUnits()
    {
        for (int x = 0; x < gm.game.world.GetLength(); x++)
        {
            for (int y = 0; y < gm.game.world.GetHeight(); y++)
            {
                GameTile currTile = gm.game.world.GetTile(x, y);

                // If the Tile has a Unit on it
                if (currTile.GetUnit() is not null)
                {
                    // Calculate World Position of that Tile
                    Vector3Int tilePosition = new Vector3Int(y, x, 0);
                    Vector3 worldPosition = baseTilemap.CellToWorld(tilePosition);
                    
                    // Store the Unit on that Tile
                    Unit currUnit = currTile.GetUnit();
                    
                    // If it's already instantiated and tracked
                    if (units.Keys.Contains(currUnit))
                    {
                        // Move GameObj Prefab to it's correct position
                        units[currUnit].transform.position = worldPosition;
                    }
                    else
                    {
                        // Instantiate a Prefab for that new Unit
                        GameObject unitOBJ = Instantiate(unitPrefab, unitsParent.transform);

                        // Place Game Obj Prefab on that Tile
                        unitOBJ.transform.position = worldPosition;

                        // Give the Prefab reference to its own Unit
                        unitOBJ.GetComponent<UnitPrefab>().unit = currTile.GetUnit();
                        unitOBJ.GetComponent<UnitPrefab>().director = this;
                    
                        // Update it's art and values
                        unitOBJ.GetComponent<UnitPrefab>().UpdatePrefab();

                        // Add it to our HashSet tracker
                        units.Add(currUnit, unitOBJ);
                    }
                }
            }
        }
    }

    void DisplayPossibleMoves(Unit unit)
    {
        // If unit can't move, don't highlight anything
        if (!unit.canMove())
        {
            return;
        }
        
        // Update the Unit's instance possible moves property
        unit.GetPossibleMoves(unit._gameTile, unit._currMP, true);
        
        // If there are moves 
        if (unit.possible_moves.Count > 0)
        {
            foreach (Point point in unit.possible_moves)
            {
                // On a shading Tilemap, place highlights on the possible moves
                shadingTilemap.SetTile(new Vector3Int(point.y, point.x), highlight);

                // Store that point
                highlightedTiles.Add(point);
            }
        }
    }

    void RemovePossibleMoves()
    {
        if (highlightedTiles.Count > 0)
        {
            foreach (Point point in highlightedTiles)
            {
                // Remove that tile
                shadingTilemap.SetTile(new Vector3Int(point.y, point.x), null);
            }
            
            highlightedTiles.Clear();
        }
    }

    /* Calls all the other Camera methods. */
    void CameraControl()
    {
        DragCamera();
        HandleZoom();
        
        /* Moves the Camera by dragging the world with Left-Click. */
        void DragCamera()
        {
            // When left mouse is clicked
            if (Input.GetMouseButtonDown(0))
            {
                dragOrign = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }

            // While left mouse is held
            if (Input.GetMouseButton(0))
            {
                Vector3 difference = dragOrign - mainCam.ScreenToWorldPoint(Input.mousePosition);
                mainCam.transform.position += difference * (dragSpeed * Time.deltaTime);
            }
        }
    
        /* Changes the size of the camera with Mouse Scroll wheel. */
        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            mainCam.orthographicSize -= scroll * zoomSpeed;
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minZoom, maxZoom);
        }
    }
}
