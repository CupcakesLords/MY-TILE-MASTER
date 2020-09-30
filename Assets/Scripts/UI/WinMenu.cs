using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework;
using AFramework.UI;

public class WinMenu : BaseUIMenu
{
    public List<GameObject> Stars;
    public Button _Next;
    public Button _Replay;
    public Button _Back;
    public Text _Level;
    public Text _Star;
    public Text _Coin;
    int Level = 1;

    void Start()
    {
        _Back.onClick.AddListener(BackToMain);
        _Replay.onClick.AddListener(Replay);
        _Next.onClick.AddListener(NextLevel);
    }

    public void NextLevel()
    {
        int temp1 = BoardManager.instance.world; int temp2 = BoardManager.instance.level;
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        object[] param = new object[1];
        param[0] = Level + 1;

        if (Level + 1 > 1000)
            return;
        
        if(Utility.ReadCheckPoint().NumberOfLevel[temp1 - 1] == temp2) //end of this check point
        {
            BoardManager.instance.world = temp1 + 1;
            BoardManager.instance.level = 1;
        }
        else
        {
            BoardManager.instance.world = temp1;
            BoardManager.instance.level = temp2 + 1;
        }

        GameEventSystem.current.TimeControl(3);
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); 
    }

    public void BackToMain()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu); 
        CanvasManager.Push(GlobalInfor.FrontMenu, null); GameEventSystem.current.TimeControl(3);
    }

    public void Replay()
    {
        BoardManager.instance.RefreshMemory();
        Pop();
        CanvasManager.Pop(GlobalInfor.GamePlayMenu);
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); GameEventSystem.current.TimeControl(3);
    }

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            Level = 1;
        else
        {
            object[] param = initParams;
            Level = (int)param[0];
        }

        //DISPLAY STARS
        _Star.text = BoardManager.instance.star + " Star";
        for(int i = 0; i < 3; i++)
        {
            if(i < BoardManager.instance.star)
            {
                Stars[i].SetActive(true);
            }
            else
            {
                Stars[i].SetActive(false);
            }
        }

        StartCoroutine(PopUpAnimation());

        //DISPLAY COIN
        int tempcoin = Utility.ReadCheckPoint().NumberOfCoin[BoardManager.instance.world - 1];
        _Coin.text = tempcoin + "";
        GameData.I.AddCoin(tempcoin);

        //DATA SAVE
        int worldsave = BoardManager.instance.world; int levelsave = BoardManager.instance.level;

        if (worldsave == GameData.I.GetWorld() && levelsave == GameData.I.GetLevel()) //latest level => SAVE LEVEL
        {
            if (Utility.ReadCheckPoint().NumberOfLevel[worldsave - 1] == levelsave) //end of this check point
            {
                GameData.I.LevelUp(worldsave + 1, 1);
            }
            else
            {
                GameData.I.LevelUp(worldsave, levelsave + 1);
            }
            //SAVE STARS OF LATEST LEVEL

            GameData.I.AddStar(BoardManager.instance.star);
            SaveGameManager.I.Save();
        }
        else //not latest level but still save stars if the its bigger than the previous stars
        {
            GameData.I.AddOldStar(Level, BoardManager.instance.star);
            SaveGameManager.I.Save();
        }
    }

    public Image BG;
    float dur = 0.5f;
    float dur2 = 0.25f;

    IEnumerator PopUpAnimation()
    {
        CanvasManager.Push(GlobalInfor.EmptyMenu, null);
        RectTransform rectTransform = BG.GetComponent<RectTransform>();
        float t = dur;
        rectTransform.localScale *= 0.85f;
        while (t > 0f && rectTransform.localScale.x <= 1.05f && rectTransform.localScale.y <= 1.05f && rectTransform.localScale.z <= 1.05f)
        {
            t -= Time.deltaTime;
            rectTransform.localScale += new Vector3(0.065f, 0.065f, 0) * (t * t / dur);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        t = 0f;
        while (t < dur2 && rectTransform.localScale.x >= 1 && rectTransform.localScale.y >= 1 && rectTransform.localScale.z >= 1)
        {
            t += Time.deltaTime;
            rectTransform.localScale -= new Vector3(0.065f, 0.065f, 0) * (t * t / dur2);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        CanvasManager.Pop(GlobalInfor.EmptyMenu);
    }

    IEnumerator PopOutAnimation()
    {
        CanvasManager.Push(GlobalInfor.EmptyMenu, null);
        RectTransform rectTransform = BG.GetComponent<RectTransform>();
        float t = dur2;
        while (t > 0f && rectTransform.localScale.x <= 1.05f && rectTransform.localScale.y <= 1.05f && rectTransform.localScale.z <= 1.05f)
        {
            t -= Time.deltaTime;
            rectTransform.localScale += new Vector3(0.065f, 0.065f, 0) * (t * t / dur2);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        t = 0f;
        while (t < dur2 && rectTransform.localScale.x >= 1 && rectTransform.localScale.y >= 1 && rectTransform.localScale.z >= 1)
        {
            t += Time.deltaTime;
            rectTransform.localScale -= new Vector3(0.065f, 0.065f, 0) * (t * t / dur2);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        CanvasManager.Pop(GlobalInfor.EmptyMenu);
        Pop();
    }
}
