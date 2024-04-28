using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChatBehavior : NetworkBehaviour
{

     public TextMeshProUGUI chatText = null;
     public TMP_InputField inputField = null;
     public Button chatButton = null;


    private static event Action<string> OnMessage;
    private static event Action<string> OnTimeChange;

    public override void OnStartAuthority()
    {
        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) { return; }

        OnMessage -= HandleNewMessage;
    }


    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }



    [Client]
    public void Send()
    {
         if (!isLocalPlayer) { return; }
        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }
        CmdSendMessage($"[{CurrentUserManager.Instance.GetCurrentUsername()}]:" + inputField.text);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage(message);
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }


    [Client]
    public void ShowChatLog(string message)
    {
        if (!isLocalPlayer) { return; }
        CmdSendChatLog($"[{CurrentUserManager.Instance.GetCurrentUsername()}]: Recieved " + message);
    }

    [Command]
    private void CmdSendChatLog(string message)
    {
        RpcHandleChatLog(message);
    }

    [ClientRpc]
    private void RpcHandleChatLog(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

    [Client]
    public void showTimer(string message)
    {
        if (!isLocalPlayer) { return; }
        CmdshowTimer(message);
    }

    [Command]
    private void CmdshowTimer(string message)
    {
        RpcshowTimer(message);
    }

    [ClientRpc]
    private void RpcshowTimer(string message)
    {
        OnTimeChange?.Invoke(message);
    }
}
