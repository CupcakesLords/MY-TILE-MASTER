using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework;
using AFramework.UI;

public class GameManager : ManualSingletonMono<GameManager>
{
    void Start()
    {
        CanvasManager.Init(GlobalInfor.UIDefaultPath, GlobalInfor.FrontMenu);

        GameData.I.RegisterSaveData();

        SaveGameManager.I.Load();

        StartCoroutine(CRAutoSave());
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
