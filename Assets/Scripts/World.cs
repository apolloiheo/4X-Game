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
    
    // Constants
    private const int DefaultBiomeFill = 7; // 1.Plains, 2.Grassland, 3.Tundra, 4.Desert, 5.Snow, 6.Coast, 7.Ocean
    
    // Class Methods
    
    /* World Constructor */
    public World(int length, int height)
    {
        _length = length;  
        _height = height;
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

    /* Sets Tile Adjacency (neighbors) based on a Flat Top/Bottom hexagon  with an even-q orientation. */
    public void SetTileAdjacency()
    {
        for (int x = 0; x < _length - 1; x++)
        {
            for (int y = 0; y < _height - 1; y++)
            {
                // Top and Bottom of all Flat edges
                _world[x, y].SetNeighbor(0, _world[x, y + 1]);
                _world[x, y + 1].SetNeighbor(3, _world[x, y]);

                // Check that X index is not out of bounds
                if (x > 0 && x < _length && y > 0 && y < _height)
                {
                    // If X is Odd
                   if (x % 2 != 0)
                   {
                       // Bottom Left of Odd X / Top Right of Even X
                       _world[x, y].SetNeighbor(4, _world[x - 1, y]);
                       _world[x - 1, y].SetNeighbor(1, _world[x, y]);
                       
                       // Bottom Right of Odd X / Top Left of Even X
                       _world[x , y].SetNeighbor(2, _world[x + 1, y]);
                       _world[x + 1, y].SetNeighbor(5, _world[x, y]);
                   }
                   else // If X is Even
                   {
                       // Top Left of Even X / Bottom Right of Odd X
                       _world[x,y].SetNeighbor(5, _world[x - 1, y]);
                       _world[x - 1, y].SetNeighbor(2, _world[x, y]);
                       
                       // Top Right of Even X / Bottom Left of Odd X
                       _world[x,y].SetNeighbor(1, _world[x + 1, y]);
                       _world[x + 1, y].SetNeighbor(4, _world[x, y]);
                       
                       // Bottom Right of Even // Top Left of Odd
                       _world[x, y].SetNeighbor(2, _world[x + 1, y - 1]);
                       _world[x + 1, y - 1].SetNeighbor(5, _world[x, y]);
                       
                       // Bottom Left of Even // Top Right of Odd
                       _world[x,y].SetNeighbor(4, _world[x - 1, y - 1]);
                       _world[x - 1, y - 1].SetNeighbor(1, _world[x, y]);
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
                output += "(" + _world[xPos, yPos].GetNeighbors()[x].GetXPos() + 
                          "," + _world[xPos, yPos].GetNeighbors()[x].GetYPos() + ")";
                
                output += "\n";
            }
        }
        
        Debug.Log(output);
    }
}
