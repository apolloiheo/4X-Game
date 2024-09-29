using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestWorldGeneration();
    }

    private void TestWorldGeneration()
    {
        World gameWorld = new World(100,50, 2);
        gameWorld.FillEmptyWorld(7);
        gameWorld.PrintWorld();
        gameWorld.SetTileAdjacency();
        gameWorld.TestTileAdjacency(2,0);
        gameWorld.TestTileAdjacency(1,0);
        gameWorld.TestTileAdjacency(3,0);
        gameWorld.TestTileAdjacency(0,1);
        gameWorld.TestTileAdjacency(0,2);
        gameWorld.TestTileAdjacency(99,49);
    }
}
