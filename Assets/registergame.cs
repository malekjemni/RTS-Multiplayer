using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using TGS;

public class RegisterGame : MonoBehaviour
{
  
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    public void OnRegisterButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Veuillez remplir tous les champs.");
            return;
        }

        StartCoroutine(RegisterUserCoroutine(username, password));
    }

    IEnumerator RegisterUserCoroutine(string username, string password)
    {
        // Création de l'objet JSON avec les données du joueur
        JSONObject playerData = new JSONObject(JSONObject.Type.OBJECT);
        playerData.AddField("username", username);
        playerData.AddField("password", password);
        Debug.Log(playerData);
        string url = "http://127.0.0.1:9090/addplayer"; // Mettez votre URL de login ici
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(playerData.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoi de la requête
        yield return request.SendWebRequest();

        // Vérification de la réponse
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur lors de l'inscription : " + request.error);
        }
        else
        {
            Debug.Log("Inscription réussie !");
            // Traiter la réponse du serveur si nécessaire
        }

        SceneManager.LoadScene("NewLogin");
    }
}