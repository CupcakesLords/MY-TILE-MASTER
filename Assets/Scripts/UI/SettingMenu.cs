using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class SettingMenu : BaseUIMenu
{
    public Button Back;
    public Button Skin;
    public GameObject music;
    public GameObject sound;

    bool Music = true;
    bool Sound = true;

    void Start()
    {
        Back.onClick.AddListener(() => BackToFront());
        Skin.onClick.AddListener(() => ToSkinMenu());
        music.GetComponent<Button>().onClick.AddListener(() => SetMusic());
        sound.GetComponent<Button>().onClick.AddListener(() => SetSound());

        music.transform.GetChild(0).GetComponent<Image>().enabled = true;
        sound.transform.GetChild(0).GetComponent<Image>().enabled = true;
        music.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
        sound.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
    }

    public void BackToFront()
    {
        StartCoroutine(PopOutAnimation());
    }

    public void ToSkinMenu()
    {
        CanvasManager.Push(GlobalInfor.SkinMenu, null);
    }

    public void SetMusic()
    {
        if(Music)
        {
            music.transform.GetChild(0).GetComponent<Image>().enabled = false;
            music.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
        }
        else
        {
            music.transform.GetChild(0).GetComponent<Image>().enabled = true;
            music.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
        }
        AudioManager.instance.ChangeMusic();
        Music = !Music;
    }

    public void SetSound()
    {
        if(Sound)
        {
            sound.transform.GetChild(0).GetComponent<Image>().enabled = false;
            sound.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
        }
        else
        {
            sound.transform.GetChild(0).GetComponent<Image>().enabled = true;
            sound.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
        }
        AudioManager.instance.ChangeSound();
        Sound = !Sound;
    }

    override
    public void Init(object[] initParams)
    {
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
