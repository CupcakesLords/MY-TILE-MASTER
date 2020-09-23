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
        public int coin;
        public int world;
        public List<int> stars;
        public int undo;
        public int shuffle;
        public int hint;
    }

    private SaveData mSaveData;

    public bool DataChanged { get; set; }

    public object GetData()
    {
        return mSaveData;
    }

    public int GetLevel() { return mSaveData.level; }
    public int GetWorld() { return mSaveData.world; }

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

    public void ResetData()
    {
        mSaveData.level = 1;
        mSaveData.world = 1;
        mSaveData.coin = 0;
        mSaveData.stars = null;
        mSaveData.undo = 3;
        mSaveData.shuffle = 3;
        mSaveData.hint = 3;
        DataChanged = true;
    }

    public void LevelUp(int world, int level)
    {
        mSaveData.level = level;
        mSaveData.world = world;
        DataChanged = true;
    }
}


