using UnityEngine;
using Mirror;
using Cinemachine;
using System;

public class PlayerControllerMulti : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private ThirdPersonController controller;
    [SerializeField] private CharacterController cc;

    [SerializeField] private GameObject UiPrefab;
    [SerializeField] private GameObject UiChatPrefab;


    void Start()
    {
        if (!isLocalPlayer)
        {           
            return;
        }

        cc.enabled = true;
        controller.enabled = true;

        // Obtenez la r�f�rence au GameObject du personnage
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");      
        mainCamera.GetComponent<Camera>().enabled = true;
        playerCamera.Priority = 10;

       
    }



    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Activer le personnage pour le joueur local

        GameObject ui = Instantiate(UiPrefab);
        ui.name = "UiPlayer";
        ui.transform.SetParent(transform);
        //GameObject uichat = Instantiate(UiChatPrefab);
        //uichat.transform.SetParent(transform);

        GameObject chat = GameObject.FindWithTag("chatLog");
        GetComponent<LootSceneManager>().countDownTimerUi = chat.GetComponentInChildren<CountDownTimerUi>();
        GetComponent<ChatBehavior>().chatText = chat.GetComponent<ChatPopup>().chatText;
        GetComponent<ChatBehavior>().inputField = chat.GetComponent<ChatPopup>().inputField;
        GetComponent<ChatBehavior>().chatButton = chat.GetComponent<ChatPopup>().openChat;
        GetComponent<ChatBehavior>().chatButton.onClick.AddListener(delegate { GetComponent<ChatBehavior>().Send(); });



    }
}
