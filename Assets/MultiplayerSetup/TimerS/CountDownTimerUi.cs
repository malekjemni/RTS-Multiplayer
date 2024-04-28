using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownTimerUi : MonoBehaviour
{
    public TextMeshProUGUI countDownTimerText;
    public TimerSync timerSync;
    public string countDownTimerString = null;


    void Start()
    {
        timerSync = TimerSync.Instance;
        StartCoroutine(UpdateCountDownTimerUI());
    }


    void UpdateCountDownTimerText()
    {
        int minutes = Mathf.FloorToInt(timerSync.countDownTimerValue / 60);
        int seconds = Mathf.FloorToInt(timerSync.countDownTimerValue % 60);
        countDownTimerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        countDownTimerText.text = countDownTimerString;
    }
    IEnumerator UpdateCountDownTimerUI()
    {
        while (true)
        {
            UpdateCountDownTimerText();
            yield return new WaitForSeconds(1f);
        }
    }
}
