using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class GamePlayMenu : BaseUIMenu
{
    public Button _Pause;
    public Button _Undo;
    public Button _Refresh;
    public Button _Hint;
    public Text _Level;

    private int Level = 1;
    private float Refresh_Timer = 0;

    void Start()
    {
        _Pause.onClick.AddListener(Pause);
        _Undo.onClick.AddListener(Undo);
        _Refresh.onClick.AddListener(Refresh);
        _Hint.onClick.AddListener(Hint);
    }

    override
    public void Init(object[] initParams) {

        if (initParams == null)
            Level = 1;
        else
        {
            object[] param = initParams;
            Level = (int)param[0];
        }

        _Level.text = "Level " + BoardManager.instance.world + " - " + BoardManager.instance.level;
        
        BoardManager.instance.StartNewGame(Level);
    }

    public void Pause()
    {
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push(GlobalInfor.PauseMenu, param);
        GameEventSystem.current.TimeControl(1);
    }

    public void Undo()
    {
        BoardManager.instance.UndoRecord();
    }

    public void Refresh()
    {
        if (Refresh_Timer > 0)
            return;
        Refresh_Timer = 1.5f;
        BoardManager.instance.Refresh();
    }

    public void Hint()
    {
        BoardManager.instance.Hint();
    }

    void Update()
    {
        if (Refresh_Timer > 0)
            Refresh_Timer -= Time.deltaTime;
    }
}
