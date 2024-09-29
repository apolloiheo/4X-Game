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
        int worldLength = world.GetLength();
        int worldHeight = world.GetHeight();
        
        if (world.GetContinents() == 1)
        {
            world.SetContinentPoint1(new Point(worldLength / 2, worldHeight / 2));
            
        } else if (world.GetContinents() == 2)
        {
            world.SetContinentPoint1(new Point(worldLength / 4, worldHeight / 2));
            world.SetContinentPoint2(new Point((worldHeight / 4) * 3 , worldHeight / 2));
        }
    }

    /* Determine the area of Continents proportional to world size.
        - Put basic Plains tiles around the ContinentStartPoints
        - Randomly expand tiles around it to form continent. 
        - Stop when it reaches a % of world coverage.
     */
    private void DetermineLand(World world)
    {
        int totalWorldSize = world.GetLength() * world.GetHeight();
        int desiredWorldCoverage = totalWorldSize / 3; // WIP - How much relative space our continent should take relative to world size.
        int currentWorldCoverage = 1; // How much relative space our continent is taking relative to world size.

        Point continentStart1 = world.GetContinentPoint1();
        Point continentStart2 = world.GetContinentPoint2();
        Point continentStart3 = world.GetContinentPoint3();
        
        switch (world.GetContinents())
        {
            case 1: // One Continent
                break;
            case 2: // Two Continents
                
                
                world.ModifyTileBiome(continentStart1, 1);
                world.ModifyTileBiome(continentStart2, 1);
                
                while (currentWorldCoverage < desiredWorldCoverage)
                {
                    // Initial Convert neighbor's to Plains.
                    foreach (Tile t in world.GetTile(continentStart1).GetNeighbors())
                    {
                        Point neighborLocation = new Point(t.GetXPos(), t.GetYPos());
                        world.ModifyTileBiome(neighborLocation, 1);
                        currentWorldCoverage++;
                    }
                    
                    
                }
                
                break;
            case 3: // Three Continents
                break;
        }
        
        
        
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
