using UnityEngine;

namespace City_Projects
{
    public class MonumentProject : CityProject
    {
        public MonumentProject()
        {
            projectName = "Monument";
            projectCost = 20;
            projectType = "building";
        }

        new public void Complete()
        {
            // Spawn Monument
        }
    }
}
