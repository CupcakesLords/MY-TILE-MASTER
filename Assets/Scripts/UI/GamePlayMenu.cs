using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using AFramework;

public class GamePlayMenu : BaseUIMenu
{
    public Button _Pause;
    public Button _Undo; public GameObject undoNumber;
    public Button _Refresh; public GameObject refreshNumber;
    public Button _Hint; public GameObject hintNumber;
    public Text _Level;

    private int Level = 1;
    private float Refresh_Timer = 0;

    private void Awake()
    {
        GameEventSystem.current.onBoosterPurchaseUpdateUI += BoosterUpdate;
    }

    private void OnDestroy()
    {
        GameEventSystem.current.onBoosterPurchaseUpdateUI -= BoosterUpdate;
    }

    void Start()
    {
        _Pause.onClick.AddListener(Pause);
        _Undo.onClick.AddListener(Undo);
        _Refresh.onClick.AddListener(Refresh);
        _Hint.onClick.AddListener(Hint);
    }

    override
    public void Init(object[] initParams) {

        if (initParams == null)
            Level = 1;
        else
        {
            object[] param = initParams;
            Level = (int)param[0];
        }

        _Level.text = "Level " + BoardManager.instance.world + " - " + BoardManager.instance.level;

        if (GameData.I.GetUndo() == 0)
        {
            undoNumber.SetActive(false);
        }
        else
        {
            undoNumber.SetActive(true);
            undoNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetUndo() + "";
        }

        if (GameData.I.GetShuffle() == 0)
        {
            refreshNumber.SetActive(false);
        }
        else
        {
            refreshNumber.SetActive(true);
            refreshNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetShuffle() + "";
        }

        if (GameData.I.GetHint() == 0)
        {
            hintNumber.SetActive(false);
        }
        else
        {
            hintNumber.SetActive(true);
            hintNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetHint() + "";
        }

        BoardManager.instance.StartNewGame(Level);
    }

    public void Pause()
    {
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push(GlobalInfor.PauseMenu, param);
        GameEventSystem.current.TimeControl(1);
    }

    public void Undo()
    {
        if (GameData.I.GetUndo() == 0)
        {
            object[] param = new object[1];
            param[0] = 2;
            CanvasManager.Push(GlobalInfor.StoreMenu, param); GameEventSystem.current.TimeControl(1);
            return;
        }

        bool temp = BoardManager.instance.UndoRecord();

        if (temp == false)
            return;

        GameData.I.UseUndo(); SaveGameManager.I.Save();
        if (GameData.I.GetUndo() == 0)
        {
            undoNumber.SetActive(false);
        }
        else
        {
            undoNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetUndo() + "";
        }
    }

    public void Refresh()
    {
        if (GameData.I.GetShuffle() == 0)
        {
            object[] param = new object[1];
            param[0] = 2;
            CanvasManager.Push(GlobalInfor.StoreMenu, param); GameEventSystem.current.TimeControl(1);
            return;
        }

        if (Refresh_Timer > 0)
            return;
        Refresh_Timer = 1.5f;

        BoardManager.instance.Refresh();

        GameData.I.UseShuffle(); SaveGameManager.I.Save();
        if (GameData.I.GetShuffle() == 0)
        {
            refreshNumber.SetActive(false);
        }
        else
        {
            refreshNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetShuffle() + "";
        }
    }

    public void Hint()
    {
        if (GameData.I.GetHint() == 0)
        {
            object[] param = new object[1];
            param[0] = 2;
            CanvasManager.Push(GlobalInfor.StoreMenu, param); GameEventSystem.current.TimeControl(1);
            return;
        }

        BoardManager.instance.Hint();

        GameData.I.UseHint(); SaveGameManager.I.Save();
        if (GameData.I.GetHint() == 0)
        {
            hintNumber.SetActive(false);
        }
        else
        {
            hintNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetHint() + "";
        }
    }

    void Update()
    {
        if (Refresh_Timer > 0)
            Refresh_Timer -= Time.deltaTime;
    }

    private int BoosterUpdate(int choice)
    {
        if (choice == 1) //undo
        {
            if (GameData.I.GetUndo() == 0)
            {
                undoNumber.SetActive(false);
            }
            else
            {
                undoNumber.SetActive(true);
                undoNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetUndo() + "";
            }
        }
        else if (choice == 2) //shuffle
        {
            if (GameData.I.GetShuffle() == 0)
            {
                refreshNumber.SetActive(false);
            }
            else
            {
                refreshNumber.SetActive(true);
                refreshNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetShuffle() + "";
            }
        }
        else if (choice == 3) //hint
        {
            if (GameData.I.GetHint() == 0)
            {
                hintNumber.SetActive(false);
            }
            else
            {
                hintNumber.SetActive(true);
                hintNumber.transform.GetChild(0).GetComponent<Text>().text = GameData.I.GetHint() + "";
            }
        }
        return 0;
    }
}
