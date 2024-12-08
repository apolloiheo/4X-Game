using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;



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
            // Raise completion event

        }

        public string GetName()
        {
            return projectName;
        }
    }
}
