using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework;
using AFramework.UI;

public class GameManager : ManualSingletonMono<GameManager>
{
    void Start()
    {
        GameData.I.RegisterSaveData();
        SaveGameManager.I.Load();

        if (GameData.I.GetLevel() == 0) //first time game is opened
        {
            Debug.Log("First time");
            GameData.I.ResetData();
            SaveGameManager.I.Save();
        }
        else
        {
            Debug.Log("!First time");
            Debug.Log("World: " + GameData.I.GetWorld());
            Debug.Log("Level: " + GameData.I.GetLevel());
        }

        CanvasManager.Init(GlobalInfor.UIDefaultPath, GlobalInfor.FrontMenu);
    }

    IEnumerator CRAutoSave()
    {
        WaitForSeconds waitTime = new WaitForSeconds(5);
        while(true)
        {
            yield return waitTime;
            SaveGameManager.I.Save();
        }
    }
}
