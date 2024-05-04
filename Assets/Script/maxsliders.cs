using UnityEngine;
using UnityEngine.UI;

public class maxsliders : MonoBehaviour
{
    public static maxsliders Instance;
    public Image sliderWood;
    public Image sliderIron;
    public Image sliderMud;
    public Image sliderEnergy;


    private void Awake()
    {
        Instance = this;
    }
    private void UpdateSliderValue(Image fillImage, int currentAmount, int maxCapacity)
    {
        float fillAmount = (float)currentAmount / maxCapacity;
        fillAmount = Mathf.Clamp01(fillAmount);
        fillImage.fillAmount = fillAmount;
    }

    void Update()
    {
    
        UpdateSliderValue(sliderWood, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Wood), ResourceManager.Instance.MaxCapacityForWood);
        UpdateSliderValue(sliderIron, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Iron), ResourceManager.Instance.MaxCapacityForIron);
        UpdateSliderValue(sliderMud, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Mud), ResourceManager.Instance.MaxCapacityForMud);
        UpdateSliderValue(sliderEnergy, ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeSolaire), ResourceManager.Instance.MaxCapacityForETypeSolaire);

    }

}



