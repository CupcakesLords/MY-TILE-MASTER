using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFramework.UI;
using UnityEngine.UI;
using AFramework;

public class StoreSubCoinMenu : BaseUIMenu
{
    private void Awake()
    {
        GameEventSystem.current.onStoreCoinControl += PopUp;
    }

    private void OnDestroy()
    {
        GameEventSystem.current.onStoreCoinControl -= PopUp;
    }

    private int PopUp(bool popup)
    {
        if (popup)
        {
            StartCoroutine(PopUpAnimation());
        }
        else
        {
            StartCoroutine(PopOutAnimation());
        }
        return 0;
    }

    public Text coins;

    override
    public void Init(object[] initParams)
    {
        coins.text = GameData.I.GetCoin() + "";
    }

    public void Buy(int choice)
    {
        if(GameData.I.GetCoin() < 100)
        {
            object[] param1 = new object[2];
            param1[0] = "Not enough money!";
            param1[1] = 1f;
            CanvasManager.Push(GlobalInfor.MessageMenu, param1);
            return;
        }
        
        if(choice == 1) //undo
        {
            GameData.I.BuyUndo();
        }
        else if (choice == 2) //shuffle
        {
            GameData.I.BuyShuffle();
        }
        else if (choice == 3) //hint
        {
            GameData.I.BuyHint();
        }

        //GameData.I.MinusCoin(100);
        SaveGameManager.I.Save();
        coins.text = GameData.I.GetCoin() + "";

        if(BoardManager.instance.IsPlaying())
        {
            GameEventSystem.current.BoosterPurchaseUpdateUI(choice);
        }

        object[] param = new object[2];
        param[0] = "Purchase successful!";
        param[1] = 1f;
        CanvasManager.Push(GlobalInfor.MessageMenu, param);
    }

    float dur = 0.5f;
    float dur2 = 0.25f;

    IEnumerator PopUpAnimation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
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
    }

    IEnumerator PopOutAnimation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
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
        
        Pop();
    }
}
