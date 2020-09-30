using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using System;

public class SkinMenu : BaseUIMenu
{
    public Button Back;
    private int currentToggle = 1;
    private bool first = true;

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
        if (first)
        {
            first = false; return;
        }
        
        if (ID == currentToggle)
            return;

        if(ID == 1)
        {
            CanvasManager.Pop(GlobalInfor.ThemeCollectionSubMenu);
            CanvasManager.Push(GlobalInfor.SkinCollectionSubMenu, null);
        }
        else if(ID == 2)
        {
            CanvasManager.Pop(GlobalInfor.SkinCollectionSubMenu);
            CanvasManager.Push(GlobalInfor.ThemeCollectionSubMenu, null);
        }

        currentToggle = ID;
    }

    override
    public void Init(object[] initParams)
    {
        StartCoroutine(PopUpAnimation());
        if (currentToggle == 1)
        {
            CanvasManager.Push(GlobalInfor.SkinCollectionSubMenu, null); GameEventSystem.current.SkinControl(true);
        }
        else if (currentToggle == 2)
        {
            CanvasManager.Push(GlobalInfor.ThemeCollectionSubMenu, null); GameEventSystem.current.ThemeControl(true);
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
        if (currentToggle == 1)
            GameEventSystem.current.SkinControl(false);
        else if (currentToggle == 2)
            GameEventSystem.current.ThemeControl(false);
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
            CanvasManager.Pop(GlobalInfor.SkinCollectionSubMenu);
        else if (currentToggle == 2)
            CanvasManager.Pop(GlobalInfor.ThemeCollectionSubMenu);
    }
}
