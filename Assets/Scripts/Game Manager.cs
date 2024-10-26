using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using File = UnityEngine.Windows.File;

public class GameManager : MonoBehaviour, IDataService
{ 
    [HideInInspector] 
    public Game game;
    public void EndTurn()
   {
       game.gameTurn++;

       if (game.civilizations != null)
       {
           foreach (Civilization civilization in game.civilizations)
           {
               civilization.OnTurnEnded();
           }
       }
   }
    
    public void NewGame(uint worldSeed)
    {
        game = new Game();
        game.world = new WorldGenerator().GenerateWorld(100,50,2,worldSeed);
        SceneManager.LoadScene(1);
    }
    
    public void SaveData<T>(string relativePath, T data, bool encrypted)
   {
       // File Path
       string path = Application.persistentDataPath + relativePath;

       // If it exists, delete it (in order to replace it)
       if (File.Exists(path))
       {
           Debug.Log("Data exists. Deleting old file and writing a new one.");
           File.Delete(path);
       } 
       
       //Serialize the data to JSON
       string jsonData = JsonConvert.SerializeObject(data);
       
       // Encode string to bytes[]
       byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
       
       //Write File
       File.WriteAllBytes(path, jsonBytes);
   }
    public T LoadData<T>(string relativePath, bool encrypted)
   {
       string path = Application.persistentDataPath + relativePath;
       
       byte[] jsonBytes = File.ReadAllBytes(path);
       
       string jsonData = System.Text.Encoding.UTF8.GetString(jsonBytes);
       
       return JsonConvert.DeserializeObject<T>(jsonData);
   }

    
}
