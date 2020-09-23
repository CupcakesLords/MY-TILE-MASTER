using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static GameLevelObject ReadGameLevelFromAsset(int level)
    {
        GameLevelObject result = new GameLevelObject();
        result.level = level;

        //Load a text file (Assets/Resources/MapConfigs/1.txt)
        TextAsset textFile = Resources.Load<TextAsset>("MapConfigs/" + level);
        string line = textFile.text;

        result.pos = JsonUtility.FromJson<Level>(line);

        List<int> t = line.AllIndexesOf("t");
        t.Add(line.Length - 1);
        for (int i = 0; i < t.Count - 1; i++) //one layer at a time
        {
            List<TilePosition> tiles = new List<TilePosition>();

            List<int> comas = line.Substring(t[i], t[i + 1] - t[i]).AllIndexesOf("],[");

            if(comas.Count == 0) //layer only has 1 tile
            {
                int mono = line.Substring(t[i] + 4).IndexOf("]]");
                AddToPositionTemp(line.Substring(t[i] + 4, mono + 1), result.pos.l[i].p, result.pos.p, tiles);
                result.tiles.Add(new TilePosition(tiles[0].x, tiles[0].y, tiles[0].z));
                continue;
            }
            
            AddToPositionTemp(line.Substring(t[i] + 4, comas[0] - 3), result.pos.l[i].p, result.pos.p, tiles);
            
            for (int j = 0; j < comas.Count - 1; j++)
            {
                AddToPositionTemp(line.Substring(t[i] + comas[j] + 2, comas[j + 1] - comas[j] - 1), result.pos.l[i].p, result.pos.p, tiles);
            }

            int end = line.Substring(t[i], t[i + 1] - t[i]).IndexOf("]]");

            AddToPositionTemp(line.Substring(t[i] + comas[comas.Count - 1] + 2, end - comas[comas.Count - 1] - 1), result.pos.l[i].p, result.pos.p, tiles);

            SortTile(tiles);
            for (int a = 0; a < tiles.Count; a++)
            {
                result.tiles.Add(new TilePosition(tiles[a].x, tiles[a].y, tiles[a].z + a));
            }
        }

        // make sure the number of tiles can be divided by 3
        int residual = result.tiles.Count % 3;
        for (int i = 0; i < residual; i++)
        {
            result.tiles.RemoveAt(0);
        }

        return result;
    }

    public static void AddToPositionTemp(string json, List<float> layer, List<float> parent, List<TilePosition> tiles)
    {
        List<int> comas = json.AllIndexesOf(",");
        string x = json.Substring(1, comas[0] - 1);
        string y = json.Substring(comas[0] + 1, comas[1] - comas[0] - 1);
        string z = json.Substring(comas[1] + 1, json.Length - 2 - comas[1]);
        tiles.Add(new TilePosition((float.Parse(x) + layer[0] + parent[0]) / 100, (float.Parse(y) + layer[1] + parent[1]) / 100, float.Parse(z) + CalculateBaseLayer(layer[2], parent[2]))); ////
    }

    public static float CalculateBaseLayer(float a, float b)
    {
        return -a - b;
    }

    public static int CalculateBaseLayer(int a, int b)
    {
        return -a - b;
    }

    public static void SortTile(List<TilePosition> tiles)
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            for(int j = i + 1; j < tiles.Count; j++)
            {
                if(TilePositionCompare(tiles[i], tiles[j]) == false)
                {
                    TilePosition temp;
                    temp = tiles[i];
                    tiles[i] = tiles[j];
                    tiles[j] = temp;
                }
            }
        }
    }

    public static bool TilePositionCompare(TilePosition a, TilePosition b) //a and b is different, return if a < b
    {
        if (a.y > b.y)
            return true;
        else if (a.y == b.y)
        {
            if (a.x < b.x)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public static List<int> AllIndexesOf(this string str, string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }

    private static CheckPoint WorldLevel;

    public static CheckPoint ReadCheckPoint()
    {
        if(WorldLevel != null)
        {
            return WorldLevel;
        }

        CheckPoint result = new CheckPoint();

        TextAsset textFile = Resources.Load<TextAsset>("CheckPoint/CheckPointData");
        string line = textFile.text;

        result = JsonUtility.FromJson<CheckPoint>(line);

        WorldLevel = result;

        return result;
    }

    public static int CalculateRelativeLevel(int world, int level)
    {
        if (world <= 0 || level <= 0)
            return 1;

        if(WorldLevel == null)
        {
            CheckPoint re = new CheckPoint();

            TextAsset textFile = Resources.Load<TextAsset>("CheckPoint/CheckPointData");
            string line = textFile.text;

            re = JsonUtility.FromJson<CheckPoint>(line);

            WorldLevel = re;
        }

        int result = 0;
        for(int i = 0; i < world - 1; i++)
        {
            result += WorldLevel.NumberOfLevel[i];
        }
        result += level;

        if (result > 1000)
            return 1000;
        else
            return result;
    }
}

