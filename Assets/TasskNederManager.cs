using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TasskNederManager : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text goldText;

    void Start()
    {
        // Récupérer les données du joueur à partir de PlayerPrefs
        string username = PlayerPrefs.GetString("Username");
        int gold = PlayerPrefs.GetInt("Gold");

        // Mettre à jour les éléments de la scène avec les données du joueur
        usernameText.text = "Username: " + username;
        goldText.text = "Gold: " + gold.ToString();
    }
}
