using Units;
using UnityEngine.UIElements;

namespace City_Projects
{
    public class WarriorProject : CityProject
    {
        public WarriorProject()
        {
            projectName = "Warrior";
            currentProductionProgress = 0;
            projectCost = 20;
            projectType = "unit";
        }

        public override void Complete()
        {
            // Spawn Warrior
            gameManager.SpawnUnit(new Warrior(settlement._gameTile, settlement._civilization));
            
            currentProductionProgress = 0;
        }
    
    
    }
}
