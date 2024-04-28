using UnityEngine;
using TMPro;
using System.Collections;

public class TimerUi : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TimerSync timerSync;
    public string timerString = null;

    void Start()
    {
        timerSync = TimerSync.Instance;
        StartCoroutine(UpdateTimerUI());
    }


    void UpdateTimerText()
    {     
        int minutes = Mathf.FloorToInt(timerSync.timerValue / 60);
        int seconds = Mathf.FloorToInt(timerSync.timerValue % 60);
        timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timerString;

    }
    IEnumerator UpdateTimerUI()
    {
        while (true)
        {
            UpdateTimerText(); 
            yield return new WaitForSeconds(1f); 
        }
    }

}
