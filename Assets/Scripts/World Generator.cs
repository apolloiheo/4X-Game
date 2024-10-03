using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = Unity.Mathematics.Random;

public class WorldGenerator : MonoBehaviour
{
    private Random _random; // Random determintic factor
    private int _continents;

    public WorldGenerator()
    {
        _random = new Random();
    }
    
    /* Returns a fully generated game world. */
    public World GenerateWorld(int length, int height, int continents)
    {
        _random.InitState();
        _random = new Random(4545454);
        _continents = continents;
        World world = new World(length, height);
        world.FillEmptyWorld(7);
        world.SetTileAdjacency();
        DetermineContinents(world);
        DetermineLand(world,_random);
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
    }

    /* Determine the area of Continents proportional to world size.
        - Put basic Plains tiles around the ContinentStartPoints
        - Randomly expand tiles around it to form continent. 
        - Stop when it reaches a % of world coverage.
     */
    private void DetermineLand(World world, Random random)
    {
        // Different Procedures given different numbers of continents
        switch (_continents)
        {
            case 1: // One Continent
                break;
            case 2: // Two Continents
                // Determine the random X & Y starting points of 2 continents
                int continentStartXWest = random.NextInt((world.GetLength() / 10) * 2, (world.GetLength() / 10) * 3);
                int continentStartYWest = random.NextInt((world.GetHeight() / 10) * 4, (world.GetHeight() / 10) * 6);
                int continentStartXEast = random.NextInt((world.GetLength() / 10) * 7, (world.GetLength() / 10) * 8);
                int continentStartYEast = random.NextInt((world.GetHeight() / 10) * 4, (world.GetHeight() / 10) * 6);

                // Store those X & Y in a ContinentStart Point for each Continent
                Point continentStart1 = new Point(continentStartXWest, continentStartYWest);
                Point continentStart2 = new Point(continentStartXEast, continentStartYEast);

                // Store some important factors
                int totalWorldSize = world.GetLength() * world.GetHeight();
                double desiredWorldCoverage = totalWorldSize * random.NextDouble(.45,.55); // Some random percentage of world size between 45-55%
                int currentWorldCoverage = 2; // How many Tiles have been turned to land so far.
                double percentageOfWorldCoverage = currentWorldCoverage / totalWorldSize; // ^ as a percentage
                int probabilityThreshold = 30; // Base percentage of likelihood to NOT place Tile. (is increased by many factors)
                int consecutiveFailures = 0; // Keeps track of how many times the procedure has failed to place a Tile. (Makes it more likely to succeed if it failed a lot)
                int failureFactor = 10; // The probability factor power of each consecutive failure.

                // Instantiate a queue of Points (to reference the points of Tiles)
                Queue<Point> queue = new Queue<Point>();

                // Add all neighbors of the first continent to the queue
                foreach (Tile t in world.GetTile(continentStart1).GetNeighbors())
                {
                    queue.Enqueue(new Point(t.GetXPos(), t.GetYPos()));
                }
                
                // Turn both continent starting points to land.
                world.ModifyTileBiome(continentStart1, 1);
                world.ModifyTileBiome(continentStart2, 1);
                
                // The percentage of land coverage that the first continent will take before switching to building the second.
                double continentSwitch = random.NextDouble(0.40, 0.60);
                // Tells the while loop when the first continent is done.
                bool continentSwitched = false;
                
                // Stop when both continents have reached the desired LandCoverage
                while (currentWorldCoverage < desiredWorldCoverage)
                {
                    // If current coverage has reached the point to switch to the other continent
                    if (currentWorldCoverage >= desiredWorldCoverage * continentSwitch && !continentSwitched)
                    {
                        // Set to true so this does not repeat
                        continentSwitched = true;
                        // Clear the previous continent's queue
                        queue.Clear();
                        
                        // Add all the neighbors of continent #2 to the queue.
                        foreach (Tile t in world.GetTile(continentStart2).GetNeighbors())
                        {
                            queue.Enqueue(new Point(t.GetXPos(), t.GetYPos()));
                        }
                    }

                    // Deque the first Tile
                    Tile currentTile = world.GetTile(queue.Dequeue());
                    // Instantiate an empty List of possible neighbors
                    List<Tile> possibleNeighbors = new List<Tile>();

                    // If its neighbors are not null and are Ocean, store them in possibleNeighbors.
                    foreach (Tile t in currentTile.GetNeighbors())
                    {
                        if (t is not null && t.GetBiome() == 7)
                        {
                            possibleNeighbors.Add(t);
                        }
                    }

                    while (possibleNeighbors.Count > 0)
                    {
                        // Randomly choose the next Neighbor Tile to expand to and set it to currentNeighbor
                        int nextNeighborIndex = random.NextInt(0, possibleNeighbors.Count - 1);
                        // Reference to the current neighbor
                        Tile currentNeighbor = possibleNeighbors[nextNeighborIndex];
                        // Store its location
                        Point neighborLocation = new Point(currentNeighbor.GetXPos(), currentNeighbor.GetYPos());
                        
                        // Probability - a random number from 1 to 100
                        int probability = random.NextInt(0, 100); 
                        
                        // Set this to 1, at the extremes of the map to make it way more likely to stop tiles from spreading. 
                        int divisionFactor = 2;
                        
                        // Increases the closer currentNeighbor's Y is to 0 or worldHeight
                        int heightFactor; 
                        
                        if (currentNeighbor.GetYPos() < world.GetHeight() / 2)
                        {
                            if (currentNeighbor.GetYPos() < (world.GetHeight() / 10) * 1)
                            {
                                divisionFactor = 1;
                            }
                            heightFactor = (world.GetHeight() / 2) - currentNeighbor.GetYPos() / divisionFactor;
                        }
                        else
                        {
                            if (currentNeighbor.GetYPos() > (world.GetHeight() / 10) * 9)
                            {
                                divisionFactor = 1;
                            }
                            heightFactor = (currentNeighbor.GetYPos() - (world.GetHeight() / 2)) / divisionFactor;
                        }
                        
                        // Increases the closer currentNeighbor's X is to the center X of the world length.
                        int distanceToCenterFactor; 
                        
                        if (currentNeighbor.GetXPos() < world.GetLength() / 2)
                        {
                            if (currentNeighbor.GetXPos() > (world.GetLength() / 10) * 4)
                            {
                                divisionFactor = 1;
                            }
                            distanceToCenterFactor = currentNeighbor.GetXPos() / divisionFactor;
                        }
                        else
                        {
                            if (currentNeighbor.GetXPos() < (world.GetLength() / 10) * 6)
                            {
                                divisionFactor = 1;
                            }
                            distanceToCenterFactor = ((world.GetLength() - (currentNeighbor.GetXPos())) / divisionFactor);
                        }
                        // Increases the closer currentNeighbor's X is to 0 or to max world.Length
                        int distanceToEdgeFactor; 
                        if (currentNeighbor.GetXPos() < world.GetLength() / 2)
                        {
                            if (currentNeighbor.GetXPos() < (world.GetLength() / 10) * 1)
                            {
                                divisionFactor = 1;
                            }
                            distanceToEdgeFactor = ((world.GetLength() / 2) - currentNeighbor.GetXPos()) / divisionFactor;
                        }
                        else
                        {
                            if (currentNeighbor.GetXPos() > (world.GetLength() / 10) * 9)
                            {
                                divisionFactor = 1;
                            }
                            distanceToEdgeFactor = (currentNeighbor.GetXPos() - (world.GetLength() / 2)) / divisionFactor;
                        }
                        
                        // Add all the factors to the probability threshold. Roll a number from 0-100 and see if it expands the tile.
                        if (probability > probabilityThreshold + heightFactor + distanceToCenterFactor + distanceToEdgeFactor - (consecutiveFailures * failureFactor))
                        {
                            // Reset consecutive failures
                            consecutiveFailures = 0;
                            // Add the neighbor to our Point Queue
                            queue.Enqueue(neighborLocation);
                            
                            // Modify the Tile's Biome (0 for now for visibility)
                            world.ModifyTileBiome(neighborLocation, 0);
                            
                            // Updates World Coverage
                            currentWorldCoverage++;
                        }
                        else
                        {
                            // Update consecutiveFailures
                            consecutiveFailures++;
                        }
                        // Once it's been processed remove it from the possibleNeighbors list.
                        possibleNeighbors.Remove(currentNeighbor);
                    }
                }
                break;
            case 3: // Three Continents
                break;
        }
        DetermineBiomes(world);
    }

    /* Determine Biomes on landmasses and on Coasts */
    private void DetermineBiomes(World world)
    {
        // Convert Tiles at the North and South edges to snow.
        int northSnowLine = world.GetHeight() - (world.GetHeight() / 7);
        int southSnowLine = world.GetHeight() / 7;
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                if (world.GetTile(x, y).GetBiome() != 7)
                {
                    if (y <= southSnowLine || y >= northSnowLine)
                    {
                        world.GetTile(x, y).SetBiome(5);
                    }
                }
            }
        }
        
        // Convert all 0 Tiles adjacent to Snow into Tundra - 0 should later be changed to plains
        // Store all those tundra Tiles in a Queue
        Queue<Point> tundraQueue = new Queue<Point>();
        int northTundraLine = northSnowLine - (world.GetHeight() / 12);
        int southTundraLine = southSnowLine + (world.GetHeight() / 12);
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                // All Plains above/below the North/South Tundra Line should be tundra
                if (world.GetTile(x, y).GetBiome() == 0)
                {
                    if (y <= southTundraLine || y >= northTundraLine)
                    {
                        world.GetTile(x, y).SetBiome(3);
                    }
                }
                
                // Any Plains adjacent to Snow should be Tundra.
                if (world.GetTile(x, y).GetBiome() == 5)
                {
                    foreach (Tile neighbor in world.GetTile(x, y).GetNeighbors())
                    {
                        if (neighbor is not null)
                        {
                            if (neighbor.GetBiome() == 0)
                            {
                                neighbor.SetBiome(3);
                                tundraQueue.Enqueue(new Point(neighbor.GetXPos(), neighbor.GetYPos()));
                            }
                        }
                    }
                }
            }
        }
        
        /*// Add Coast Tiles
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                if (world.GetTile(x, y).GetBiome() == 7)
                {
                    foreach (Tile neighbor in world.GetTile(x, y).GetNeighbors())
                    {
                        if (neighbor is not null && neighbor.GetBiome() != 7 && neighbor.GetBiome() != 6)
                        {
                            world.GetTile(x, y).SetBiome(6);
                        }
                    }
                }
            }
        }*/
        
    }

    /* Determine Hills and Mountains  */
    private void DetermineTerrain(World world)
    {
        // To be implemented
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
