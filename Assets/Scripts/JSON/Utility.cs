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

        //string line = System.IO.File.ReadAllText("Assets/Resources/MapConfigs/" + level + ".txt");

        //Load a text file (Assets/Resources/MapConfigs/1.txt)
        TextAsset textFile = Resources.Load<TextAsset>("MapConfigs/" + level);
        string line = textFile.text;

        result.pos = JsonUtility.FromJson<Level>(line);

        List<int> t = line.AllIndexesOf("t");
        t.Add(line.Length - 1);
        for(int i = 0; i < t.Count - 1; i++) //one layer at a time
        {
            List<TilePosition> tiles = new List<TilePosition>();

            List<int> comas = line.Substring(t[i], t[i + 1] - t[i]).AllIndexesOf("],[");

            AddToPositionTemp(line.Substring(t[i] + 4, comas[0] - 3), result.pos.l[i].p, result.pos.p, tiles);

            for (int j = 0; j < comas.Count - 1; j++)
            {
                AddToPositionTemp(line.Substring(t[i] + comas[j] + 2, comas[j + 1] - comas[j] - 1), result.pos.l[i].p, result.pos.p, tiles);
            }

            int end = line.Substring(t[i], t[i + 1] - t[i]).IndexOf("]]");

            AddToPositionTemp(line.Substring(t[i] + comas[comas.Count - 1] + 2, end - comas[comas.Count - 1] - 1), result.pos.l[i].p, result.pos.p, tiles);

            SortTile(tiles);
            for(int a = 0; a < tiles.Count; a++)
            {
                result.tiles.Add(new TilePosition(tiles[a].x, tiles[a].y, tiles[a].z + a));
            }
        }

        return result;
    }

    public static void AddToPosition(string json, List<float> layer, List<float> parent, GameLevelObject lv, int increment)
    {
        List<int> comas = json.AllIndexesOf(",");
        string x = json.Substring(1, comas[0] - 1);
        string y = json.Substring(comas[0] + 1, comas[1] - comas[0] - 1);
        string z = json.Substring(comas[1] + 1, json.Length - 2 - comas[1]);
        lv.tiles.Add(new TilePosition((float.Parse(x) + layer[0] + parent[0]) / 100, (float.Parse(y) + layer[1] + parent[1]) / 100, float.Parse(z) + layer[2] + parent[2] + increment));
    }

    public static void AddToPositionTemp(string json, List<float> layer, List<float> parent, List<TilePosition> tiles)
    {
        List<int> comas = json.AllIndexesOf(",");
        string x = json.Substring(1, comas[0] - 1);
        string y = json.Substring(comas[0] + 1, comas[1] - comas[0] - 1);
        string z = json.Substring(comas[1] + 1, json.Length - 2 - comas[1]);
        tiles.Add(new TilePosition((float.Parse(x) + layer[0] + parent[0]) / 100, (float.Parse(y) + layer[1] + parent[1]) / 100, float.Parse(z) + layer[2] + parent[2]));
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
}

