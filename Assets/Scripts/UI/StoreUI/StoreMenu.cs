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
        StartCoroutine(PopOutAnimation());
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
            if (first || prev == currentToggle)
            {
                first = false; 
                CanvasManager.Push(GlobalInfor.StoreSubStoreMenu, null); 
            }
            store.GetComponent<Toggle>().isOn = true;
            GameEventSystem.current.onStoreStoreControl(true);
        }
        else if (currentToggle == 2)
        {
            if (first || prev == currentToggle)
            {
                first = false; 
                CanvasManager.Push(GlobalInfor.StoreSubCoinMenu, null); 
            }
            coin.GetComponent<Toggle>().isOn = true;
            GameEventSystem.current.onStoreCoinControl(true);
        }
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
        if (currentToggle == 1)
            GameEventSystem.current.onStoreStoreControl(false);
        else if (currentToggle == 2)
            GameEventSystem.current.onStoreCoinControl(false);

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
        if (currentToggle == 1)
            CanvasManager.Pop(GlobalInfor.StoreSubStoreMenu);
        else if (currentToggle == 2)
            CanvasManager.Pop(GlobalInfor.StoreSubCoinMenu);

        if (BoardManager.instance.AlreadyFirstMatchDestroyed())
            GameEventSystem.current.TimeControl(2);
    }
}
