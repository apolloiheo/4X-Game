using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class World : MonoBehaviour
{
    // Instance Attributes
    private int _length;
    private int _height;
    private Tile[,] _world; // 2D Array of Tiles
    private int _continents; // # of Continents in our world. Could be 1, 2 or 3.
    
    // Constants
    private const int DefaultBiomeFill = 7; // 1.Plains, 2.Grassland, 3.Tundra, 4.Desert, 5.Snow, 6.Coast, 7.Ocean
    
    // Class Methods
    
    /* World Constructor */
    public World(int length, int height, int continents)
    {
        _length = length;  
        _height = height;
        _continents = continents;
        _world = new Tile[length, height];
    }
    
    /* Fill an empty world with a starting Tile. */
    public void FillEmptyWorld(int biome)
    {
        for (int x = 0; x < _length; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Instantiate default/background tile to fill world with
                Tile tile = new Tile(DefaultBiomeFill, 0, 0, 0);
                
                // Record its X & Y position within its instance attributes
                tile.SetXPos(x);
                tile.SetYPos(y);
                
                // Store it in 2D array.
                _world[x, y] = tile;
            }
        }
    }

    /* Sets Tile Adjacency (neighbors) based on a Flat Top/Bottom hexagon grid with an even-q orientation (0,0 must be at bottom left of grid). */
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

    /* Takes a Tile's X & Y Position on a 2D grid and prints a list of its neighbors' X & Y.  */
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
}
