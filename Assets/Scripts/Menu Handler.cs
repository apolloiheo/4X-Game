using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Menu Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject newGameCanvas;
    public GameObject loadGameCanvas;
    
    [Header("Prefabs")]
    public GameObject saveButtonPrefab;
    public Transform saveListContainer;
    
    [Header("World Seed Input")]
    public TMP_InputField seedInput;
    
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameManager);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }
    
    // Opens Load Game Menu 
    public void OpenLoadGameMenu()
    {
        mainMenuCanvas.SetActive(false);
        newGameCanvas.SetActive(false);
        loadGameCanvas.SetActive(true);
        
        void PopulateSaveList()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.save");

            foreach (string filePath in files)
            {
                GameObject saveButton = Instantiate(saveButtonPrefab, saveListContainer);
                string filename = Path.GetFileNameWithoutExtension(filePath);
                saveButton.GetComponentInChildren<Text>().text = filename;
            
                saveButton.GetComponent<Button>().onClick.AddListener(() => LoadSelectedSave(filePath));
            }
        }
        
        void LoadSelectedSave(string filePath)
        {
        
        }
    }

    // Opens New Game Menu
    public void OpenNewGameMenu()
    {
        mainMenuCanvas.SetActive(false);
        loadGameCanvas.SetActive(false);
        newGameCanvas.SetActive(true);
    }

    // Closes any Menu and returns to Main Menu
    public void BackToMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        newGameCanvas.SetActive(false);
        loadGameCanvas.SetActive(false);
    }

    public void StartNewGame()
    {
        uint seed = Convert.ToUInt32(seedInput.text);
        gameManager.NewGame(seed);
    }
}
