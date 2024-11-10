using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace City_Projects
{
    [System.Serializable]
    public abstract class CityProject
    {
        [JsonProperty] public string projectName;
        [JsonProperty] public int projectCost;
        [JsonProperty] public int currentProductionProgress = 0;
        [JsonProperty] public string projectType;

        public void AddToProgress(int production)
        {
            currentProductionProgress += production;
        
            if (currentProductionProgress >= projectCost)
            {
                Complete();
            }
        }

        public void Complete()
        {
            // Abstract Class cannot be instantiated
        }

        public string GetName()
        {
            return projectName;
        }

    }
}
