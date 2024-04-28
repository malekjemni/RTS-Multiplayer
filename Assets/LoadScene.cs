using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{


    public void LoadRegisterScene()
    {
        SceneManager.LoadScene("NewRegister");
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("NewLogin");
    }

}
