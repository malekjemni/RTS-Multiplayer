using System.Collections;
using UnityEngine;
using Mirror;

public class MysteryBox : NetworkBehaviour
{
    public GameObject effectPrefab;

    [SyncVar]
    public GameObject player;
    private GameObject playerUi;
    private PlayerModifier playerModifier;
    public bool isConsumed = false;

    public void CollectBox(GameObject _player)
    {
        if (!isConsumed)
        {
            player = _player;
            playerUi = _player.transform.Find("UiPlayer").gameObject;
            playerModifier = player.GetComponent<PlayerModifier>();
            MisteryEffetc();
            player.GetComponent<CollectHandler>().SpawnBoostEffect(gameObject);
            isConsumed = true;
        }
    }

    public void MisteryEffetc()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                playerUi.GetComponent<BoostUI>().SetPowerupIcon(0, "Movement Speed!", playerModifier.speedDuration);
                player.GetComponent<ChatBehavior>().ShowChatLog("the boost : Movement Speed");
                playerModifier.AddSpeed();
                break;

            case 2:
                playerUi.GetComponent<BoostUI>().SetPowerupIcon(1, "Players Stunned!",playerModifier.stunDuration);
                player.GetComponent<ChatBehavior>().ShowChatLog("the boost : Stunned");
                playerModifier.Stun();
                break;

            default:
                playerUi.GetComponent<BoostUI>().SetPowerupIcon(2, "Double Jump!",playerModifier.jumpDuration);
                player.GetComponent<ChatBehavior>().ShowChatLog("the boost : Double Jump");
                playerModifier.AddJump();
                break;
        }
    }

}
