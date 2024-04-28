
using UnityEngine;
using UnityEngine.UI;

public class HidePassword : MonoBehaviour
{
    public InputField password;
    void Start()
    {
     
        password.contentType = InputField.ContentType.Password;

    }
}
