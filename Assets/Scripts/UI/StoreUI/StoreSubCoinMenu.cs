using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework.UI;
using UnityEngine.UI;
using AFramework;

public class StoreSubCoinMenu : BaseUIMenu
{
    public Text coins;

    override
    public void Init(object[] initParams)
    {
        coins.text = GameData.I.GetCoin() + "";
    }

    public void Buy(int choice)
    {
        if(GameData.I.GetCoin() < 100)
        {
            return;
        }
        
        if(choice == 1) //undo
        {
            GameData.I.BuyUndo();
        }
        else if (choice == 2) //shuffle
        {
            GameData.I.BuyShuffle();
        }
        else if (choice == 3) //hint
        {
            GameData.I.BuyHint();
        }

        //GameData.I.MinusCoin(100);
        SaveGameManager.I.Save();
        coins.text = GameData.I.GetCoin() + "";

        if(BoardManager.instance.IsPlaying())
        {
            GameEventSystem.current.BoosterPurchaseUpdateUI(choice);
        }
    }
}
