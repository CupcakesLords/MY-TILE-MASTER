using System.Collections;
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

    private int World = -100;
    private int NumberOfLevel;
    private int StartLevel;

    private float x;
    private float y;
    private float gap = 100; //200

    private GameObject[,] buttons;

    void Awake()
    {
        x = BG.transform.position.x - BG.rectTransform.sizeDelta.x * 0.35f;
        y = BG.transform.position.y + BG.rectTransform.sizeDelta.y * 0.35f;

        //x = BG.transform.position.x - BG.rectTransform.sizeDelta.x * 0.7f;
        //y = BG.transform.position.y + BG.rectTransform.sizeDelta.y * 0.65f;

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
        Pop();
    }

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            return;

        object[] param = initParams;

        if ((int)param[0] != World) //different world
        {
            World = (int)param[0];
            NumberOfLevel = (int)param[1];
            StartLevel = (int)param[2];

            DeleteButton();
            DrawButton();
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

                    buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = level.ToString();
                    buttons[k, l].transform.GetChild(1).gameObject.SetActive(false);
                    buttons[k, l].GetComponent<Button>().interactable = true;

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
                buttons[k, l].transform.SetParent(canvas.transform, false);

                int level = StartLevel + k + buttons.GetLength(0) * l;

                buttons[k, l].transform.position = new Vector3(x + k * gap, y - l * gap, 0);
                buttons[k, l].GetComponent<Button>().GetComponentInChildren<Text>().text = level.ToString();
                buttons[k, l].GetComponent<Button>().onClick.AddListener(() => GotoActionPhase(level));

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
}
