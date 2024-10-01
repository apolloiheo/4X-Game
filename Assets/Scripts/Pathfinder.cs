using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public static class Pathfinder
// if _terrain is 2 the tile is impassable.
// C# version 9 doees not have a Priority Queue, so for now I'm going to borrow one from Red Blob
{
    public static Dictionary<Tile,Tile> AStar(Tile start, Tile goal)
    {
        Dictionary<Tile,Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, double>  costSoFar = new Dictionary<Tile, double>();

        PriorityQueue<Tile, double> frontier = new PriorityQueue<Tile, double>();
        Debug.Log(start);
        Debug.Log(goal);
        Debug.Log(frontier);
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0) {
            Tile current = frontier.Dequeue();
            if (current == goal)
            {
                break;
            }
            Debug.Log(current.GetNeighbors().Length);
            for (int x = 0; x < current.GetNeighbors().Length; x++)
            {
                Debug.Log(x);
                Tile neighbour = current.GetNeighbors()[x];
                Debug.Log(neighbour);
                /* if (neighbour.GetTerrain() == 2)
                {
                    continue; 
                } */
                double new_cost = costSoFar[current] + neighbour.GetMovementCost();
                if (!costSoFar.ContainsKey(neighbour) || new_cost < costSoFar[neighbour])
                {
                    costSoFar[neighbour] = new_cost;
                    double priority = new_cost + Position.cost_distance(current, neighbour);
                    frontier.Enqueue(current, priority);
                    cameFrom[neighbour] = current;
                }
            }
            
    }
        return cameFrom;
    }

    public class PriorityQueue<TElement, TPriority>
    {
        private List<Tuple<TElement, TPriority>> elements = new List<Tuple<TElement, TPriority>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(TElement item, TPriority priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public TElement Dequeue()
        {
            Comparer<TPriority> comparer = Comparer<TPriority>.Default;
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (comparer.Compare(elements[i].Item2, elements[bestIndex].Item2) < 0)
                {
                    bestIndex = i;
                }
            }

            TElement bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
