using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LootSceneManager : NetworkBehaviour
{
    public CountDownTimerUi countDownTimerUi;
    private bool isFinished = false;

    private void Start()
    {
        GameObject timer = GameObject.FindWithTag("chatLog");
        countDownTimerUi = timer.GetComponentInChildren<CountDownTimerUi>();
    }

    private void Update()
    {
        if (countDownTimerUi.countDownTimerString.Equals("00:00") && !isFinished && TimerSync.Instance.isCountDownTimerRunning)
        {
            EndLootScene();
            isFinished = true;
        }
    }
    public void EndLootScene()
    {
        StartCoroutine(DeleteRooms());
    }
    public IEnumerator DeleteRooms()
    {
        string url = "http://127.0.0.1:9090/rooms/clear";
        UnityWebRequest request = UnityWebRequest.Delete(url);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
            TimerSync.Instance.StopCountDownTimer();
            SceneManager.LoadScene("TasskNeder", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError($"Failed to delete all rooms: {request.error}");
        }
    }
}
