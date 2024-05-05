using RtsCam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUpgrade : MonoBehaviour
{
    public GameObject woodStorageUp;
    public GameObject ironStorageUp;
    public GameObject mudStorageUp;
    public GameObject energieStorageUp;
    public UIManager uiManager;
    public GameObject uiElement;
    public RtsCamera rtsCamera;

    public void ToggleUIElement()
    {
        if (uiElement.activeSelf)
        {
            uiElement.SetActive(false);
            rtsCamera.enabled = true;
        }
        else
        {
            uiElement.SetActive(true);
            rtsCamera.enabled = false;
        }
    }
    private void Update()
    {
        if (ResourceManager.Instance.MaxCapacityForWood <= uiManager.playerStorageData.storagewood)
        {
            woodStorageUp.SetActive(true);
        }
        else
        {
            woodStorageUp.SetActive(false);
        }
        if (ResourceManager.Instance.MaxCapacityForIron <= uiManager.playerStorageData.storageclay)
        {
            ironStorageUp.SetActive(true);
        }
        else
        {
            ironStorageUp.SetActive(false);
        }
        if (ResourceManager.Instance.MaxCapacityForMud <= uiManager.playerStorageData.storagemud)
        {
            mudStorageUp.SetActive(true);
        }
        else
        {
            mudStorageUp.SetActive(false);
        }   
        if (ResourceManager.Instance.MaxCapacityForETypeSolaire <= uiManager.playerStorageData.storageenergie)
        {
            energieStorageUp.SetActive(true);
        }
        else
        {
            energieStorageUp.SetActive(false);
        }

    }

    public void UpgradeWoodStorage()
    {
        ResourceManager.Instance.UpgradeResourceStorage(ResourceType.Wood, 5000);
        uiManager.UpdateGoldOnServer(50);
    }
    public void UpgradeIronStorage()
    {
        ResourceManager.Instance.UpgradeResourceStorage(ResourceType.Iron, 5000);
        uiManager.UpdateGoldOnServer(50);
    }
    public void UpgradeMudStorage()
    {
        ResourceManager.Instance.UpgradeResourceStorage(ResourceType.Mud, 5000);
        uiManager.UpdateGoldOnServer(50);
    }
    public void UpgradeEnergieStorage()
    {
        ResourceManager.Instance.UpgradeResourceStorage(ResourceType.ETypeSolaire, 5000);
        uiManager.UpdateGoldOnServer(50);
    }
}
