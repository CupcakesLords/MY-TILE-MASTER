using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public Image mask;
    public List<Image> Star = new List<Image>();
    public List<Image> Filling = new List<Image>();
    float originalSize;
    float time = 60f;
    float current = 60f;
    List<float> star = new List<float>();
    int currentStar = 2;

    void Awake()
    {
        originalSize = mask.rectTransform.rect.width;
        float width = GetComponent<RectTransform>().rect.width;
        star.Add(15f); star.Add(30f); star.Add(45f);

        Star[0].rectTransform.anchoredPosition = new Vector3((star[0] / time) * width - (width / 2), Star[0].rectTransform.anchoredPosition.y, 0);
        Star[1].rectTransform.anchoredPosition = new Vector3((star[1] / time) * width - (width / 2), Star[1].rectTransform.anchoredPosition.y, 0);
        Star[2].rectTransform.anchoredPosition = new Vector3((star[2] / time) * width - (width / 2), Star[2].rectTransform.anchoredPosition.y, 0);

        GameEventSystem.current.onTimeControl += Control;
        GameEventSystem.current.onSetTimeBar += Set;
    }

    private int Set(float total, float star1, float star2, float star3)
    {
        time = total; current = total;
        star.Clear();

        float width = GetComponent<RectTransform>().rect.width;
        star.Add(star1); star.Add(star2); star.Add(star3);

        Star[0].rectTransform.anchoredPosition = new Vector3((star[0] / time) * width - (width / 2), Star[0].rectTransform.anchoredPosition.y, 0);
        Star[1].rectTransform.anchoredPosition = new Vector3((star[1] / time) * width - (width / 2), Star[1].rectTransform.anchoredPosition.y, 0);
        Star[2].rectTransform.anchoredPosition = new Vector3((star[2] / time) * width - (width / 2), Star[2].rectTransform.anchoredPosition.y, 0);

        return 0;
    }

    private int Control(int action)
    {
        if (action == 1)
            Pause();
        else if (action == 2)
            Resume();
        else if (action == 3)
            Restart();
        return 0;
    }

    bool onPause = false;

    public void Pause()
    {
        onPause = true;
    }

    public void Resume()
    {
        onPause = false;
    }

    public void Restart()
    {
        onPause = false; mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize);
        current = time; Filling[0].enabled = true; Filling[1].enabled = true; Filling[2].enabled = true; currentStar = 2;
    }

    void Update()
    {
        if (onPause)
            return;
        if (current > 0)
        {
            current = current - Time.deltaTime;
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * (current / time));

            if(currentStar >= 0)
            {
                if(current- star[currentStar] <= 0.01 || star[currentStar] - current >= 0.01)
                {
                    Filling[currentStar].enabled = false;
                    currentStar = currentStar - 1;

                    BoardManager.instance.star = currentStar + 1;
                }
            }
        }
    }

    private void OnDestroy()
    {
        GameEventSystem.current.onTimeControl -= Control;
        GameEventSystem.current.onSetTimeBar -= Set;
    }
}
