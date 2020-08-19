using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CreateGameLevelMenuItem
{
    [MenuItem("Custom/Game levels/Create New Game Level Holder")]
    public static void CreateGameLevelHolder()
    {
        GameLevel level =
            ScriptableObject.CreateInstance<GameLevel>();
        
        level.tiles = new List<Tile>();
       
        AssetDatabase.CreateAsset(level,
            "Assets/Resources/Levels/Level" + level.level.ToString() +".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = level;
    }
}
