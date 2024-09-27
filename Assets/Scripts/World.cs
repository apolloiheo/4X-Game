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
                // Instantiate default tile to fill world with
                Tile tile = new Tile(DefaultBiomeFill, 0, 0, 0);
                
                // Record its position within its instance properties
                tile.SetXPos(x);
                tile.SetYPos(y);
                
                // Store it in 2D array.
                _world[x, y] = tile;
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
}
