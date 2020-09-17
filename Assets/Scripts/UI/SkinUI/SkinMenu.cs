using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using System;

public class SkinMenu : BaseUIMenu
{
    public Button Back;
    private int currentToggle = 1;
    private bool first = true;

    void Start()
    {
        Back.onClick.AddListener(() => BackToMap());
    }

    private void BackToMap()
    {
        Pop();
        if (currentToggle == 1)
            CanvasManager.Pop(GlobalInfor.SkinCollectionSubMenu);
        else if(currentToggle == 2)
            CanvasManager.Pop(GlobalInfor.ThemeCollectionSubMenu);
    }

    public void Toggle(int ID)
    {
        if (first)
        {
            first = false; return;
        }
        
        if (ID == currentToggle)
            return;

        if(ID == 1)
        {
            CanvasManager.Pop(GlobalInfor.ThemeCollectionSubMenu);
            CanvasManager.Push(GlobalInfor.SkinCollectionSubMenu, null);
        }
        else if(ID == 2)
        {
            CanvasManager.Pop(GlobalInfor.SkinCollectionSubMenu);
            CanvasManager.Push(GlobalInfor.ThemeCollectionSubMenu, null);
        }

        currentToggle = ID;
    }

    override
    public void Init(object[] initParams)
    {
        if (currentToggle == 1)
            CanvasManager.Push(GlobalInfor.SkinCollectionSubMenu, null);
        else if (currentToggle == 2)
            CanvasManager.Push(GlobalInfor.ThemeCollectionSubMenu, null);
    }
}
