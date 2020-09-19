using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using System;

public class ThemeCollectionSubMenu : BaseUIMenu
{
    private List<Button> themes = new List<Button>();
    public List<GameObject> Images;
    private int last_chosen = 1;

    private void Start()
    {
        Images[last_chosen - 1].SetActive(true);
    }

    public void ChangeTheme(int i)
    {
        if (i == last_chosen)
            return;

        Images[i - 1].SetActive(true);
        Images[last_chosen - 1].SetActive(false);

        BoardManager.instance.ResetCharacter(i);
        last_chosen = i;
    }
}
