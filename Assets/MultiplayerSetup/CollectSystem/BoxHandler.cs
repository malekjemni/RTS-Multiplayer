using Mirror;
using UnityEngine;

public class BoxHandler : NetworkBehaviour
{
    public GameObject[] LootTable;
    public Transform holder;

    public GameObject Item;

    public bool isGolden { get; set; } = false;
    [SyncVar]
    public bool isOpened = false;
    [SyncVar]
    public int lootIndex;

    public void PressedOpenBox()
    {
        if (!isOpened)
        {
            int randomIndex = Random.Range(0, LootTable.Length);
            isGolden = true;
            lootIndex = randomIndex;
            GetComponentInChildren<BoxCollider>().enabled = false;
            GetComponent<Animator>().SetBool("Trigger", true);          
            isOpened = true;
        }
       
    }
 

}
