﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MainMenu : BaseUIMenu
{
    public Button Exit;
    public GameObject buttonPrefab;
    public GameObject canvas;
    public Image BG;
    public Text Star;

    private int World = -100;
    private int NumberOfLevel;
    private int StartLevel;

    private float x;
    private float y;
    private float gap = 125; //200 //140

    private GameObject[,] buttons;

    void Awake()
    {
        //x = BG.transform.position.x - BG.rectTransform.sizeDelta.x * 0.425f;
        //y = BG.transform.position.y + BG.rectTransform.sizeDelta.y * 0.35f;

        x = BG.transform.position.x - BG.rectTransform.sizeDelta.x * 0.275f; //0.3f
        y = BG.transform.position.y + BG.rectTransform.sizeDelta.y * 0.2f;

        Exit.onClick.AddListener(() => PopOut());
    }
    
    void GotoActionPhase(int Level)
    {
        Pop();
        object[] param = new object[1];
        param[0] = Level;

        BoardManager.instance.world = World;
        BoardManager.instance.level = Level - StartLevel + 1;

        CanvasManager.Pop(GlobalInfor.MapMenu); 
        CanvasManager.Push(GlobalInfor.GamePlayMenu, param);
    }

    public void PopOut()
    {
        StartCoroutine(PopOutAnimation());
    }

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            return;

        object[] param = initParams;

        Star.text = (string)param[3];

        if ((int)param[0] != World) //different world
        {
            World = (int)param[0];
            NumberOfLevel = (int)param[1];
            StartLevel = (int)param[2];

            DeleteButton();
            DrawButton();

            StartCoroutine(PopUpAnimation());
        }
        else //same world, but needs to update process
        {
            if (buttons == null)
                return;
           
            for (int k = 0; k < buttons.GetLength(0); k++)
            {
                for (int l = 0; l < buttons.GetLength(1); l++)
                {
                    int level = StartLevel + k + buttons.GetLength(0) * l;

                    buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = (k + buttons.GetLength(0) * l + 1).ToString();
                    buttons[k, l].transform.GetChild(1).gameObject.SetActive(false);
                    buttons[k, l].GetComponent<Button>().interactable = true;

                    if (level - 1 < GameData.I.GetStar().Count) //load star
                    {
                        for (int i = 2; i < 5; i++)
                        {
                            buttons[k, l].transform.GetChild(i).gameObject.SetActive(true);
                        }
                        for (int i = 2; i < 2 + GameData.I.GetStar()[level - 1]; i++)
                        {
                            buttons[k, l].transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                        }
                    }

                    if (World == GameData.I.GetWorld()) //lock levels
                    {
                        if (k + buttons.GetLength(0) * l >= GameData.I.GetLevel())
                        {
                            buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = "";
                            buttons[k, l].transform.GetChild(1).gameObject.SetActive(true);
                            buttons[k, l].GetComponent<Button>().interactable = false;
                        }
                    }
                }
            }

            StartCoroutine(PopUpAnimation());
        }
    }

    private void DrawButton()
    {
        int rows = NumberOfLevel / 5;
        buttons = new GameObject[5, rows];
        for (int k = 0; k < buttons.GetLength(0); k++)
        {
            for (int l = 0; l < buttons.GetLength(1); l++)
            {
                buttons[k, l] = Instantiate(buttonPrefab) as GameObject;
                //buttons[k, l].transform.SetParent(canvas.transform, false);
                buttons[k, l].transform.SetParent(BG.transform, false);

                int level = StartLevel + k + buttons.GetLength(0) * l;

                buttons[k, l].transform.position = new Vector3(x + k * gap, y - l * gap, 0);
                buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = (k + buttons.GetLength(0) * l + 1).ToString();
                buttons[k, l].GetComponent<Button>().onClick.AddListener(() => GotoActionPhase(level));

                if(level - 1 < GameData.I.GetStar().Count) //load star
                {
                    for (int i = 2; i < 5; i++)
                    {
                        buttons[k, l].transform.GetChild(i).gameObject.SetActive(true);
                    }
                    for (int i = 2; i < 2 + GameData.I.GetStar()[level - 1]; i++)
                    {
                        buttons[k, l].transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                    }
                }

                if (World == GameData.I.GetWorld()) //lock levels
                {
                    if (k + buttons.GetLength(0) * l >= GameData.I.GetLevel()) 
                    {
                        buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = "";
                        buttons[k, l].transform.GetChild(1).gameObject.SetActive(true);
                        buttons[k, l].GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }

    private void DeleteButton()
    {
        if (buttons == null) return;
        for (int k = 0; k < buttons.GetLength(0); k++)
            for (int l = 0; l < buttons.GetLength(1); l++)
                Destroy(buttons[k, l]);
        buttons = null;
    }

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
