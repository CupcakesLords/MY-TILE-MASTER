using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject WinUI;
    public GameObject LoseUI;

    private int PopUpWin(int UI)
    {
        if (UI == 1)
        {
            WinUI.SetActive(true);
        }
        else if(UI == 0)
        {
            LoseUI.SetActive(true);
        }
        return 0;
    }

    public void Reload()
    {
        WinUI.SetActive(false);
        LoseUI.SetActive(false);
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void Undo()
    {
        BoardManager.instance.UndoRecord();
    }

    void Start()
    {
        GameEventSystem.current.onLevelClear += PopUpWin;
    }

    void OnDestroy()
    {
        GameEventSystem.current.onLevelClear -= PopUpWin;
    }
}
