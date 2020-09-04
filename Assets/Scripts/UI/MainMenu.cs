using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MainMenu : BaseUIMenu
{
    public Button GamePlayButton;
    public GameObject buttonPrefab;
    public GameObject canvas;

    //private int Level;
    //private int Number;

    void Start()
    {
        GamePlayButton.onClick.AddListener(() => GotoActionPhase(int.Parse(GamePlayButton.GetComponentInChildren<Text>().text)));
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.transform.SetParent(canvas.transform, false);
        float x = GamePlayButton.transform.position.x;
        float y = GamePlayButton.transform.position.y;
        float z = GamePlayButton.transform.position.z;
        newButton.transform.position = new Vector3(x + 200, y, z);
    }
    
    void GotoActionPhase(int Level)
    {
        Pop();
        object[] param = new object[1];
        param[0] = Level;
        CanvasManager.Push("GamePlayMenu", param);
    }
}
