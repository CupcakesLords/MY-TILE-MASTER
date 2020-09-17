using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class SkinCollectionSubMenu : BaseUIMenu
{
    private List<Button> skins = new List<Button>();

    public void ChangeSkin(int i)
    {
        BoardManager.instance.ResetBG(i);
    }
}
