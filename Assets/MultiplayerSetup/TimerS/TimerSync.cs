using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class TimerSync : MonoBehaviour
{
    private static TimerSync _instance;
    public static TimerSync Instance { get { return _instance; } }

    private WebSocket webSocket;
    public float timerValue = 0f;
    public float countDownTimerValue = 0f;
    public bool isCountDownTimerRunning = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); 
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject); 
    }

    void Start()
    {
        webSocket = new WebSocket("ws://localhost:9090");
        webSocket.OnMessage += OnMessage;
        webSocket.Connect();
    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        var data = JsonUtility.FromJson<TimerUpdateMessage>(e.Data);
        if (data.type == "timerUpdate")
        {
            timerValue = data.value;
        }
        if (data.type == "countDownTimerUpdate")
        {
            if(countDownTimerValue == 0f )
            {
                isCountDownTimerRunning = true;
            }
            countDownTimerValue = data.value;
        }
    }

    private void OnDestroy()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }



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
                Debug.Log("running timer");
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


[System.Serializable]
public class TimerUpdateMessage
{
    public string type;
    public float value;
}
