using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MailBox : MonoBehaviour
{
    public GameObject playerAuctionPrefab;
    public Transform contentParent;
    public UIManager uIManager;
    public AuctionRequestManager auctionRequestManager;

    private void OnEnable()
    {
        InvokeRepeating(nameof(OpenMailBoxAuctions), 0f, 5f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(OpenMailBoxAuctions));
    }
    public void ClaimCompletedTrades()
    {
        CheckForCompletedAuctions();
    }
    public void OpenMailBoxAuctions() => StartCoroutine(GetPlayerAuctions());
    public void GetAuctionsData(List<PlayerAuction> auctionList)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < auctionList.Count; i++)
        {
            

            GameObject playerEntry = Instantiate(playerAuctionPrefab, contentParent);

            TextMeshProUGUI usernameText = playerEntry.transform.Find("Username").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI buyOffer = playerEntry.transform.Find("Take").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sellOffer = playerEntry.transform.Find("Give").GetComponent<TextMeshProUGUI>();

            var auction = auctionList[i];

            if (auctionList[i].state)
            {
                playerEntry.transform.Find("Completed").gameObject.SetActive(true);
                playerEntry.transform.Find("TradeButton").gameObject.SetActive(false);
            }
            else
            {
                Button tradeAction = playerEntry.transform.Find("TradeButton").GetComponent<Button>();
                tradeAction.onClick.RemoveAllListeners();
                tradeAction.onClick.AddListener(() => LoadSelectedAuction(auction._id));
            }
          

            

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
            
        }
    }
    public void LoadSelectedAuction(string auctionId)
    {
        StartCoroutine(GetSelectedAuction(auctionId));
    }
    IEnumerator GetSelectedAuction(string auctionId)
    {
        string url = "http://127.0.0.1:9090/auction/" + auctionId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            PlayerAuction auction = JsonUtility.FromJson<PlayerAuction>(request.downloadHandler.text);
            if (Enum.TryParse(auction.sellerResourceType, out ResourceType sellerResourceType))
            {
                ResourceManager.Instance.AddResourceStorage(sellerResourceType, auction.sellerAmount);
                uIManager.UpdateStorageAttributesOnServer();
                StartCoroutine(auctionRequestManager.DeleteAuction(auction._id));
            }          
        }
        else
        {
            Debug.LogError("Error retrieving Auction data: " + request.error);
        }
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
    IEnumerator GetPlayerAuctions()
    {
        string url = $"http://127.0.0.1:9090/playerAuctions/{CurrentUserManager.Instance.GetCurrentUserId()}/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                AuctionList auctionList = JsonUtility.FromJson<AuctionList>(webRequest.downloadHandler.text);
                GetAuctionsData(auctionList.auctions);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }


    public void CheckForCompletedAuctions() => StartCoroutine(GetCompletedAuctions());
    IEnumerator GetCompletedAuctions()
    {
        string url = $"http://127.0.0.1:9090/playerAuctions/{CurrentUserManager.Instance.GetCurrentUserId()}/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                AuctionList auctionList = JsonUtility.FromJson<AuctionList>(webRequest.downloadHandler.text);
                VerifyTransaction(auctionList.auctions);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
    public void VerifyTransaction(List<PlayerAuction> auctionList)
    {
        foreach (var auction in auctionList)
        {
            if (auction.state)
            {

                if (Enum.TryParse(auction.buyerResourceType, out ResourceType buyerResourceType))
                {
                    ResourceManager.Instance.AddResourceStorage(buyerResourceType, auction.buyerAmount);
                    uIManager.UpdateStorageAttributesOnServer();
                    StartCoroutine(auctionRequestManager.DeleteAuction(auction._id));
                }
                else
                {
                    Debug.LogError("Invalid seller resource type: " + auction.buyerResourceType);
                    continue; // Skip to the next auction if the resource type is invalid
                }
            }
        }


    }
}
