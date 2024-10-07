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
    private GameTile failSafeTile;

    public WorldGenWalker(World w, GameTile startTile, int _direction)
    {
        world = w;
        CurrTile = startTile;
        failSafeTile = startTile;
        direction = _direction;
        _random.InitState();
        _random = new Random(124);
    }

    public bool move()
    {
        if (CurrTile == null)
        {
            CurrTile = failSafeTile;
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
                return false;
            }
        }
        
        
        Point point = new Point(currNeighbor.GetXPos(), currNeighbor.GetYPos());
        world.ModifyTileBiome(point, 1);
        CurrTile = neighbors[direction];
        direction = _random.NextInt(0, 6);
        return true;
        
    }

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
