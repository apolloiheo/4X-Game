using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        TestWorldGeneration();
    }

    private void TestWorldGeneration()
    {
        World gameWorld = new WorldGenerator().GenerateWorld(100, 50,2);
        gameWorld.PrintWorld();
        DrawTilemap(gameWorld);
        gameWorld.SetTileAdjacency();
        gameWorld.TestTileAdjacency(2,0);
        gameWorld.TestTileAdjacency(1,0);
        gameWorld.TestTileAdjacency(3,0);
        gameWorld.TestTileAdjacency(0,1);
        gameWorld.TestTileAdjacency(0,2);
        gameWorld.TestTileAdjacency(99,49);
    }

    public void DrawTilemap(World world)
    {
        for (int x = 0; x < world.GetLength(); x++)
        {
            for (int y = 0; y < world.GetHeight(); y++)
            {
                // To be implemented
                
            }
        }
    }
}
