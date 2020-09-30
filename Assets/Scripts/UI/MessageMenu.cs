using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MessageMenu : BaseUIMenu
{
    public Text Message;
    float dur = 0.25f;

    override
    public void Init(object[] initParams)
    {
        if (initParams == null)
            return;
       
        object[] param = initParams;
        string Mess = (string)param[0];
        Message.text = Mess;
        dur = (float)param[1];

        StartCoroutine(Wait()); 
    }

    IEnumerator Wait()
    {
        RectTransform rectTransform = GetComponent<RectTransform>(); float _x = rectTransform.position.x; float _y = rectTransform.position.y; float _z = rectTransform.position.z;
        float t = 0f;
        rectTransform.position -= new Vector3(0, 75f, 0);
        while (t < dur && rectTransform.position.y <= _y)
        {
            t += Time.deltaTime;
            rectTransform.position += new Vector3(0, 150f, 0) * (t * t / dur);
            yield return null;
        }
        rectTransform.position = new Vector3(_x, _y, _z);
        WaitForSeconds waitTime = new WaitForSeconds(0.25f);
        yield return waitTime;
        Pop();
    }
}
