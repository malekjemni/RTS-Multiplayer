using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class MenuSelection : MonoBehaviour
{
    public GameObject leaderBoardView;
    public GameObject playerEntryPrefab;
    public Transform contentParent;

    public void QuitGame() => Application.Quit();
    public void ShowLeaderBoard() => StartCoroutine(GetLeaderboard());
    public IEnumerator GetLeaderboard()
    {
        string url = $"http://127.0.0.1:9090/leaderboard";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseText = webRequest.downloadHandler.text;
                List<PlayerData> playerList = JsonUtility.FromJson<PlayersData>(responseText).players;
                LoadLeaderboard(playerList);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
    public void LoadLeaderboard(List<PlayerData> playerList)
    {
        leaderBoardView.SetActive(true);
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < playerList.Count; i++)
        {
     
            GameObject playerEntry = Instantiate(playerEntryPrefab, contentParent);

      
            TextMeshProUGUI rankText = playerEntry.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI usernameText = playerEntry.transform.Find("Username").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = playerEntry.transform.Find("Score").GetComponent<TextMeshProUGUI>();

            
            rankText.text = (i + 1).ToString();
            usernameText.text = playerList[i].username;
            scoreText.text = playerList[i].score.ToString();
        }
    }
}
[System.Serializable]
public class PlayersData
{
    public List<PlayerData> players;
}