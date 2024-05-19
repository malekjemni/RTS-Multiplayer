using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AuctionRequestManager : MonoBehaviour
{
    private const string AuctionUrl = "http://127.0.0.1:9090/auction/";
    public IEnumerator DeleteAuction(string auctionId)
    {
        UnityWebRequest request = UnityWebRequest.Delete(AuctionUrl + auctionId);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error deleting auction: " + request.error);
        }
        else
        {
            Debug.Log("Auction deleted");
        }
    }
}
