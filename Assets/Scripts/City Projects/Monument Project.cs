using Unity.VisualScripting;
using UnityEngine;

namespace City_Projects
{
    public class MonumentProject : CityProject
    {
        public MonumentProject()
        {
            projectName = "Monument";
            currentProductionProgress = 0;
            projectCost = 20;
            projectType = "building";
        }

        public override void Complete()
        { 
            int[] buildingYields = { 0, 0, 0, 2 , 0};
            Building monument = new Building("Monument", buildingYields);
            gameManager.AddBuilding(settlement, monument);
            currentProductionProgress = 0;
            alreadyBuilt = true;
            settlement._currentCityProject = null;
        }
    }
}
