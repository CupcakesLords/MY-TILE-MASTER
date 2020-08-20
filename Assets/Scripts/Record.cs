using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record 
{
    public float prevX;
    public float prevY;
    public GameObject tile;

    public Record(float x, float y, GameObject t)
    {
        prevX = x;
        prevY = y;
        tile = t;
    }
}
