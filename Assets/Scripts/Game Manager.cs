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
                // Current Tile Position Vector 3
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
                tilemap.SetTile(tilePosition, tile);
                
                if (world.GetTile(x, y).GetBiome() == 7)
                {
                    tilemap.SetColor(tilePosition, Color.blue);
                } else if (world.GetTile(x, y).GetBiome() == 0 || world.GetTile(x, y).GetBiome() == 1)
                {
                    tilemap.SetColor(tilePosition, Color.yellow);
                } else if (world.GetTile(x, y).GetBiome() == 5)
                {
                    tilemap.SetColor(tilePosition, Color.white);
                } else if (world.GetTile(x, y).GetBiome() == 3)
                {
                    tilemap.SetColor(tilePosition, Color.gray);
                }
            }
        }
    }
}
