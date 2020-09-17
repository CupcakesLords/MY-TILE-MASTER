using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class PauseMenu : BaseUIMenu
{
    public Button _Continue;
    public Button _Restart;
    public Button _Theme;
    public Button _Home;
    public Text _Level;

    private int Level;
    void Start()
    {
        _Continue.onClick.AddListener(Continue);
        _Restart.onClick.AddListener(Restart);
        _Home.onClick.AddListener(BackToHome);
        _Theme.onClick.AddListener(ToTheme);
    }

    void ToTheme()
    {
        CanvasManager.Push(GlobalInfor.SkinMenu, null);
    }

    void Continue()
    {
        Pop();
        GameEventSystem.current.TimeControl(2);
    }

    public void Restart()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); GameEventSystem.current.TimeControl(3);
    }

    public void BackToHome()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        CanvasManager.Push(GlobalInfor.FrontMenu, null); GameEventSystem.current.TimeControl(3);
    }

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            Level = 1;
        else
        {
            object[] param = initParams;
            Level = (int)param[0];
        }

        _Level.text = "Level " + Level;
    }
}
