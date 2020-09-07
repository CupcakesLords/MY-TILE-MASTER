using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MapMenu : BaseUIMenu
{
    public Button Back;

    private CheckPoint lv;

    void Start()
    {
        Back.onClick.AddListener(() => BackToFront());

        lv = Utility.ReadCheckPoint();
    }

    public void GotoActionPhase(int world)
    {
        world = world - 1;

        object[] param = new object[3];
        param[0] = world + 1;
        param[1] = lv.NumberOfLevel[world];

        int level = 1;

        for(int i = 0; i < lv.NumberOfLevel.Count; i++)
        {
            if (i == world)
                break;
            level += lv.NumberOfLevel[i];
        }

        param[2] = level;

        CanvasManager.Push(GlobalInfor.MainMenu, param);
    }

    public void BackToFront()
    {
        Pop();
        CanvasManager.Push(GlobalInfor.FrontMenu, null); GameEventSystem.current.ControlMap(false);
    }
}
