using System.Collections;
using System.Collections.Generic;
using TGS;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class loginmanager : MonoBehaviour
{
    public static PlayerData LoadedPlayerData;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    public void AttemptLogin()
    {
        StartCoroutine(LoginRoutine());
    }

    IEnumerator LoginRoutine()
    {
       
        // Créer un objet JSON avec les données d'identification
        JSONObject loginData = new JSONObject();
        loginData.AddField("username", usernameInput.text);
        loginData.AddField("password", passwordInput.text);

        // Convertir l'objet JSON en chaîne
        string jsonData = loginData.ToString();

        // Préparer la requête POST
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        string url = "http://127.0.0.1:9090/login"; // Mettez votre URL de login ici
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoyer la requête
        yield return request.SendWebRequest();

        // Vérifier s'il y a des erreurs
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Erreur de connexion: " + request.error);
            messageText.text = "Erreur de connexion. Veuillez réessayer.";
            yield break;
        }
        string username = usernameInput.text;
        // Extraire les données de la réponse JSON
        JSONObject responseData = new JSONObject(request.downloadHandler.text);
        if (responseData.HasField("token"))
        {


            if (responseData.HasField("token"))
            {

                if (LoadedPlayerData == null) // Correctly check if LoadedPlayerData is null
                {
                    LoadedPlayerData = new PlayerData(); // Instantiate it if it is null
                }
                string token = responseData["token"].ToString();
                string id = responseData["player"]["_id"];
                LoadedPlayerData.token = token;
                Debug.Log("Connexion réussie. Token received: " + id);
                CurrentUserManager.Instance.SetCurrentUser(id);
                StartCoroutine(GetPlayerData(username));
            }
            else
            {
                Debug.Log("Connexion échouée. Pas de token reçu.");
                messageText.text = "Invalid username or password.";
            }



        }
        CurrentUserManager.Instance.SetCurrentUsername(usernameInput.text);
        PlayerPrefs.SetString("Username", usernameInput.text);
     
    }
  
    IEnumerator GetPlayerData(string username)
    {
        JSONObject playerData1 = new JSONObject(JSONObject.Type.OBJECT);
        playerData1.AddField("username", username);
        Debug.Log("Envoi de la requête GET pour récupérer les données du joueur.");
        string url = "http://localhost:9090/getUserbyUserName/" + username;
        UnityWebRequest request = UnityWebRequest.Get(url);



        // Envoyer la requête GET
        yield return request.SendWebRequest();

        // Vérifier s'il y a des erreurs
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error retrieving player data: " + request.error);
        }
        else
        {
            // Attempt to deserialize the JSON response to the PlayerData class
            try
            {
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);

                if (playerData != null)
                {
                    LoadedPlayerData = playerData;
                 

                    // Load the next scene after retrieving player data
                    SceneManager.LoadScene("TasskNeder", LoadSceneMode.Single);
                }
                else
                {
                    Debug.LogError("Failed to deserialize player data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception when processing player data: " + e.Message);
            }
        }
    }

}
[System.Serializable]
public class PlayerData
{
    public string username;
    public int gold;
    public int storagewood;
    public int storagemud;
    public int storageclay;
    public int storageenergie;
    // If you want to include the 'cell' array in Unity, you'll need a corresponding class for it
    // public Cell[] cell;

    // Note that these additional fields seem to be redundant with the ones above (gold vs goldd, etc.),
    // So you might need to clarify with your backend which ones to use
    public int goldd;
    public int wood;
    public int mud;
    public int clay;
    public int energie;
    public string token;
    public string _id;

    // Add other fields as necessary
}

// If you need to include the 'cell' array, define a Cell class as well:


