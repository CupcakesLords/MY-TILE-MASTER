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
    public List<int> GetStar() { return mSaveData.stars; }
    public int GetCoin() { return mSaveData.coin; }

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
        mSaveData.stars = null; mSaveData.stars = new List<int>(); mSaveData.stars.Add(0);
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

    public void AddStar(int star)
    {
        if (star < 0 || star > 3)
            star = 3;

        mSaveData.stars.RemoveAt(mSaveData.stars.Count - 1); //remove last
        mSaveData.stars.Add(star);
        mSaveData.stars.Add(0);
        DataChanged = true;
    }

    public void AddOldStar(int level, int star)
    {
        if (level - 1 < 0 || level - 1 >= mSaveData.stars.Count)
            return;

        if (star < 0 || star > 3)
            star = 3;
        if (mSaveData.stars[level - 1] < star)
        {
            mSaveData.stars[level - 1] = star;
        }
        DataChanged = true;
    }

    public void AddCoin(int coin)
    {
        if (coin <= 0)
            return;
        mSaveData.coin += coin;
        DataChanged = true;
    }
}


