using Units;

namespace City_Projects
{
    public class ScoutProject : CityProject
    {
        public ScoutProject()
        {
            projectName = "Scout";
            currentProductionProgress = 0;
            projectCost = 16;
            projectType = "unit";
        }

        public override void Complete()
        {
            // Spawn Scout
            gameManager.SpawnUnit(new Scout(settlement._gameTile, settlement._civilization));
            currentProductionProgress = 0;
            settlement._currentCityProject = null;
        }
    }
}
