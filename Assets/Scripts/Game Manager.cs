using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
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
   }

   public void EndTurn()
   {
       game.gameTurn++;
   }
   
   List<Civilization> DetermineCivilizations()
   {
       return null;
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
