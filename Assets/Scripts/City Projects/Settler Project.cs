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

        new public void Complete()
        {
            // Spawn Settler
            settlement._currentCityProject = null;
        }
    }
}
