using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework;
using AFramework.UI;

public class WinMenu : BaseUIMenu
{
    public Button _Next;
    public Button _Replay;
    public Button _Back;
    public Text _Level;
    public Text _Star;
    int Level = 1;

    void Start()
    {
        _Back.onClick.AddListener(BackToMain);
        _Replay.onClick.AddListener(Replay);
        _Next.onClick.AddListener(NextLevel);
    }

    public void NextLevel()
    {
        int temp1 = BoardManager.instance.world; int temp2 = BoardManager.instance.level;
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        object[] param = new object[1];
        param[0] = Level + 1;

        if (Level + 1 > 1000)
            return;
        
        if(Utility.ReadCheckPoint().NumberOfLevel[temp1 - 1] == temp2) //end of this check point
        {
            BoardManager.instance.world = temp1 + 1;
            BoardManager.instance.level = 1;
        }
        else
        {
            BoardManager.instance.world = temp1;
            BoardManager.instance.level = temp2 + 1;
        }

        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); GameEventSystem.current.TimeControl(3);
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
        _Level.text = "LEVEL " + Level + " PASSED!";
        _Star.text = BoardManager.instance.star + " Star";

        int worldsave = BoardManager.instance.world; int levelsave = BoardManager.instance.level;

        if (Utility.ReadCheckPoint().NumberOfLevel[worldsave - 1] == levelsave) //end of this check point
        {
            GameData.I.LevelUp(worldsave + 1, 1);
            SaveGameManager.I.Save();
        }
        else
        {
            GameData.I.LevelUp(worldsave, levelsave + 1);
            SaveGameManager.I.Save();
        }
    }
}
