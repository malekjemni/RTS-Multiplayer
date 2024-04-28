using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    public GameObject powerupSlot;
    public Sprite[] powerupsIcons;
    public TextMeshProUGUI BoostText;
    public GameObject boxText;
    public GameObject slider;
    public CountDownTimerUi countDownTimerUi;


    public TextMeshProUGUI timeText;
    private bool isFinished = false;


    public void SetBoxText(bool _state)
    {
        boxText.SetActive(_state);
    }
    public void SetPowerupIcon(int index,string message,float durations)
    {
        StopCoroutine(BoostTimer(durations));
        powerupSlot.SetActive(true);
        powerupSlot.GetComponentInChildren<Image>().sprite = powerupsIcons[index];
        BoostText.text = message;
        StartCoroutine(BoostTextOff());
        StartCoroutine(BoostTimer(durations));
    }



    IEnumerator BoostTimer(float _durations)
    {
        Slider sliderhand = slider.GetComponent<Slider>();
        slider.SetActive(true);
        sliderhand.maxValue = _durations;
        sliderhand.value = _durations;
        int counter = (int)_durations;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            sliderhand.value = Mathf.Clamp(counter, 0, _durations);

        }
        slider.SetActive(false);
        powerupSlot.SetActive(false);
    }

    IEnumerator BoostTextOff()
    {
        BoostText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        BoostText.gameObject.SetActive(false);
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
