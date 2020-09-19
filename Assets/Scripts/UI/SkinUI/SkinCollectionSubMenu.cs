using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class SkinCollectionSubMenu : BaseUIMenu
{
    private List<Button> skins = new List<Button>();
    public List<GameObject> Images;
    private int last_chosen = 1;

    private void Start()
    {
        Images[last_chosen - 1].SetActive(true);
    }

    public void ChangeSkin(int i)
    {
        if (i == last_chosen)
            return;

        Images[i - 1].SetActive(true);
        Images[last_chosen - 1].SetActive(false);

        BoardManager.instance.ResetBG(i);
        last_chosen = i;
    }
}
