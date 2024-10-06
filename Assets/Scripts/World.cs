using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class World
{
    // Instance Attributes
    private int _length;
    private int _height;
    private GameTile[,] _world; // 2D Array of Tiles
    
    // Constants
    private const int DefaultBiomeFill = 7; // 1.Plains, 2.Grassland, 3.Tundra, 4.Desert, 5.Snow, 6.Coast, 7.Ocean
    
    // Class Methods
    
    /* World Constructor */
    public World(int length, int height)
    {
        _length = length;  
        _height = height;
        _world = new GameTile[length, height];
    }
    
    /* Fill an empty world with a starting Tile. */
    public void FillEmptyWorld(int biome)
    {
        for (int x = 0; x < _length; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Instantiate default/background tile to fill world with
                GameTile gameTile = new GameTile(DefaultBiomeFill, 0, 0, 0);
                
                // Record its X & Y position within its instance attributes
                gameTile.SetXPos(x);
                gameTile.SetYPos(y);
                
                // Store it in 2D array.
                _world[x, y] = gameTile;
            }
        }
    }

    /* Sets Tile Adjacency (neighbors) based on a Flat Top/Bottom hexagon  with an even-q orientation. */
    public void SetTileAdjacency()
    {
        for (int x = 0; x < _length; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Edge 0
                if (y < _height - 1)
                {
                    _world[x, y].SetNeighbor(0, _world[x, y + 1]);
                    _world[x, y + 1].SetNeighbor(3, _world[x, y]);
                }
                
                // Edge 3
                if (y > 0)
                {
                    _world[x,y].SetNeighbor(3, _world[x , y - 1]);
                    _world[x, y - 1].SetNeighbor(0, _world[x, y]);
                }
                
                // Every Odd X Tile
                if (x % 2 != 0)
                {
                    // Edge 1 
                    if (x < _length - 1 && y < _height - 1)
                    {
                        _world[x, y].SetNeighbor(1, _world[x + 1, y + 1]);
                        _world[x + 1, y + 1].SetNeighbor(4, _world[x, y]);
                    }

                    // Edge 2 
                    if (x < _length - 1)
                    {
                        _world[x, y].SetNeighbor(2, _world[x + 1, y]);
                        _world[x + 1, y].SetNeighbor(5, _world[x, y]);
                    }

                    // Edge 4
                    if (x > 0)
                    {
                        _world[x,  y].SetNeighbor(4, _world[x - 1, y]);
                        _world[x - 1, y].SetNeighbor(1, _world[x, y]);
                    }

                    // Edge 5
                    if (x > 0 && y < _height - 1)
                    {
                        _world[x, y].SetNeighbor(5, _world[x - 1, y + 1]);
                        _world[x - 1, y + 1].SetNeighbor(2, _world[x, y]);
                    }
                } 
            }
        }
    }

    /* Print the world to console. (Bad way to test but will do for now) */
    public void PrintWorld()
    {
        string worldString = "";
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _length; x++)
            {
                worldString += _world[x,y].GetBiome().ToString() + " ";
            }
            worldString += "\n";
        }
        Debug.Log(worldString);
    }

    public void TestTileAdjacency(int xPos, int yPos)
    {
        string output = "Tile ("+xPos+","+yPos+") is adjacent to: " + "\n";

        for (int x = 0; x < _world[xPos, yPos].GetNeighbors().Length; x++)
        {
            if (_world[xPos, yPos].GetNeighbors()[x] is not null)
            {
                output += "Edge: " + x + " " + "(" + _world[xPos, yPos].GetNeighbors()[x].GetXPos() + 
                          "," + _world[xPos, yPos].GetNeighbors()[x].GetYPos() + ")";
                
                output += "\n";
            }
        }
        Debug.Log(output);
    }
    // Sets a random amount of world tiles to be mountains, then tests patthfinding on it.
    // Destructively modifies the world.
    public void TestPathfinder(int x1, int y1, int x2, int y2, double pOfMountain)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _length; x++)
            {
                if (Random.Range(0, 10) > pOfMountain && x!= x1 && y!= y1 && x != x2 && y != y2 )
                {
                    _world[x,y].SetTerrain(2);
                }
            }
        }

        List<Tuple<GameTile, int>> path = Pathfinder.AStarWithLimit(_world[x1, y1], _world[x2, y2], 250);
        foreach (var tuple in path)
        {
            Debug.Log(tuple.Item1);
        }
    }
    // Creates a unit in a position(or uses a unit already in the position) and attempts to move it to the target location.
    public void TestUnitMovement(int startX, int startY, int endX, int endY, int movementPoints=2)
    {
        GameTile startTile = _world[startX, startY];
        GameTile endTile = _world[endX, endY];
        if (startTile.GetUnit() is not null)
        {
            startTile.GetUnit().Move(endTile);
        }
        else
        {
            startTile.SetUnit(new Unit("Recruit", movementPoints, 0, 0, 0 ));
            startTile.GetUnit().Move(endTile);
        }
    }
}

