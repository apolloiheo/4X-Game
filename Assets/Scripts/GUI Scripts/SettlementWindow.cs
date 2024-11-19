using System;
using System.Collections.Generic;
using City_Projects;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SettlementWindow : MonoBehaviour
{
    public GameObject settlementWindow;
    public Canvas _worldCanvas;
    public RectTransform unitsContent;
    public RectTransform buildingsContent;
    public Director director;
    public TMP_Text settlementName;
    public GameObject citizenUI;
    public Settlement _settlement;
    public Tilemap tilemap;
    public GameObject cityProjectButton;
    
    private HashSet<GameObject> citizenUIs = new HashSet<GameObject>();

    private Dictionary<CityProject, GameObject> projects = new Dictionary<CityProject, GameObject>();

    public void Start()
    {
        director = FindObjectOfType<Director>();
        citizenUIs = new HashSet<GameObject>();
        FillProjectTabs();
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestoryWindow();
            DestroyCitizenUIs();
        }
    }

    public void RenderCitizenUIs()
    {
        // Initialize
        citizenUIs = new HashSet<GameObject>();

        foreach (GameTile tile in _settlement._territory)
        {
            // Get a Vector 3 Position of that tile
            Vector3Int tilePosition = new Vector3Int(tile.GetYPos(), tile.GetXPos(), 0);
            Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
            
            // Instance citizen Obj and the UI prefab
            GameObject citizen = Instantiate(citizenUI, _worldCanvas.transform);
            
            // Position it in world canvas
            RectTransform rectTransform = citizen.GetComponentInChildren<RectTransform>();
            rectTransform.position = worldPosition;
            
            // Set Citizen scripts references
            citizen.GetComponent<Citizen>().settlementWindow = this;
            citizen.GetComponent<Citizen>().settlement = _settlement;
            citizen.GetComponent<Citizen>().gameTile = tile;
            
            // Keep track of it in HashSet
            citizenUIs.Add(citizen);
        }
    }

    /* Renders buttons on top of Citizen tiles. */
    public void ToggleManageCitizens()
    {
        if (citizenUIs.Count <= 0)
        {
            RenderCitizenUIs();
        }
        else
        {
            DestroyCitizenUIs();
            _settlement.AutoAssignWorkedTiles();
        }
    }

    private void DestroyCitizenUIs()
    {
        foreach (GameObject ui in citizenUIs)
        {
            Destroy(ui);
        }

        citizenUIs = new HashSet<GameObject>();
    }
    
    private void DestoryWindow()
    {
        settlementWindow.transform.SetParent(null);
        director._zoomedIn = false;
        director.RestoreCameraState();
        Destroy(settlementWindow);
    }

    public void FillProjectTabs()
    {
        // Fill Project Tabs with the Settlement's added projects
        foreach (CityProject project in _settlement.GetProjects())
        {
            // Initialize variable
            GameObject projectPrefab = null;
            
            // Place Units in the Units content
            if (project.projectType == "unit")
            {
                projectPrefab = Instantiate(cityProjectButton, unitsContent);
            }
            // Place Buildings in the Buildings content
            else if (project.projectType == "building")
            {
                projectPrefab = Instantiate(cityProjectButton, buildingsContent);
            }
                
            projectPrefab.GetComponent<ProjectButton>().name.text = project.projectName;
            projectPrefab.GetComponent<ProjectButton>().turns.text = Math.Ceiling((double)((project.projectCost - project.currentProductionProgress) / _settlement.GetYieldsPt()[1])).ToString();
            projectPrefab.GetComponent<ProjectButton>().settlement = _settlement;
            projectPrefab.GetComponent<ProjectButton>().project = project;

            projects.Add(project, projectPrefab);
        }
    }

    /* Goes through each Project Prefab button and updates its cost. (Useful when Settlement changes yields) */
    public void UpdateProjectTabs()
    {
        foreach (CityProject project in projects.Keys)
        {
            GameObject projectPrefab = projects[project];
            
            projectPrefab.GetComponent<ProjectButton>().turns.text = Math.Ceiling((double)((project.projectCost - project.currentProductionProgress) / _settlement.GetYieldsPt()[1])).ToString();
        }
    }
}
