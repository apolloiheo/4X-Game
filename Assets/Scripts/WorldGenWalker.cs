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
    private Random _random;
    public bool tooFarUp = false;
    public bool tooFarDown = false;

    public WorldGenWalker(World w, GameTile startTile, int _direction) //initializing variables
    {
        world = w;
        CurrTile = startTile;
        direction = _direction;
        _random.InitState();
        _random = new Random(8);
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
        if (CurrTile == null || _random.NextInt(0, 100) > 98) // 1% chance for walker to go rogue
        {
            int randomX = _random.NextInt(0, world.GetLength() - 1);
            int randomY = _random.NextInt(0, world.GetHeight() - 1);
            CurrTile = world.GetTile(new Point(randomX, randomY));
            return false;
        }
        GameTile[] neighbors = CurrTile.GetNeighbors();
        GameTile currNeighbor = neighbors[direction];

        if (currNeighbor == null || currNeighbor.GetBiome() != 7)
        {
            currNeighbor = findNewNeighbor(neighbors);
            if (currNeighbor == null)
            {
                CurrTile = neighbors[direction];
                direction = _random.NextInt(0, 6);
                
                if (tooFarUp && (direction == 0 || direction == 1 || direction == 5)) //too far up causes re-roll for upward tiles
                {
                    direction = _random.NextInt(0, 6);
                }
                
                if (tooFarDown && (direction == 2 || direction == 3 || direction == 4)) //too far down causes a re-roll for downward tiles
                {
                    direction = _random.NextInt(0, 6);
                }
                return false;
            }
        }
        
        
        Point point = new Point(currNeighbor.GetXPos(), currNeighbor.GetYPos());
        world.ModifyTileBiome(point, 1);
        CurrTile = neighbors[direction];
        direction = _random.NextInt(0, 6);
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
            if (neighbor != null && neighbor.GetBiome() == 7)
            {
                validNeighbors.AddLast(neighbor);
            }
        }
        for (int i = 0; i < _random.NextInt(0, validNeighbors.Count); i++)
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
