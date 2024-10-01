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
        World gameWorld = new World(100,50);
        gameWorld.FillEmptyWorld(1);
        //gameWorld.PrintWorld();
        gameWorld.SetTileAdjacency();
        gameWorld.TestTileAdjacency(0,0);
        gameWorld.TestTileAdjacency(1,0);
        gameWorld.SetTileAdjacency();
        gameWorld.TestPathfinder(1,0,0,0);
    }
}
