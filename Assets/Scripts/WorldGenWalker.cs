using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class WorldGenWalker : MonoBehaviour
{
    public GameTile CurrTile; //the tile that the walker is standing on
    public World world;
    public int direction; //a number from 0-5, which will pull from the list of neighbors
    public bool tooFarUp = false;
    public bool tooFarDown = false;
    public int oldBiome;
    public int newBiome;
    
    public bool DefaultIsValidNeighbor(GameTile tile)
    {
        return true;
    }

    public WorldGenWalker(World w, GameTile startTile, int _direction, int _oldBiome, int _newBiome) //initializing variables
    {
        world = w;
        CurrTile = startTile;
        direction = _direction;
        oldBiome = _oldBiome;
        newBiome = _newBiome;
    }

    /*
     * the walker will take a step
     * changes the tile it is currently standing on,
     * then move in the current direction and sets a new random number
     * as its new direction
     *
     * several fail safes in place:
     * if the currTile is null, which will happen
     * if the walker traverses off of the map,
     * it will start again from a new completely random tile
     *
     * if the direction would take the walker onto land or off the map,
     * it looks for a valid neighbor that is neither null nor land
     *(the above is done through the findNewNeighbor function)
     *
     * if all else fails it just takes a random step which will
     * sometimes take it off of the map, in which case the first failsafe
     * triggers
     *
     * returns true if it modified a tile, false if it didn't
     */
    public bool move() 
    {
        if (CurrTile == null || UnityEngine.Random.Range(0, 100) > 98) // 1% chance for walker to go rogue
        {
            int randomX = UnityEngine.Random.Range(0, world.GetLength());
            int randomY = UnityEngine.Random.Range(0, world.GetHeight());
            CurrTile = world.GetTile(new Point(randomX, randomY)); //teleports to a completely random spot
            return false;
        }
        GameTile[] neighbors = CurrTile.GetNeighbors();
        GameTile currNeighbor = neighbors[direction];

        if (currNeighbor == null || currNeighbor.GetBiome() != oldBiome)
        {
            currNeighbor = findNewNeighbor(neighbors);
            if (currNeighbor == null)
            {
                CurrTile = neighbors[direction];
                direction = UnityEngine.Random.Range(0, 6);
                
                if (tooFarUp && (direction == 0 || direction == 1 || direction == 5)) //too far up causes re-roll for upward tiles
                {
                    direction = UnityEngine.Random.Range(0, 6);
                }
                
                if (tooFarDown && (direction == 2 || direction == 3 || direction == 4)) //too far down causes a re-roll for downward tiles
                {
                    direction = UnityEngine.Random.Range(0, 6);
                }
                return false;
            }
        }
        
        
        Point point = new Point(currNeighbor.GetXPos(), currNeighbor.GetYPos());
        world.ModifyTileBiome(point, newBiome);
        CurrTile = neighbors[direction];
        direction = UnityEngine.Random.Range(0, 6);
        if (tooFarUp && (direction == 0 || direction == 1 || direction == 5)) //too far up causes re-roll for upward tiles
        {
            direction = UnityEngine.Random.Range(0, 6);
        }
                
        if (tooFarDown && (direction == 2 || direction == 3 || direction == 4)) //too far down causes a re-roll for downward tiles
        {
            direction = UnityEngine.Random.Range(0, 6);
        }
        return true;
        
    }

    /*goes through the list of neighbors and finds all non-null water tiles
     stores all valid tiles in a linked list, then randomly selects one
     returns the randomly selected Tile to be used as currNeighbor in move()
     */
    public GameTile findNewNeighbor(GameTile[] neighbors)
    {
        LinkedList<GameTile> validNeighbors = new LinkedList<GameTile>();
        
        foreach (GameTile neighbor in neighbors)
        {
            if (neighbor != null && neighbor.GetBiome() == oldBiome)
            {
                validNeighbors.AddLast(neighbor);
            }
        }
        
        for (int i = 0; i < UnityEngine.Random.Range(0, validNeighbors.Count); i++)
        {
            validNeighbors.RemoveFirst();
        }

        if (validNeighbors.Count > 0)
        {
            return validNeighbors.First.Value;
        }

        return null;
    }
}
