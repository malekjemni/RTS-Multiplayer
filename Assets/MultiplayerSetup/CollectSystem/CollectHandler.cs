using UnityEngine;
using Mirror;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Networking;

public class CollectHandler : NetworkBehaviour
{
    public LayerMask layerMask;
    public GameObject effectPrefab;
    private PlayerInputManager _input;
    public bool isNear;


    private void Start()
    {
        _input = GetComponent<PlayerInputManager>();
        _input.interact = false;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray,2f, layerMask);
        isNear = false;

        foreach (RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Lootable"))
            {     
                isNear = true;
                transform.Find("UiPlayer").GetComponent<BoostUI>().SetBoxText(true);
                if (_input.interact && !hit.collider.gameObject.GetComponent<BoxHandler>().isOpened)
                {
                    OpenChest(hit.collider.gameObject);                  
                }
             
            }
            else if(hit.collider.CompareTag("Item"))
            {
               // ConsumeItemLoot(hit.collider.gameObject);
            }          
        }
        if (!isNear)
        {
            transform.Find("UiPlayer").GetComponent<BoostUI>().SetBoxText(false);
        }

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!isLocalPlayer) { return; }
        if (collision.transform.CompareTag("Boost"))
        {
            collision.gameObject.GetComponent<MysteryBox>().CollectBox(gameObject);
        }
    }

    private void OpenChest(GameObject chest)
    {
        if (!isOwned) { return; }
        BoxHandler chestBox = chest.GetComponent<BoxHandler>();
        chestBox.PressedOpenBox();
        GameObject loot = Instantiate(chestBox.LootTable[chestBox.lootIndex], chestBox.holder.position, Quaternion.identity);
        loot.transform.SetParent(chestBox.holder);
        loot.GetComponentInChildren<Animator>().SetBool("Animationtrigger", true);
        StartCoroutine(GetLootItem(loot));
        CmdOpenChest(chest);       
    }

   

    public void SpawnBoostEffect(GameObject box)
    {
        if (!isOwned) { return; }
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        CmdCollectBoostBox(box);
        Destroy(effect, 2f);
    }

    [Command]
    public void CmdCollectBoostBox(GameObject box)
    {
        RpcCollectBoostBox();      
        NetworkServer.Destroy(box);     
    }   

    [ClientRpc]
    public void RpcCollectBoostBox()
    {
        if (isOwned) { return; }
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
    }


    [Command]
    public void CmdOpenChest(GameObject chest)
    {       
        RpcOpenChest(chest);
    }

    [ClientRpc]
    public void RpcOpenChest(GameObject chest)
    {
        if (isOwned) { return; }
        BoxHandler chestBox = chest.GetComponent<BoxHandler>();
        chestBox.PressedOpenBox();
        GameObject loot = Instantiate(chestBox.LootTable[chestBox.lootIndex], chestBox.holder.position, Quaternion.identity);
        loot.transform.SetParent(chestBox.holder);
        loot.GetComponentInChildren<Animator>().SetBool("Animationtrigger", true);
        StartCoroutine(GetLootItem(loot));
    }

    
    IEnumerator GetLootItem(GameObject loot)
    {      
        yield return new WaitForSeconds(4f);
        if (isOwned)
        {
            loot.GetComponent<LootItem>().PressedLootBox();
            GetComponent<ChatBehavior>().ShowChatLog("the item " + loot.GetComponent<LootItem>().itemName);
            if(loot.GetComponent<LootItem>().itemName == "Gold")
            {
                GetComponent<LootSceneManager>().EndLootScene();           
            }
      
        }
        else
        {
            if (loot.GetComponent<LootItem>().itemName == "Gold")
            {
                GetComponent<LootSceneManager>().EndLootScene();
            }
        }
        Destroy(loot);
    }
}
