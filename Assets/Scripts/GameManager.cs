using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework;
using AFramework.UI;

public class GameManager : ManualSingletonMono<GameManager>
{
    void Start()
    {
        CanvasManager.Init(GlobalInfor.UIDefaultPath, GlobalInfor.MainMenu);
    }

    void Update()
    {
        
    }
}
