using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AFramework.UI;

public class MessageMenu : BaseUIMenu
{
    public Text Message;
    float dur;

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
        WaitForSeconds waitTime = new WaitForSeconds(dur);
        yield return waitTime;
        Pop();
    }
}
