using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeValue = 5f;
    public TMP_Text timerText;

    // Update is called once per frame
    void Update()
    {
        /*if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
        }
        DisplayTime(timeValue);*/
        if (timerText.text == "4:100" || timerText.text == "4:99")
        {
            timerText.gameObject.SetActive(false);
        }
    }

    public void DisplayTime(float timeToDisplay, bool isDecaying)
    {
        timerText.gameObject.SetActive(true);
        Debug.Log(timeToDisplay);
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliseconds = timeToDisplay % 1 * 100;

        //timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        timerText.text = string.Format("{0:0}:{1:00}", seconds, milliseconds);

        if (isDecaying)
        {
            timerText.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            timerText.color = new Color32(0, 255, 0, 255);
        }
    }
}
