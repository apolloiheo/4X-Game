using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class WorldGenWalker : MonoBehaviour
{
    public GameTile CurrTile;
    public World world;
    public int direction;
    private Random _random;
    public int failCount = 0;

    public WorldGenWalker(World w, GameTile startTile, int _direction)
    {
        world = w;
        CurrTile = startTile;
        direction = _direction;
        _random.InitState();
        _random = new Random(3);
    }

    public bool move()
    {
        GameTile[] neighbors = CurrTile.GetNeighbors();

        while (neighbors[direction] == null)
        {
            direction = _random.NextInt(0, neighbors.Length);
        }
        
        Point point = new Point(neighbors[direction].GetXPos(), neighbors[direction].GetYPos());
        if (neighbors[direction].GetBiome() == 7)
        {
            world.ModifyTileBiome(point, 1);
            CurrTile = neighbors[direction];
            direction = _random.NextInt(0, 6);
            return true;
        }
        
        CurrTile = neighbors[direction];
        direction = _random.NextInt(0, 6);
        failCount++;
        return false;
        
    }
}
