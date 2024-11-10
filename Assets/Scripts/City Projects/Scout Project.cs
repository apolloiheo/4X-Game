namespace City_Projects
{
    public class ScoutProject : CityProject
    {
        public ScoutProject()
        {
            projectName = "Scout";
            projectCost = 16;
            projectType = "unit";
        }

        new public void Complete()
        {
            // Spawn Scout
        }
    }
}
