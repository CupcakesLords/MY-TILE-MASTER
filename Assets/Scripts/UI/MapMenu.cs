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
    public Text AllStar;

    public List<GameObject> BG;
    public List<GameObject> Locks;
    public List<GameObject> Stars;
    public List<GameObject> Flags;
    public List<GameObject> Buttons;

    private CheckPoint lv;
    private List<string> memo = new List<string>();
    private int currentFlag = -1;

    void Start()
    {
        Back.onClick.AddListener(() => BackToFront());
        Skin.onClick.AddListener(() => ToSkinMenu());
        currentWorld.onClick.AddListener(() => GotoActionPhase(GameData.I.GetWorld()));

        lv = Utility.ReadCheckPoint();
    }

    public void GotoActionPhase(int world)
    {
        world = world - 1;

        object[] param = new object[4];
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
        if (memo != null && world < memo.Count)
            param[3] = memo[world];
        else
            param[3] = "Bugged";

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

        memo.Clear(); memo = null; memo = new List<string>();

        for (int i = 0; i < BG.Count; i++)
        {
            if (i < GameData.I.GetWorld())
            {
                BG[i].SetActive(true);
                Locks[i].SetActive(false);
                Buttons[i].GetComponent<Button>().interactable = true;
                Stars[i].SetActive(true);

                SetStar(i);
            }
            else
            {
                BG[i].SetActive(false);
                Locks[i].SetActive(true);
                Buttons[i].GetComponent<Button>().interactable = false;
                Stars[i].SetActive(false);
            }
        }

        Process.text = GameData.I.GetWorld() + " - " + GameData.I.GetLevel();
        int temp = 0;
        foreach (int i in GameData.I.GetStar())
            temp += i;
        AllStar.text = temp + "";

        if(currentFlag >= 0)
        {
            Flags[currentFlag].SetActive(false);
        }
        Flags[GameData.I.GetWorld() - 1].SetActive(true);
        currentFlag = GameData.I.GetWorld() - 1;
    }

    private void SetStar(int n)
    {
        if (lv == null)
            lv = Utility.ReadCheckPoint();

        if (n >= lv.NumberOfLevel.Count)
            return;

        int total = lv.NumberOfLevel[n];
        int level = 0;

        for (int i = 0; i < lv.NumberOfLevel.Count; i++)
        {
            if (i == n)
                break;
            level += lv.NumberOfLevel[i];
        }

        if (level >= GameData.I.GetStar().Count)
            return;

        int current = 0;
        for (int i = level; i < level + total; i++)
        {
            if (i >= GameData.I.GetStar().Count)
                break;
            current += GameData.I.GetStar()[i];
        }

        Stars[n].GetComponentInChildren<Text>().text = current + "/" + (total * 3);
        memo.Add(current + "/" + (total * 3));
    }
}
