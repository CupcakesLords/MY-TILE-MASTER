using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelObject
{
    public int level;
    public Level pos;
    public List<TilePosition> tiles;
    public GameLevelObject()
    {
        tiles = new List<TilePosition>();
    }
}

public class TilePosition
{
    public float x, y, z;
    public TilePosition(float a, float b, float c)
    {
        x = a; y = b; z = c;
    }
}
