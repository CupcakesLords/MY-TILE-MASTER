using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OldMenu : MonoBehaviour
{
    public GameObject WinUI;
    public GameObject LoseUI;
    public Button _Undo;
    public Button _Refresh;
    public Button _Hint;
    public Text _Level;
    private float Refresh_Timer = 0;

    private void Update()
    {
        if (Refresh_Timer > 0)
            Refresh_Timer -= Time.deltaTime;
    }

    private int PopUpWin(int UI)
    {
        if (UI == 1)
        {
            WinUI.SetActive(true); _Undo.interactable = false; _Refresh.interactable = false; _Hint.interactable = false;
        }
        else if(UI == 0)
        {
            LoseUI.SetActive(true); _Undo.interactable = false; _Refresh.interactable = false; _Hint.interactable = false;
            Time.timeScale = 0;
        }
        return 0;
    }

    public void Reload()
    {
        WinUI.SetActive(false); Time.timeScale = 1; _Undo.interactable = true; _Refresh.interactable = true; _Hint.interactable = true;
        LoseUI.SetActive(false);
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void NextLevel()
    {
        WinUI.SetActive(false); Time.timeScale = 1; _Undo.interactable = true; _Refresh.interactable = true; _Hint.interactable = true;
        LoseUI.SetActive(false); 
        GlobalStatic.PlayerChoice += 1;
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void Revive()
    {
        WinUI.SetActive(false); Time.timeScale = 1; _Undo.interactable = true; _Refresh.interactable = true; _Hint.interactable = true;
        LoseUI.SetActive(false); 
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

    public void Refresh()
    {
        if (Refresh_Timer > 0)
            return;
        Refresh_Timer = 1.5f;
        BoardManager.instance.Refresh();
    }

    public void Hint()
    {
        BoardManager.instance.Hint();
    }

    void Start()
    {
        GameEventSystem.current.onLevelClear += PopUpWin;
        _Level.text = "Level " + GlobalStatic.PlayerChoice;
    }

    void OnDestroy()
    {
        GameEventSystem.current.onLevelClear -= PopUpWin;
    }
}
