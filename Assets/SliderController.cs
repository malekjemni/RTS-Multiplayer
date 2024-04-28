using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public static SliderController Instance;

   

    public GameObject slider;


    private void Awake()
    {
        Instance = this;
    }
   

 
    void Update()
    {

       // UpdateSliderValue(sliderEnergy, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeSolaire), ResourceManager.Instance.MaxCapacityForETypeSolaire);

    }

    public void StartTimer()
    {
        StartCoroutine("BoostTimer");
    }


    IEnumerator BoostTimer()
    {
       Slider sliderhand=slider.GetComponent<Slider>();
        slider.SetActive(true);
        sliderhand.maxValue = 20;
        sliderhand.value = 20;
        int counter=20;
        while(counter>0)
        {
        yield return new WaitForSeconds(1);
            counter--;
            sliderhand.value = Mathf.Clamp(counter, 0, 20);
           
        }
        slider.SetActive(false);
    }
      



}





