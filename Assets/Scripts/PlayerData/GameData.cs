using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : AFramework.SingletonMono<GameData>, AFramework.ISaveData
{
    #region save
    [System.Serializable]
    public class SaveData
    {
        public int level;
    }
    SaveData mSaveData;

    public bool DataChanged { get; set; }

    public object GetData()
    {
        return mSaveData;
    }

    public void RegisterSaveData()
    {
        AFramework.SaveGameManager.I.RegisterMandatoryData("gamedata", this);
    }

    public void SetData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            mSaveData = new SaveData();
        }
        else
        {
            mSaveData = JsonUtility.FromJson<SaveData>(data);
        }

        //abc
    }

    public void OnAllDataLoaded()
    {

    }
    #endregion

    public void SetLevel(int newLevel)
    {
        mSaveData.level = newLevel;
        DataChanged = true;
    }
}
