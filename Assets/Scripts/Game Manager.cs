using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Units;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    [HideInInspector] 
    public Game game;

    private bool savedGame = false;
    public void EndTurn()
   {
       game.gameTurn++;

       if (game.civilizations is not null)
       {
           foreach (Civilization civilization in game.civilizations)
           {
               civilization.OnTurnEnded();
               Debug.Log("Scanned Civilization.");
           }
       }
       
       Debug.Log("Next Turn.");
   }
    
    public void NewDemoGame(uint worldSeed)
    {
        game = new Game();
        game.singlePlayer = true;
        Civilization player1 = new Civilization(new Color32(255, 181, 43, 255));
        Civilization player2 = new Civilization(new Color32(255, 95, 92, 255));
        Civilization player3 = new Civilization(new Color32(103, 92, 255, 255));
        Civilization player4 = new Civilization(new Color32(0, 150, 13, 255));
        player1.IsNPC = false;
        game.civilizations = new List<Civilization>();
        game.civilizations.Add(player1);
        game.civilizations.Add(player2);
        game.civilizations.Add(player3);
        game.civilizations.Add(player4);
        game.world = new WorldGenerator().GenerateWorld(100, 50, 2, worldSeed, game.civilizations.Count);
        Test();
        SceneManager.LoadScene(1);
    }
    
    // Places some settlements down for testing
    private void Test()
    {
        List<Point> spawnPoints = game.world.GetSpawnPoints();
        
        foreach (Point start in spawnPoints)
        {
            // Put a Settlement at each start point
            GameTile currTile = game.world.GetTile(start);
            
            Settlement settlement = new Settlement("Jersey", game.civilizations[0], currTile);
            
            game.civilizations[0].AddSettlement(settlement);
            
            currTile.SetSettlement(settlement);

            foreach (GameTile tile in currTile.GetNeighbors())
            {
                if (tile.IsWalkable())
                {
                    SpawnUnit(new Warrior(tile, game.civilizations[0]));
                    break;
                }
            }
        }
    }    
    
    public void SaveGame(string filename) 
    {
        if (!savedGame)
        {
            game.StageForSerialization();
            if (filename == "")
            {
                filename = "saveFile";
            }
            string fileRelativePath = filename + ".json";
            SaveData(fileRelativePath, game, false);
            savedGame = true;
        }
        
        void SaveData<T>(string relativePath, T data, bool encrypted)
        {
            // File Path
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            // If it exists, delete it (in order to replace it)
            if (File.Exists(path))
            {
                Debug.Log("Data exists. Deleting old file and writing a new one.");
                File.Delete(path);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                Converters = new List<JsonConverter>
                {
                    new GameTileArrayConverter()
                },
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = false
                }
            };
       
            //Serialize the data to JSON
            string jsonData = JsonConvert.SerializeObject(data, settings);
       
            // Encode string to bytes[]
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
       
            //Write File
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                fileStream.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }
        Debug.Log("Game successfully saved as: " + filename);
    }

    public void LoadGame(string filePath)
    {
        string path = Path.Combine(Application.persistentDataPath, filePath);

        if (File.Exists(path))
        {
            game = LoadData<Game>(path, false);
            game.RestoreAfterDeserialization(game);
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("File not found.");
        }
        
        T LoadData<T>(string relativePath, bool encrypted)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            try
            {
                byte[] jsonBytes = File.ReadAllBytes(path);

                string jsonData = System.Text.Encoding.UTF8.GetString(jsonBytes);
                
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = new List<JsonConverter>
                    {
                        new GameTileArrayConverter()
                    },
                    ContractResolver = new DefaultContractResolver
                    {
                        IgnoreSerializableAttribute = false
                    }
                };

                return JsonConvert.DeserializeObject<T>(jsonData, settings);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load data: " + e.Message);
                return default;
            }
        }
    }

    public void MoveUnit(Unit unit, Point target)
    {
        // Shortest path from Unit's position to target point
        List<GameTile> path = Pathfinder.FindShortestPath(unit._gameTile, game.world.GetTile(target));
        
        // Movement Cost of moving across that Path.
        int moveCost = Pathfinder.CalculatePathCost(path);
        
        // Subtract Movement Points from Unit
        unit._currMP -= moveCost;
        
        // Store Game Tile references
        GameTile previousTile = unit._gameTile;
        GameTile newTile = game.world.GetTile(target);
        
        // Update Tiles
        previousTile.SetUnit(null);
        newTile.SetUnit(unit);
        
        // Update Unit's own Tile
        unit._gameTile = newTile;
        
        // Restart get possible moves
        unit.GetPossibleMoves(unit._gameTile, unit._currMP, true);
    }

    private void SpawnUnit(Unit unit)
    {
        // Set it on the Tile
        GameTile tile = unit._gameTile;
        tile.SetUnit(unit);
        // Add into it's Civilization
        Civilization owner = unit._civilization;
        owner._units.Add(unit);
    }
}
