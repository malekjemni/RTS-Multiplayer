using System.Collections;
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
        StartCoroutine(GetCellFromWorld(CurrentUserManager.Instance.GetCurrentUserId(), cell.index));
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
    public IEnumerator GetCellFromWorld(string playerId, int cellIndex)
    {
        string url = $"http://127.0.0.1:9090/world/{playerId}/{cellIndex}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseText = webRequest.downloadHandler.text;
                currentCell = JsonUtility.FromJson<TerrainCellData>(responseText);
                SetDisplayValue(currentCell);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
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
