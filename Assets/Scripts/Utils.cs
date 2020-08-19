using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private const string _levelsPath = "Levels/";

    public static GameLevel ReadDefaultGameLevelFromAsset(int level)
    {
        object o = Resources.Load(_levelsPath + "Level" + level);
        GameLevel retrievedGameLevel = (GameLevel)o;
        return retrievedGameLevel;
    }
}
