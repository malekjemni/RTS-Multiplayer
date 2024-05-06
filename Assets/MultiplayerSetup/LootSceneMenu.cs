using Mirror;
using TMPro;
using UnityEngine;

public class LootSceneMenu : MonoBehaviour
{

    public GameObject uiElement;
    public TextMeshProUGUI usernameText;
    private void Start()
    {
        usernameText.text = CurrentUserManager.Instance.GetCurrentUsername();
    }   

    public void ToggleUIElement()
    {
        if (uiElement.activeSelf)
        {
            uiElement.SetActive(false);
        }
        else
        {
            uiElement.SetActive(true);
        }
    }
    public void BackToMainGame()
    {
        uiElement.SetActive(false);
        // NetworkManager.singleton.StopClient();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
