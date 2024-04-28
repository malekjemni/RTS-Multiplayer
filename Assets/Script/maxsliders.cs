using UnityEngine;
using UnityEngine.UI;

public class maxsliders : MonoBehaviour
{
    public static maxsliders Instance;
    public Slider sliderWood;
    public Slider sliderIron;
    public Slider sliderMud;
    public Slider sliderEnergy;

    private void Awake()
    {
        Instance = this;
    }
    private void UpdateSliderValue(Slider slider, int currentAmount, int maxCapacity)
    {
        slider.value = Mathf.Clamp(currentAmount, 0, maxCapacity);
        slider.maxValue = maxCapacity;
    }

     void Update()
    {
    
        UpdateSliderValue(sliderWood, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Wood), ResourceManager.Instance.MaxCapacityForWood);
        UpdateSliderValue(sliderIron, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Iron), ResourceManager.Instance.MaxCapacityForIron);
        UpdateSliderValue(sliderMud, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Mud), ResourceManager.Instance.MaxCapacityForMud);
        UpdateSliderValue(sliderEnergy, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeSolaire), ResourceManager.Instance.MaxCapacityForETypeSolaire);

    }

}



