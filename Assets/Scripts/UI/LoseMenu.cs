using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class LoseMenu : BaseUIMenu
{
    public Button _Revive;
    public Button _Replay;
    public Button _Back;
    public Text _Level;
    private int Level = 1;

    void Start()
    {
        _Back.onClick.AddListener(BackToMain);
        _Replay.onClick.AddListener(Replay);
        _Revive.onClick.AddListener(Revive);
    }

    public void Revive()
    {
        BoardManager.instance.bar.Lighten();
        Pop();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.RewindLost(); GameEventSystem.current.TimeControl(2);
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

        GameEventSystem.current.TimeControl(3);
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param); 
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

        _Level.text = BoardManager.instance.world + " - " + BoardManager.instance.level;
        StartCoroutine(PopUpAnimation());
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
