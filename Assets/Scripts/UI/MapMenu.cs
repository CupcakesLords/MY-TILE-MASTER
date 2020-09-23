using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MapMenu : BaseUIMenu
{
    public Button Back;

    public Button Skin;

    public Button currentWorld;

    public Text Process;

    public List<GameObject> BG;
    public List<GameObject> Locks;

    private CheckPoint lv;

    void Start()
    {
        Back.onClick.AddListener(() => BackToFront());
        Skin.onClick.AddListener(() => ToSkinMenu());
        currentWorld.onClick.AddListener(() => GotoActionPhase(GameData.I.GetWorld()));

        Process.text = GameData.I.GetWorld() + " - " + GameData.I.GetLevel();

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
        CanvasManager.Push(GlobalInfor.FrontMenu, null); 
    }

    public void ToSkinMenu()
    {
        CanvasManager.Push(GlobalInfor.SkinMenu, null);
    }

    override
    public void Init(object[] initParams)
    {
        if (BG.Count != Locks.Count)
            return;
        
        for (int i = 0; i < BG.Count; i++)
        {
            if (i < GameData.I.GetWorld())
            {
                BG[i].SetActive(true);
                Locks[i].SetActive(false);
            }
            else
            {
                BG[i].SetActive(false);
                Locks[i].SetActive(true);
            }
        }

        Process.text = GameData.I.GetWorld() + " - " + GameData.I.GetLevel();
    }
}
