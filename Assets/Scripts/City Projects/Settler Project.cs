using Units;

namespace City_Projects
{
    public class SettlerProject : CityProject
    {
        public SettlerProject()
        {
            projectName = "Settler";
            projectCost = 30;
            projectType = "unit";
        }

        public override void Complete()
        {
            // Spawn Settler
            gameManager.SpawnUnit(new Settler(settlement._gameTile, settlement._civilization));
            currentProductionProgress = 0;
            settlement._currentCityProject = null;
        }
    }
}
