using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuctionManager : MonoBehaviour
{
    private const string CreateAuctionUrl = "http://127.0.0.1:9090/auction/create";
    private const string AuctionUrl = "http://127.0.0.1:9090/auction/";
    private const string GetAllAuctionsUrl = "http://127.0.0.1:9090/auctions";

    public UIManager uIManager;
    public AuctionRequestManager auctionRequestManager;
    public GameObject playerAuctionPrefab;
    public Transform contentParent;

    [Header("Create Auction UI")]
    public string seller;
    public TMP_InputField sellerAmount;
    public TMP_InputField buyerAmount;
    public TMP_Dropdown sellerResource;
    public TMP_Dropdown buyerResource;

    public GameObject auctionListUI;
    public GameObject createAuctiontUI;
    public GameObject mainUI;
    public GameObject confirmUI;
    public Button confirmButton;

    public TextMeshProUGUI errorMsg;
    public TextMeshProUGUI CreateErrorMsg;

    private void OnEnable()
    {
        InvokeRepeating(nameof(LoadAuctions), 0f, 5f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(LoadAuctions));
    }
    public void OpenAuctionUi()
    {
        mainUI.SetActive(false);
        auctionListUI.SetActive(true);
        createAuctiontUI.SetActive(false);
    }
    public void OpenAuctionCreationUI()
    {
        auctionListUI.SetActive(false);
        createAuctiontUI.SetActive(true);
        PopulateDropdownWithEnum<ResourceType>(sellerResource);
        PopulateDropdownWithEnum<ResourceType>(buyerResource);
    }
    public IEnumerator CreateAuction(PlayerAuction auction)
    {
        string json = JsonUtility.ToJson(auction);
        UnityWebRequest request = new UnityWebRequest(CreateAuctionUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error creating auction: " + request.error);
        }
        else
        {
            OpenAuctionUi();
        }
    }
  
    public IEnumerator GetAllAuctions()
    {
        UnityWebRequest request = UnityWebRequest.Get(GetAllAuctionsUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error retrieving auctions: " + request.error);
        }
        else
        {
            AuctionList auctionList = JsonUtility.FromJson<AuctionList>(request.downloadHandler.text);
            GetAuctionsData(auctionList.auctions);
        }
    }
    public void GetAuctionsData(List<PlayerAuction> auctionList)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < auctionList.Count; i++)
        {
            if (auctionList[i].state)
            {
                continue;
            }

            GameObject playerEntry = Instantiate(playerAuctionPrefab, contentParent);

            TextMeshProUGUI usernameText = playerEntry.transform.Find("Username").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI buyOffer = playerEntry.transform.Find("Give").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sellOffer = playerEntry.transform.Find("Take").GetComponent<TextMeshProUGUI>();
            Button tradeAction = playerEntry.transform.Find("TradeButton").GetComponent<Button>();

            var auction = auctionList[i]; 

            StartCoroutine(GetPlayerName(auction.seller, (playerName) =>
            {
                if (!string.IsNullOrEmpty(playerName))
                {
                    usernameText.text = playerName;
                }
                else
                {
                    Debug.LogError("Failed to retrieve player name.");
                }
            }));

            sellOffer.text = auction.sellerResourceType + "/" + auction.sellerAmount;
            buyOffer.text = auction.buyerResourceType + "/" + auction.buyerAmount;

            tradeAction.onClick.RemoveAllListeners();
            tradeAction.onClick.AddListener(() => LoadSelectedAuction(auction._id));
        }
    }
    public void LoadAuctions() => StartCoroutine(GetAllAuctions());
    public void CreateNewAuction()
    {
        PlayerAuction auction = new PlayerAuction()
        {
            seller = CurrentUserManager.Instance.GetCurrentUserId(),
            sellerAmount = int.Parse(sellerAmount.text),
            buyerAmount = int.Parse(buyerAmount.text),
            sellerResourceType = sellerResource.options[sellerResource.value].text,
            buyerResourceType = buyerResource.options[buyerResource.value].text,
            state = false,
        };
        AttemptTradeCreation(auction);  


    }
    void PopulateDropdownWithEnum<T>(TMP_Dropdown dropdown) where T : System.Enum
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>(System.Enum.GetNames(typeof(T)));
        dropdown.AddOptions(options);
    }
    public IEnumerator GetPlayerName(string playerId, System.Action<string> callback)
    {
        string url = "http://127.0.0.1:9090/player/" + playerId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            string playerName = responseText.Trim('"');
            callback?.Invoke(playerName);
        }
        else
        {
            Debug.LogError("Error retrieving player data: " + request.error);
            callback?.Invoke(null);
        }
    }
    public void LoadSelectedAuction(string auctionId)
    {
        StartCoroutine(GetSelectedAuction(auctionId));
        confirmUI.SetActive(true);
        auctionListUI.SetActive(false);
    }
    IEnumerator GetSelectedAuction(string auctionId)
    {
        string url = "http://127.0.0.1:9090/auction/" + auctionId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            PlayerAuction auction = JsonUtility.FromJson<PlayerAuction>(request.downloadHandler.text);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => AttemptTrade(auction));
        }
        else
        {
            Debug.LogError("Error retrieving Auction data: " + request.error);
        }
    }
    public void AttemptTrade(PlayerAuction auction)
    {
        if (CanMakeTrade(auction))
        {
            if (Enum.TryParse(auction.buyerResourceType, out ResourceType buyerResourceType))
            {       
                ResourceManager.Instance.SubtractResourceStorage(buyerResourceType, auction.buyerAmount);
                
            }
            if (Enum.TryParse(auction.sellerResourceType, out ResourceType sellerResourceType))
            {
                ResourceManager.Instance.AddResourceStorage(sellerResourceType, auction.sellerAmount);
            }


            uIManager.UpdateStorageAttributesOnServer();
            CompleteTrade(auction);
            confirmUI.SetActive(false);
            auctionListUI.SetActive(true);
        }
        else
        {
            errorMsg.text = "Not enough resources for the trade.";
        }
    }
    public void AttemptTradeCreation(PlayerAuction auction)
    {
        if (CanCreateTrade(auction))
        {
            if (Enum.TryParse(auction.sellerResourceType, out ResourceType sellerResourceType))
            {
                StartCoroutine(CreateAuction(auction));
                ResourceManager.Instance.SubtractResourceStorage(sellerResourceType, auction.sellerAmount);
                
            }
        }
        else
        {
            CreateErrorMsg.text = "Not enough resources to create a trade.";
        }
    }
    public bool CanMakeTrade(PlayerAuction auction)
    {
        if (Enum.TryParse(auction.buyerResourceType, out ResourceType resourceType))
        {
            int requiredAmount = auction.buyerAmount;

            switch (resourceType)
            {
                case ResourceType.ETypeSolaire:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.ETypeWind:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.ETypeWater:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.Iron:
                    return uIManager.playerStorageData.storageclay >= requiredAmount;
                case ResourceType.Wood:
                    return uIManager.playerStorageData.storagewood >= requiredAmount;
                case ResourceType.Mud:
                    return uIManager.playerStorageData.storagemud >= requiredAmount;
                default:
                    Debug.LogError("Unknown resource type: " + auction.buyerResourceType);
                    return false;
            }
        }
        else
        {
            Debug.LogError("Invalid resource type: " + auction.buyerResourceType);
            return false;
        }
    }
    public bool CanCreateTrade(PlayerAuction auction)
    {
        if (Enum.TryParse(auction.sellerResourceType, out ResourceType resourceType))
        {
            int requiredAmount = auction.sellerAmount;

            switch (resourceType)
            {
                case ResourceType.ETypeSolaire:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.ETypeWind:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.ETypeWater:
                    return uIManager.playerStorageData.storageenergie >= requiredAmount;
                case ResourceType.Iron:
                    return uIManager.playerStorageData.storageclay >= requiredAmount;
                case ResourceType.Wood:
                    return uIManager.playerStorageData.storagewood >= requiredAmount;
                case ResourceType.Mud:
                    return uIManager.playerStorageData.storagemud >= requiredAmount;
                default:
                    Debug.LogError("Unknown resource type: " + auction.sellerResourceType);
                    return false;
            }
        }
        else
        {
            Debug.LogError("Invalid resource type: " + auction.sellerResourceType);
            return false;
        }
    }
    public void CompleteTrade(PlayerAuction auction) => StartCoroutine(CompleteTradeCoroutine(auction));
    IEnumerator CompleteTradeCoroutine(PlayerAuction auction)
    {
        string url = AuctionUrl + auction._id;
        auction.state = true;

        string jsonData = JsonUtility.ToJson(auction);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Trade completed: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to update cell on server: {request.error}");
        }
    }
  



}
[Serializable]
public class AuctionList
{
    public List<PlayerAuction> auctions;
}
