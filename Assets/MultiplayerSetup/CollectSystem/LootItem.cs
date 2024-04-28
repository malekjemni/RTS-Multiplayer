using Mirror;
using UnityEngine;

public class LootItem : NetworkBehaviour
{

    public string itemName;
    public bool isLooted = false;


    public void PressedLootBox()
    {
        if(!isLooted)
        {
            isLooted = true;                     
        }
            
    }
}
