using UnityEngine;

public class ProductionPanel : MonoBehaviour
{
    public GameObject uiElement;
    public RectTransform button;

    public void ToggleUIElement()
    {
        if (uiElement.activeSelf)
        {
            uiElement.SetActive(false);
            button.sizeDelta = new Vector2(70, 70);
        }
        else
        {
            uiElement.SetActive(true);
            button.sizeDelta = new Vector2(35, 35);
        }
    }
}
