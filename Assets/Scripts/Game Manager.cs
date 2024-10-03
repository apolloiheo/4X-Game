using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile tile;
    
    // Start is called before the first frame update
    void Start()
    {
        TestWorldGeneration();
    }

    private void TestWorldGeneration()
    {
        World gameWorld = new WorldGenerator().GenerateWorld(100, 50,2);
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
                if (world.GetTile(x, y).GetBiome() == 1)
                {
                    tile.color = new Color32(145, 158, 11, 255);
                    
                } else if (world.GetTile(x, y).GetBiome() == 2)
                {
                    tile.color = new Color32(92, 128, 82, 255);
                } else if (world.GetTile(x, y).GetBiome() == 3)
                {
                    tile.color = new Color32(144, 158, 141, 255);
                } else if (world.GetTile(x, y).GetBiome() == 4)
                {
                    tile.color = new Color32(255, 217, 112, 255);
                } else if (world.GetTile(x, y).GetBiome() == 5)
                {
                    tile.color = Color.white;
                } else if (world.GetTile(x, y).GetBiome() == 6)
                {
                    tile.color = new Color32(110, 187, 255, 255);
                } else if (world.GetTile(x, y).GetBiome() == 7)
                {
                    tile.color = new Color32(20, 102, 184, 255);
                }
                
                tilemap.SetTile(new Vector3Int(y, x, 0), tile);
            }
        }
    }
}
