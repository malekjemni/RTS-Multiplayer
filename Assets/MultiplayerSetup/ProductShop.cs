using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductShop : MonoBehaviour
{
     public ResourceType resourceType;
     public int price;
     public TMP_InputField buyAmount;
     public TextMeshProUGUI costAmount;
     public Button plusButton;
     public Button minusButton;
     public UIManager uIManager;
     public GameObject purchaseButton;


    private void Start()
    {
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
        purchaseButton.GetComponent<Button>().onClick.AddListener(OnPurchaseButtonClicked);

        buyAmount.onValueChanged.AddListener(delegate { OnValueChanged(); });
        OnValueChanged();
    }
    private void OnPurchaseButtonClicked()
    {
        if (int.TryParse(costAmount.text, out int cost))
        {     
            uIManager.UpdateGoldOnServer(cost);
            ResourceManager.Instance.AddResourceStorage(resourceType, int.Parse(buyAmount.text));
            uIManager.UpdateStorageAttributesOnServer();
        }
        else
        {
            Debug.LogError("Failed to parse cost amount: " + costAmount.text);
        }
        ResetValues();
    }
    public void OnPlusButtonClicked()
    {
        if (int.TryParse(buyAmount.text, out int currentAmount))
        {
            currentAmount += 100;
            buyAmount.text = currentAmount.ToString();
        }
        else
        {
            buyAmount.text = "100";
        }
        UpdateCostAmount();
    }
    public void OnMinusButtonClicked()
    {
        if (int.TryParse(buyAmount.text, out int currentAmount))
        {
            currentAmount = Mathf.Max(currentAmount - 100, 0);
            buyAmount.text = currentAmount.ToString();
        }
        else
        {
            buyAmount.text = "0";
        }
        UpdateCostAmount();
    }
    public void OnValueChanged()
    {
        UpdateCostAmount();
    }
    private void UpdateCostAmount()
    {

        if (int.TryParse(buyAmount.text, out int currentAmount))
        {
            int cost = Mathf.FloorToInt((currentAmount * 0.01f) * price);
            costAmount.text = cost.ToString();
            purchaseButton.SetActive(currentAmount > 0);
            if (loginmanager.LoadedPlayerData.gold < cost)
            {
                purchaseButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                purchaseButton.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            costAmount.text = "0";
            purchaseButton.SetActive(false);
        }     
    }
    private void ResetValues()
    {
        buyAmount.text = "0";
        costAmount.text = "0";
        purchaseButton.SetActive(false);
    }
}
