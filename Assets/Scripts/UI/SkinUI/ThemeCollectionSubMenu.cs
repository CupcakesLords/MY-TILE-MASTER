using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using System;

public class ThemeCollectionSubMenu : BaseUIMenu
{
    private List<Button> themes = new List<Button>();

    public void ChangeTheme(int i)
    {
        BoardManager.instance.ResetCharacter(i);
    }
}
