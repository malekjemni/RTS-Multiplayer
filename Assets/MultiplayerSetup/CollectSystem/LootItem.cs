using Mirror;
using UnityEngine;

public class LootItem : NetworkBehaviour
{

    public string itemName;
    public int resourceAmount;
    public ResourceType resourceType;
    public bool isLooted = false;
    public bool isGolden = false;


    public void PressedLootBox()
    {
        if(!isLooted)
        {
            isLooted = true;                     
        }
            
    }
}
