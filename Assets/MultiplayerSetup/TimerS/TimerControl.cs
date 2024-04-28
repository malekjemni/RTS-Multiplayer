using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TimerControl : MonoBehaviour
{

    public bool isCountDownTimerRunning = false;
    public void StartCountDownTimer()
    {
         StartCoroutine(SendStartCountDownTimerRequest());      
    }

    public void StopCountDownTimer()
    {
        StartCoroutine(SendStopCountDownTimerRequest());
    }

    IEnumerator SendStartCountDownTimerRequest()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:9090/startCountDownTimer"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                isCountDownTimerRunning = true;
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Countdown timer started successfully");
            }
        }
    }

    IEnumerator SendStopCountDownTimerRequest()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:9090/stopCountDownTimer"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                isCountDownTimerRunning = false;
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Countdown timer stopped successfully");
            }
        }
    }
}
