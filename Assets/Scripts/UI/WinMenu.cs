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
            WinUI.SetActive(true); Time.timeScale = 0;
        }
        else if(UI == 0)
        {
            LoseUI.SetActive(true); Time.timeScale = 0;
        }
        return 0;
    }

    public void Reload()
    {
        WinUI.SetActive(false); Time.timeScale = 1;
        LoseUI.SetActive(false); Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void NextLevel()
    {
        WinUI.SetActive(false); Time.timeScale = 1;
        LoseUI.SetActive(false); Time.timeScale = 1;
        GlobalStatic.PlayerChoice += 1;
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void Revive()
    {
        WinUI.SetActive(false); Time.timeScale = 1;
        LoseUI.SetActive(false); Time.timeScale = 1;
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
        BoardManager.instance.UndoRecord();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
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
