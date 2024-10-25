using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [HideInInspector] 
    public Game game;

   public void Awake()
   {
       if (game == null)
       {
           game = new Game();
       }

       game.world = NewWorld(1231231);
       game.civilizations = DetermineCivilizations();
       
       VillageTest();
   }

   public void EndTurn()
   {
       game.gameTurn++;

       if (game.civilizations != null)
       {
           foreach (Civilization civilization in game.civilizations)
           {
               civilization.OnTurnEnded();
           }
       }
   }
   
   List<Civilization> DetermineCivilizations()
   {
       return null;
   }

   public void VillageTest()
   {
       World world = game.world;
       GameTile testTile = world.GetTile(20, 20);
       Civilization testCivilization = new Civilization();
       Settlement village = new Settlement("Berkeley", testCivilization, testTile);
       testTile.SetSettlement(village);
   }

   World NewWorld(uint seed)
   {
     WorldGenerator worldGen = new WorldGenerator();

     return worldGen.GenerateWorld(100, 50, 2, seed);
   }

   public World GetWorld()
   {
       return game.world;
   }
    
}
