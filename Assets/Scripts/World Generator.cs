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
        _random = new Random(43333333);
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
                // Determine the random X & Y starting points of continents
                int continentStartXWest = random.NextInt((world.GetLength() / 10) * 2, (world.GetLength() / 10) * 3);
                int continentStartYWest = random.NextInt((world.GetHeight() / 10) * 4, (world.GetHeight() / 10) * 6);
                int continentStartXEast = random.NextInt((world.GetLength() / 10) * 7, (world.GetLength() / 10) * 8);
                int continentStartYEast = random.NextInt((world.GetHeight() / 10) * 4, (world.GetHeight() / 10) * 6);

                // Store those X & Y in a ContinentStart Point for each Continent
                Point continentStart1 = new Point(continentStartXWest, continentStartYWest);
                Point continentStart2 = new Point(continentStartXEast, continentStartYEast);

                int totalWorldSize = world.GetLength() * world.GetHeight();
                int desiredWorldCoverage = (totalWorldSize / 10) * 4; // WIP - How much relative space our continent should take relative to world size.
                int currentWorldCoverage = 1; // How much relative space our continent is taking relative to world size.
                double percentageOfWorldCoverage = currentWorldCoverage / totalWorldSize;
                int probabilityThreshold = 20; // Base percentage of likelihood to NOT place Tile.

                // Make a queue of the Tiles from which to spread to neighbors (continentStart1 and continentStart2 are first)
                Queue<Point> queue = new Queue<Point>();

                // Add all the neighbors of each continents start to the queue
                foreach (Tile t in world.GetTile(continentStart1).GetNeighbors())
                {
                    queue.Enqueue(new Point(t.GetXPos(), t.GetYPos()));
                }
                
                // Turn those two points to plains (or to a market of where the continent started)
                world.ModifyTileBiome(continentStart1, 1);
                world.ModifyTileBiome(continentStart2, 1);
                double continentSwitch = random.NextDouble(0.40, 0.60);
                bool continentSwitched = false;
                
                // Stop when our continents have reached the desiredLandCoverage
                while (currentWorldCoverage < desiredWorldCoverage)
                {
                    // At some random point, clear the queue and work on the other continent
                    if (currentWorldCoverage >= desiredWorldCoverage * continentSwitch && !continentSwitched)
                    {
                        continentSwitched = true;
                        queue.Clear();
                        
                        // Add all the neighbors of each continents start to the queue
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
                        Tile currentNeighbor = possibleNeighbors[nextNeighborIndex];
                        // Store its location
                        Point neighborLocation = new Point(currentNeighbor.GetXPos(), currentNeighbor.GetYPos());
                        // Probability factor
                        int probability = random.NextInt(0, 100); // A random number between 0 - 100
                        int divisionFactor = 2;
                        
                        int heightFactor; // Increases the closer currentNeighbor is to 0 or worldHeight
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

                        int distanceToCenterFactor; // Increases the closer currentNeighbor is to worldLength / 2
                        
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
                        
                        int distanceToEdgeFactor; // Increase the closer you are to 0 or world.Length
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

                        // Random chance the Tile won't be changed probability must be higher than our threshold.
                        if (probability > probabilityThreshold + heightFactor + distanceToCenterFactor + distanceToEdgeFactor)
                        {
                            // Add the neighbor to our Point Queue
                            queue.Enqueue(neighborLocation);
                            
                            // Modify the Tile's Biome (0 for now for visibility)
                            world.ModifyTileBiome(neighborLocation, 0);
                            
                            // Updates World Coverage
                            currentWorldCoverage++;
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
