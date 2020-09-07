using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework.UI;
using UnityEngine.EventSystems;

public class WorldButton : MonoBehaviour
{
    private int World;

    private CheckPoint lv;

    void Start()
    {
        lv = Utility.ReadCheckPoint();
        World = int.Parse(name);
    }

    public void GotoActionPhase(int world)
    {
        world = world - 1;

        object[] param = new object[3];
        param[0] = world + 1;
        param[1] = lv.NumberOfLevel[world];

        int level = 1;

        for (int i = 0; i < lv.NumberOfLevel.Count; i++)
        {
            if (i == world)
                break;
            level += lv.NumberOfLevel[i];
        }

        param[2] = level;

        CanvasManager.Push(GlobalInfor.MainMenu, param);
    }
    
    private void OnMouseDown()
    {
        GotoActionPhase(World);
    }
}
