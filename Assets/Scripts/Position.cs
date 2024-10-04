using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Position
{
    // EvenQ and EvenQ to Axial isn't necessary, but exists in case we ever would want to work with offset coordinates for whatever reason
    // or if we need to convert the Axial coordinates back into the original tile's coordinates.
    public static EvenQPos EvenQ(GameTile a) { 
        int col = a.GetXPos();
        int row = a.GetYPos() + (a.GetXPos() + (a.GetXPos() & 1)) / 2;
        return new EvenQPos(col, row);
        }


    public static AxialPos Axial(GameTile a) {
        int q = a.GetXPos();
        int r = a.GetYPos() - (a.GetXPos() + (a.GetXPos() & 1)) / 2;
        return new AxialPos(q, r);
    }

    public static EvenQPos AxialToEvenQ(AxialPos hex)
    {
        int col = hex.q;
        int row = hex.r + (hex.q + (hex.q & 1)) / 2;
        return new EvenQPos(col, row);
    }
    public static AxialPos EvenQToAxial(EvenQPos hex)
    {
        int q = hex.x;
        int r = hex.y - (hex.x + (hex.x & 1)) / 2;
        return new AxialPos(q, r);
    }


    public static float axial_distance(AxialPos a, AxialPos b) {
        return (Mathf.Abs(a.q - b.q)
              + Mathf.Abs(a.q + a.r - b.q - b.r)
              + Mathf.Abs(a.r - b.r)) / 2;
          }

    public static float axial_distance(GameTile a, GameTile b)
    {
        AxialPos posA = Axial(a);
        AxialPos posB = Axial(b);
        return axial_distance(posA, posB);
    }

    // All in one
    public static float cost_distance(GameTile a, GameTile b)
    {
        return axial_distance(a, b) * a.GetMovementCost();
    }


}

public class AxialPos
{
    public int q;
    public int r;

    public AxialPos(int q, int r)
    {
        this.q = q;
        this.r = r;
    }
}

public class EvenQPos
{
    public int x; // aka col
    public int y; // aka row

    public EvenQPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
