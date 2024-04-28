using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopup : MonoBehaviour
{
    [SerializeField] public Button openChat;
    [SerializeField] public TextMeshProUGUI chatText = null;
    [SerializeField] public TMP_InputField inputField = null;


    public void openChatHub()
    {
        //ChatBehavior chat = GameObject.FindWithTag("chatLog").GetComponent<ChatBehavior>();
      //  ChatBehavior chat = transform.parent.GetComponent<ChatBehavior>();    
        //openChat.onClick.AddListener(delegate { chat.Send(); });
    }

}
