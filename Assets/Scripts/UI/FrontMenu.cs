using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class FrontMenu : BaseUIMenu
{
    public Button ToMap;

    void Start()
    {
        ToMap.onClick.AddListener(() => GotoMap());
    }

    void Update()
    {
        
    }

    public void GotoMap()
    {
        Pop();
        CanvasManager.Push(GlobalInfor.MapMenu, null); GameEventSystem.current.ControlMap(true);
    }
}
