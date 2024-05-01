using Mirror;
using System.Collections;
using TGS;
using UnityEngine;
using UnityEngine.Networking;

public class SelectModeMenu : MonoBehaviour
{
    public GameObject canva;
    private bool isHostStarted = false;
    public GameObject lootScene;
    public TimerUi timerUi;

    private bool isCreated = false;
    private string roomId;
    public void CreateNewTerrain()
    {
        canva.SetActive(false);
        StartCoroutine(WaitForTerrainGridManagerToInitialize(() =>
        {
            TerrainGridManager.Instance.CreateNewTerrain();
        }));
    }
    public void LoadTerrain()
    {
        canva.SetActive(false);
        StartCoroutine(WaitForTerrainGridManagerToInitialize(() =>
        {
            TerrainGridManager.Instance.LoadTerrain();
        }));
    }
    public void StartLootScene()
    {
        StartCoroutine(GetRoomsAndAddPlayer());
        lootScene.SetActive(false);
    }

    private IEnumerator WaitForTerrainGridManagerToInitialize(System.Action action)
    {
        while (TerrainGridManager.Instance == null)
        {
            yield return null;
        }
        action.Invoke();
    }
    private void Update()
    {
        if ((timerUi.timerString.Equals("00:30") ||
      timerUi.timerString.Equals("05:00") ||
      timerUi.timerString.Equals("10:00") ||
      timerUi.timerString.Equals("15:00")) && !isCreated)
        {
            lootScene.SetActive(true);
            isCreated = true;
        }


    }

    public void CreateRoom(string state)
    {
        StartCoroutine(PostRoom(state));
    }

    public void AddPlayerToRoom(string roomId, string playerId)
    {
        StartCoroutine(PostAddPlayerToRoom(roomId, playerId));     
    }

    public void GetPlayersInRoom(string roomId)
    {
        StartCoroutine(GetPlayers(roomId));
    }

    IEnumerator GetPlayers(string roomId)
    {
        string url = "http://127.0.0.1:9090/room/" + roomId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JSONObject responseData = new JSONObject(request.downloadHandler.text);
                if (responseData.IsArray && responseData.Count == 1)
                {
                    string playerId = responseData[0]["_id"].str;

                    StartHost();
                }
                else
                {
                    StartClient();
                }
            
              }
            else
            {
                Debug.LogError("Failed to get players in room: " + request.error);
            }
        }
    }
    IEnumerator PostAddPlayerToRoom(string roomId, string playerId)
    {
        string url = "http://127.0.0.1:9090/room/addPlayer";
        JSONObject json = new JSONObject();
        json.AddField("roomId", roomId);
        json.AddField("playerId", playerId);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                GetPlayersInRoom(roomId);
            }
            else
            {
                Debug.LogError("Failed to add player to room: " + request.error);
            }
        }
    }
    IEnumerator PostRoom(string state)
    {
        string url = "http://127.0.0.1:9090/room/create";
        JSONObject json = new JSONObject();
        json.AddField("state", state);

        // Create a UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JSONObject responseData = new JSONObject(request.downloadHandler.text);
                roomId = responseData["_id"];
                AddPlayerToRoom(roomId, CurrentUserManager.Instance.GetCurrentUserId());               
                Debug.Log("Room created successfully"+ roomId);
            }
            else
            {
                Debug.LogError("Failed to create room: " + request.error);
            }
        }
    }
    IEnumerator GetRoomsAndAddPlayer()
    {
        string url = "http://127.0.0.1:9090/rooms";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JSONObject json = new JSONObject(request.downloadHandler.text);

                bool lobbyExists = false;
                string lobbyId = "";
       
                for (int i = 0; i < json.Count; i++)
                {
                    JSONObject roomJson = json[i];

                    string roomId = roomJson["_id"];
                    string state = roomJson["state"];

                    if (state == "lobby")
                    {
                        lobbyExists = true;
                        lobbyId = roomId;
                        break;
                    }
                }
                if (!lobbyExists)
                {
                    CreateRoom("lobby");
                }
                else
                {
                    AddPlayerToRoom(lobbyId, CurrentUserManager.Instance.GetCurrentUserId());
                }
            }
            else
            {
                Debug.LogError("Failed to get rooms: " + request.error);
            }
        }
    }



    private void StartHost()
    {
         NetworkManager.singleton.StartHost();
         TimerSync.Instance.StartCountDownTimer();
    }
    private void StartClient()
    {
        NetworkManager.singleton.StartClient();
    }
}
