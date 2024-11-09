using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Director : MonoBehaviour
{
    // Serialized Variables
    [Header("Game Manager")] 
    public GameManager gm;
    [Header("Cameras")] 
    public Camera mainCam;
    [Header("Canvases")] 
    public Canvas worldCanvas;
    public GameObject menuCanvas;
    public GameObject guiCanvas;
    public GameObject saveCanvas;
    public TMP_InputField saveIF;
    public GameObject settlementWindowCanvas;
    [Header("Owner")] 
    public Civilization civilization;
    [Header("Tilemaps")] 
    public Tilemap baseTilemap;
    public Tilemap hillsTilemap;
    public Tilemap mountainTilemap;
    public Tilemap featureTilemap;
    public Tilemap unitTilemap;
    [Header("Flat Tiles")] 
    public Tile tile;
    public Tile prairieTile;
    public Tile grassTile;
    public Tile tundraTile;
    public Tile desertTile;
    public Tile oceanTile;
    public Tile coastTile;
    public Tile snowTile;
    public Tile lakeTile;
    [Header("Hills Tiles")]
    public Tile prairieHillsTile;
    public Tile grassHillsTile;
    public Tile tundraHillsTile;
    public Tile desertHillsTile;
    public Tile snowHillsTile;
    [Header("Features")] 
    public Tile woodsTile;
    public Tile floodplainsTile;
    public Tile marshTile;
    public Tile rainforestTile;
    public Tile oasisTile;
    [Header("Terrain")] 
    public Tile mountain;
    [Header("Rivers")] 
    public GameObject riversParent;
    public GameObject riverSegment;
    [Header("Settlements")] 
    public Tile village;
    [Header("Units")] 
    public Tile warrior;
    [Header("UI Prefabs")] 
    public GameObject settlementUI;

    // Instance Attributes
    private bool _needsDirection;

    private Dictionary<GameTile, GameObject> settlementUIs = new Dictionary<GameTile, GameObject>();
    
    // Camera Constants 
    private const float dragSpeed = 10f;
    private const float zoomSpeed = 2f;
    private const float minZoom = 2f;
    private const float maxZoom = 15f;
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
        
        // Place some Settlements down (for testing)
        Test();
        
        // Situate the World Canvas for UI (based on tilemap size)
        SetUpWorldCanvas();
        
        // Render the game (once)
        RenderGame();

        // Player needs to take action -> True
        _needsDirection = true;
    }
    
    // Update is called every frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            saveCanvas.SetActive(false);
        }
        
        CameraControl();
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
    }

    /* Resizes World Canvas to match Tilemap size (should work with different world sizes or grid/cell sizes) */
    private void SetUpWorldCanvas()
    {
        // Set exact dimensions based on editor observations
        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        canvasRect.transform.position = new Vector3(worldCanvas.transform.position.x / 2, worldCanvas.transform.position.y / 2, worldCanvas.transform.position.z);
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
    private void RenderGame()
    {
        World gameWorld = gm.game.world;
        RenderTilemaps(gameWorld);
        RenderSettlementUI(gameWorld);

        /* Renders all Tilemaps */
        void RenderTilemaps(World world)
        {
            // Grid Dimensions
            double tileHeight = 0.95f;
            double tileWidth = 1f;

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
                        featureTilemap.SetTile(new Vector3Int(y, x, 0), woodsTile);
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
        
        /* Renders Settlement UI above Settlement Tiles */
        void RenderSettlementUI(World world)
        {
            for (int x = 0; x < world.GetLength(); x++)
            {
                for (int y = 0; y < world.GetHeight(); y++)
                {
                    GameTile currTile = world.GetTile(x, y);

                    if (currTile.GetSettlement() is not null)
                    {
                        // If we 
                        if (!settlementUIs.ContainsKey(currTile))
                        {
                            // Instantiate UI Prefab
                            GameObject uiInstance = Instantiate(settlementUI, worldCanvas.transform);
                            
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
                            // Update that Tile's yields.
                            UpdateUIFields(settlementUIs[currTile], currTile.GetSettlement());
                        }
                    }
                }
            }
            
            // Helper method to update UI fields for a given settlement
            void UpdateUIFields(GameObject uiObject, Settlement settlement)
            {
                // Access TMP_Text Objs
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
        }
    }

    public void ToggleSettlementWindow()
    {
        // Instantiate Settlement Window Canvas
        GameObject settlementWindow = Instantiate(settlementWindowCanvas);
        
        
        
        
    }

    // Places some settlements down for testing
    private void Test()
    {
        List<Point> spawnPoints = gm.game.world.GetSpawnPoints();
        
        foreach (Point start in spawnPoints)
        {
            // Put a Settlement at each start point
            GameTile currTile = gm.game.world.GetTile(start);
            
            Settlement settlement = new Settlement("Jersey", gm.game.civilizations[0], currTile);
            
            currTile.SetSettlement(settlement);
        }
    }    

    // Load Main Menu Scene
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /* Calls all the other Camera methods. */
    void CameraControl()
    {
        DragCamera();
        HandleZoom();
    }

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
