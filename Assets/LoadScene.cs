using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject registerScreen;


    public void LoadRegisterScene()
    { 
        loginScreen.SetActive(false);
        registerScreen.SetActive(true);
    }

    public void LoadLoginScene()
    {
        loginScreen.SetActive(true);
        registerScreen.SetActive(false);
    }

}
