using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

public class CellDataDisplay : MonoBehaviour
{
    private bool isUpdating = false;
    public int ironactif;
    public int woodactif;
    public int mudactif;
    public int wateractif, solaireactif, windactif;
    public TerrainGridSystem terrainGrid;
    public TMP_Text productivityText;

    public TMP_Text regionText;
    public TMP_Text stateText;
    public TMP_Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        if (terrainGrid == null)
        {
            Debug.LogError("Terrain Grid System reference is not set!");
            return;
        }
       
        StartCoroutine(RepeatingUpdateCoroutine());

    }
    void Update()
    {

        // Afficher les données de la cellule au démarrage du jeu
        DisplayCellData();
    }


    void DisplayCellData()
    {
        mudactif = 0;
        woodactif = 0;
        ironactif = 0;
        solaireactif = 0;
        windactif = 0;
        wateractif = 0;
        // Parcourir toutes les cellules du Terrain Grid
        for (int x = 0; x < terrainGrid.columnCount; x++)
        {
            for (int y = 0; y < terrainGrid.rowCount; y++)
            {
                // Obtient l'index de la cellule à la position (x, y)
                int cellIndex = TerrainGridManager.Instance.cellDataGrid[x, y].index;

                // Obtient les données de la cellule
                TerrainCellData cellData = TerrainGridManager.Instance.cellDataGrid[x, y];

                if (cellData.regionData.resourceType == ResourceType.Iron)
                {
                    ironactif = ironactif + cellData.level;



                }

                if (cellData.regionData.resourceType == ResourceType.Wood)
                {

                    woodactif = woodactif + cellData.level;


                }

                if (cellData.regionData.resourceType == ResourceType.Mud)
                {

                    mudactif = mudactif + cellData.level;


                }

                if (cellData.regionData.resourceType == ResourceType.ETypeWater)
                {

                    wateractif = wateractif + cellData.level;


                }

                if (cellData.regionData.resourceType == ResourceType.ETypeWind)
                {

                    windactif = windactif + cellData.level;


                }

                if (cellData.regionData.resourceType == ResourceType.ETypeSolaire)
                {

                    solaireactif = solaireactif + cellData.level;


                }




            }
        }
     
        UpdatePlayerData();


    }
    private bool IsUpdating = false;
    public void UpdatePlayerData()
    {
        // Assuming loginmanager.LoadedPlayerData.username gives you the current username
        string username = loginmanager.LoadedPlayerData.username;

        // Instantiate and populate your data class
        PlayerUpdateData data = new PlayerUpdateData
        {
            wood = woodactif,
            iron = ironactif,
            mud = mudactif,
            solar = solaireactif,
            wind = windactif,
            water = wateractif
        };

        // Serialize the data object to a JSON string
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData); // Ensure this prints the expected JSON structure

        // Start the coroutine to update player data
        if (!IsUpdating) // Check to prevent concurrent updates
        {
            StartCoroutine(UpdatePlayerDataCoroutine(username, jsonData));
        }
    }


    IEnumerator UpdatePlayerDataCoroutine(string username, string jsonData)
    {
        IsUpdating = true;
        string url = $"http://localhost:9090/players/{username}";

        // Create a new UnityWebRequest, manually setting it to PUT
        UnityWebRequest request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
       
        yield return request.SendWebRequest();
        IsUpdating = false;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating player data: " + request.error);
        }
        else
        {
            Debug.Log("Player data updated successfully: " + request.downloadHandler.text);
        }
    }
    IEnumerator RepeatingUpdateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15); // Wait for 15 seconds before each update.

            if (!isUpdating) // Check to prevent concurrent updates.
            {
                UpdatePlayerData();
            }
        }
    }

}
[System.Serializable]
public class PlayerUpdateData
{
    public int wood;
    public int iron;
    public int mud;
    public int solar;
    public int wind;
    public int water;
}
