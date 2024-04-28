using System;
using System.Collections;
using TGS;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CellDisplay : MonoBehaviour
{

    public TextMeshProUGUI RegionText;
    public TextMeshProUGUI ProductivityText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI StateText;

    public TerrainCellData currentCell = null;
    private RegionData targetRegion;

    public void UpdateCellData(int index)
    {
        TerrainCellData cell = TerrainGridManager.Instance.GetCellData(index);
        targetRegion = cell.regionData;      
        StartCoroutine(GetCellData(cell.index));       
    }

    public void SetDisplayValue(TerrainCellData cell)
    {
        
        if (cell != null)
        {
            if (currentCell != null)
            {
                cell.region = currentCell.region;
                cell.level = currentCell.level;
                cell.state = currentCell.state;
                cell.regionData = targetRegion;
                if(cell.level == 0)
                {
                    cell.productivite = cell.regionData.productionRateBase;
                }
                else
                {
                    cell.productivite = cell.regionData.productionRateBase * cell.level;
                }
                UpdateUIWithCellData(cell);              
            }
            else
            {
                Debug.LogError("Failed to retrieve cell data for index: " );
            }
        }
    }
    public IEnumerator GetCellData(int index)
    {
        string url = "http://127.0.0.1:9090/cell/" + index;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
    
            string responseText = request.downloadHandler.text;
            currentCell = JsonUtility.FromJson<TerrainCellData>(responseText);
            SetDisplayValue(currentCell);
        }
        else
        {
            Debug.LogError("Error retrieving cell data: " + request.error);
        }

        request.Dispose();
    }

    private void UpdateUIWithCellData(TerrainCellData cell)
    {
        if (cell.level == 0)
        {
            ProductivityText.text = (cell.regionData.productionRateBase).ToString();
        }
        else
        {
            ProductivityText.text = (cell.regionData.productionRateBase * cell.level).ToString();
        }
       

        LevelText.text = cell.level.ToString();
        StateText.text = cell.state ? "Active" : "Inactive";
    }
}
