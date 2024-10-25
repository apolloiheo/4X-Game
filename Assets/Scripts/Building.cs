using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : MonoBehaviour
{
    private int[] _yields;




    public int[] GetYields()
    {
        return _yields;
    }
}
