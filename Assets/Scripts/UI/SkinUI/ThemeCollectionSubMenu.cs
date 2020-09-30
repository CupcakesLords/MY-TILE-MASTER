using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;
using System;

public class ThemeCollectionSubMenu : BaseUIMenu
{
    private List<Button> themes = new List<Button>();
    public List<GameObject> Images;
    private int last_chosen = 1;

    private void Start()
    {
        Images[last_chosen - 1].SetActive(true);
    }

    public void ChangeTheme(int i)
    {
        if (i == last_chosen)
            return;

        Images[i - 1].SetActive(true);
        Images[last_chosen - 1].SetActive(false);

        BoardManager.instance.ResetCharacter(i);
        last_chosen = i;
    }

    private void Awake()
    {
        GameEventSystem.current.onThemeControl += Animate;
    }

    private void OnDestroy()
    {
        GameEventSystem.current.onThemeControl -= Animate;
    }

    private int Animate(bool popup)
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
