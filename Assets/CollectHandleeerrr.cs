using Mirror;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectHandleeerrr : NetworkBehaviour
{
    public LayerMask layerMask;
  
    //public TextMeshProUGUI messageText; 

    private bool hasCollectedLoot = false;
    public bool HasBoost = false;

    private void Update()
    {
        if (!isLocalPlayer) return; // Ne traitez que sur le joueur local

        if (!hasCollectedLoot)
        {

            // Create a raycast from this object's position forward
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit[] hits;

            // Perform the raycast
            hits = Physics.RaycastAll(ray, 2f, layerMask);

            // Loop through all hits and debug information about each hit
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Lootable"))
                {
                    bool box = hit.collider.gameObject.GetComponent<Animator>().GetBool("Trigger");
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (!box)
                            hit.collider.gameObject.GetComponent<Animator>().SetBool("Trigger", true);
                        if (hit.collider.gameObject.GetComponent<BoxHandler>().isGolden)
                        {
                            ItemSpawner.Instance.Item.GetComponentInChildren<Animator>().SetTrigger("Animationtrigger");
                            TextBlinker.Instance.StartUI();
                            //CmdCollectLoot(); // Appel de la commande pour collecter le loot sur le serveur
                            //break; // Sortir de la boucle dès que le loot est collecté
                            CmdCollectLoot();
                        }
                    }
                }
                else if (hit.collider.CompareTag("Item"))
                {
                    Debug.Log("hit");
                }


            }
          
                    
                
            
        }
        
    }

    [Command]
    private void CmdCollectLoot()
    {
        //if (loot != null)
        //{
        //    hasCollectedLoot = true; // Marquer le loot comme collecté
        //    RpcDisableLoot(loot); // Désactiver le loot sur tous les clients
            RpcShowMessage(); // Afficher un message à tous les joueurs
        //    Debug.Log("Loot collected");
        //}
        //else
        //{
        //    Debug.LogError("Loot object is null");
        //}
    }

    //[ClientRpc]
    //private void RpcDisableLoot(GameObject loot)
    //{
    //    loot.SetActive(false); // Désactiver le loot sur tous les clients
    //}

    [ClientRpc]
    private void RpcShowMessage()
    {
        /* messageText.text = message;*/ // Afficher le message sur tous les clients
        Debug.Log("loot collected");
    }

    public IEnumerator OnBoost()
    {

        HasBoost = true;
        yield return new WaitForSeconds(20);
        HasBoost = false;
    }
};
