using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class LoseMenu : BaseUIMenu
{
    public Button _Revive;
    public Button _Replay;
    public Button _Back;
    public Text _Level;
    private int Level = 1;

    void Start()
    {
        _Back.onClick.AddListener(BackToMain);
        _Replay.onClick.AddListener(Replay);
        _Revive.onClick.AddListener(Revive);
    }

    public void Revive()
    {
        BoardManager.instance.bar.Lighten();
        Pop();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.RewindLost(); GameEventSystem.current.TimeControl(2);
    }

    public void BackToMain()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        CanvasManager.Push(GlobalInfor.FrontMenu, null); GameEventSystem.current.TimeControl(3);
    }

    public void Replay()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); GameEventSystem.current.TimeControl(3);
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

        _Level.text = BoardManager.instance.world + " - " + BoardManager.instance.level;

    }
}
