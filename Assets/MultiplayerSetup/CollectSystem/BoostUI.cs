using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    public GameObject powerupSlot;
    public Sprite[] powerupsIcons;
    public TextMeshProUGUI BoostText;
    public GameObject boxText;
    public GameObject slider;


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
 
}
