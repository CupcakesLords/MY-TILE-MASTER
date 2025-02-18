﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class FrontMenu : BaseUIMenu
{
    public Button Setting;
    public Button ToMap;
    public Button StartGame;
    public Text Process;
    public Text World;
    public Text Coins;
    public Button Coin;
    public Button Shop;

    void Start()
    {
        ToMap.onClick.AddListener(() => GotoMap());
        StartGame.onClick.AddListener(() => GoToCurrentLevel());
        Coin.onClick.AddListener(() => GoToShop());
        Shop.onClick.AddListener(() => GoToShop());
        Setting.onClick.AddListener(() => GoToSetting());
    }

    void GoToSetting()
    {
        CanvasManager.Push(GlobalInfor.SettingMenu, null);
    }

    void GoToShop()
    {
        object[] param = new object[1];
        param[0] = 1;
        CanvasManager.Push(GlobalInfor.StoreMenu, param); 
    }

    void GoToCurrentLevel()
    {
        Pop();

        object[] param = new object[1];
        param[0] = Utility.CalculateRelativeLevel(GameData.I.GetWorld(), GameData.I.GetLevel());

        BoardManager.instance.world = GameData.I.GetWorld();
        BoardManager.instance.level = GameData.I.GetLevel();

        CanvasManager.Push(GlobalInfor.GamePlayMenu, param);
    }

    public void GotoMap()
    {
        Pop();
        CanvasManager.Push(GlobalInfor.MapMenu, null); 
    }

    override
    public void Init(object[] initParams)
    {
        Process.text = GameData.I.GetWorld() + " - " + GameData.I.GetLevel();
        Coins.text = GameData.I.GetCoin() + "";
        World.text = Utility.ReadCheckPoint().Names[GameData.I.GetWorld() - 1];
    }
}
