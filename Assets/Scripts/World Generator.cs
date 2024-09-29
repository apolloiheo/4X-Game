using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class WorldGenerator : MonoBehaviour
{
    private Random _random; // Random determintic factor
    
    /* Returns a fully generated game world. */
    public World GenerateWorld(int length, int height, int continents)
    {
        World world = new World(length, height, continents);
        
        DetermineContinents(world);
        DetermineLand(world);
        DetermineBiomes(world);
        DetermineTerrain(world);
        DetermineRivers(world);
        DetermineFeatures(world);
        DetermineResources(world);
        
        return world;
    }
    
    /* Determine Continent Numbers */
    private void DetermineContinents(World world)
    {
        
    }

    /* Determine the area of Continents proportional to world size. */
    private void DetermineLand(World world)
    {
        
    }

    /* Determine Biomes on landmasses and on Coasts */
    private void DetermineBiomes(World world)
    {
        
    }

    /* Determine Hills and Mountains  */
    private void DetermineTerrain(World world)
    {
        
    }
    
    /* Determine Rivers - From Mountains to Coast  */ 
    private void DetermineRivers(World world)
    {
        
    }
    
    /* Determine the features on Tiles. */
    private void DetermineFeatures(World world)
    {
        
    }

    /* Determine the Resources and Resource spread across the game world.  */
    private void DetermineResources(World world)
    {
        
    }
}
