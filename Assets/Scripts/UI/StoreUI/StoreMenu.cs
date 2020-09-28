using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework.UI;
using UnityEngine.UI;

public class StoreMenu : BaseUIMenu
{
    public Button Back;
    private int currentToggle = 1;
    public GameObject store;
    public GameObject coin;
    bool first = true;
    int prev = 1;

    void Start()
    {
        Back.onClick.AddListener(() => BackToMap()); 
    }

    private void BackToMap()
    {
        Pop();
        if (currentToggle == 1)
            CanvasManager.Pop(GlobalInfor.StoreSubStoreMenu);
        else if (currentToggle == 2)
            CanvasManager.Pop(GlobalInfor.StoreSubCoinMenu);

        if (BoardManager.instance.AlreadyFirstMatchDestroyed())
            GameEventSystem.current.TimeControl(2);
    }

    public void Toggle(int ID)
    {
        if (ID == currentToggle)
            return;

        if (ID == 2)
        {
            CanvasManager.Pop(GlobalInfor.StoreSubStoreMenu);
            CanvasManager.Push(GlobalInfor.StoreSubCoinMenu, null);
        }
        else if (ID == 1)
        {
            CanvasManager.Pop(GlobalInfor.StoreSubCoinMenu);
            CanvasManager.Push(GlobalInfor.StoreSubStoreMenu, null);
        }

        currentToggle = ID;
    }

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            currentToggle = 1;
        else
        {
            prev = currentToggle;
            object[] param = initParams;
            currentToggle = (int)param[0]; //1 or 2
        }

        if (currentToggle == 1)
        {
            store.GetComponent<Toggle>().isOn = true;
            if (first || prev == currentToggle)
            {
                first = false;
                CanvasManager.Push(GlobalInfor.StoreSubStoreMenu, null);
            }
        }
        else if (currentToggle == 2)
        {
            coin.GetComponent<Toggle>().isOn = true;
            if (first || prev == currentToggle)
            {
                first = false;
                CanvasManager.Push(GlobalInfor.StoreSubCoinMenu, null);
            }
        }
    }
}
