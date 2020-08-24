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

        string line = System.IO.File.ReadAllText("Assets/Map/MapConfigs/" + level + ".txt");
        
        result.pos = JsonUtility.FromJson<Level>(line);

        List<int> t = line.AllIndexesOf("t");
        t.Add(line.Length - 1);
        for(int i = 0; i < t.Count - 1; i++)
        {
            int increment = 0;
            List<int> comas = line.Substring(t[i], t[i + 1] - t[i]).AllIndexesOf("],[");

            //Debug.Log(line.Substring(t[i] + 4, comas[0] - 3));
            AddToPosition(line.Substring(t[i] + 4, comas[0] - 3), result.pos.l[i].p, result.pos.p, result, increment); increment++;

            for (int j = 0; j < comas.Count - 1; j++)
            {
                //Debug.Log(line.Substring(t[i] + comas[j] + 2, comas[j + 1] - comas[j] - 1));
                AddToPosition(line.Substring(t[i] + comas[j] + 2, comas[j + 1] - comas[j] - 1), result.pos.l[i].p, result.pos.p, result, increment); increment++;
            }

            int end = line.Substring(t[i], t[i + 1] - t[i]).IndexOf("]]");
            //Debug.Log(line.Substring(t[i] + comas[comas.Count - 1] + 2, end - comas[comas.Count - 1] - 1));
            AddToPosition(line.Substring(t[i] + comas[comas.Count - 1] + 2, end - comas[comas.Count - 1] - 1), result.pos.l[i].p, result.pos.p, result, increment); increment++;
        }
        /*
        foreach(TilePosition i in result.tiles)
        {
            Debug.Log(i.x + "|" + i.y + "|" + i.z);
        }
        */
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

