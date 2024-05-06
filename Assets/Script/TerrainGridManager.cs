using System.Collections;
using System.Collections.Generic;
using TGS;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TerrainGridManager : MonoBehaviour
{
    public Material LockMaterial;
    public TerrainGridSystem tgs;
    public RegionData[] regionDataList; // List of RegionData scriptable objects
    public TerrainCellData[,] cellDataGrid;//matrice mtaa les cellules data
    public static TerrainGridManager Instance;
    public GameObject cellUiPrefab;

    private const string createCellUrl = "http://127.0.0.1:9090/cell/create";
    private const string getCellsUrl = "http://127.0.0.1:9090/cells";
    private const string deleteALL = "http://127.0.0.1:9090/clear";
    public List<TerrainCellData> dataCells;
    private List<string> cellIds = new List<string>();
    private string worldId;

    public Transform UiRoot;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine(CheckIfPlayerHasWorld());
    }
    public void CreateNewTerrain()
    {
        InitializeGrid();    
        StartCoroutine("InitiateStartingGrid");
    }
    public void LoadTerrain()
    {
        StartCoroutine(GetAllCellsInDatabase());
        StartCoroutine("InitiateStartingGrid");
    }
    public void InitializeGrid()
    {

        cellDataGrid = new TerrainCellData[tgs.columnCount, tgs.rowCount];
        for (int x = 0; x < tgs.columnCount; x++)
        {
            for (int y = 0; y < tgs.rowCount; y++)
            {

                //cellDataGrid[x, y] = PopulateGridWithCellData(x, y);
                cellDataGrid[x, y] = GenerateCellData(x, y);
                //  tgs.CellSetColor(cellDataGrid[x, y].id,UnityEngine.Color.gray);
                //  tgs.CellFadeOut(cellDataGrid[x, y].id, UnityEngine.Color.gray, 10, 10);
               if (!cellDataGrid[x, y].state) { tgs.CellSetMaterial(cellDataGrid[x, y].index, LockMaterial); }
                // tgs.CellSetVisible(cellDataGrid[x, y].id, true);  
            }
        }
    }
    public void LoadGrid()
    {
        cellDataGrid = new TerrainCellData[tgs.columnCount, tgs.rowCount];
        for (int x = 0; x < tgs.columnCount; x++)
        {
            for (int y = 0; y < tgs.rowCount; y++)
            {
                cellDataGrid[x, y] = PopulateGridWithCellData(x, y);
                if (!cellDataGrid[x, y].state) { tgs.CellSetMaterial(cellDataGrid[x, y].index, LockMaterial); }
            }
        }
    }
    public TerrainCellData GetCellData(int index)
    {
        for (int x = 0; x < tgs.columnCount; x++)
        {
            for (int y = 0; y < tgs.rowCount; y++)
            {

                if (cellDataGrid[x, y].index == index)
                {

                    return cellDataGrid[x, y];
                }


            }
        }
        return null;
    }
    private IEnumerator InitiateStartingGrid()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 6; i++)
        {
            tgs.CellSetColor(tgs.TerritoryGetCells(i)[0], UnityEngine.Color.clear);
            tgs.CellFlash(tgs.TerritoryGetCells(i)[0], UnityEngine.Color.yellow, 2, 3);

            TerrainCellData cell = GetCellData(tgs.TerritoryGetCells(i)[0].index);
            cell.state = true;
            cell.level = 1;
            StartCoroutine(GetCellByIndex(cell.index));
            // cell.regionData.activeCells++;
            // tgs.CellFadeOut(tgs.TerritoryGetCells(i)[0].index, UnityEngine.Color.yellow, 10, 10);

        }
    }
    public int ActiveCellsInRegion(int index)
    {
        int counter = 0;
        for (int i = 0; i < tgs.TerritoryGetCells(index).Count; i++)
        {
            if (GetCellData(tgs.TerritoryGetCells(index)[i].index).state == false)
            {
                counter = i;
                break;
            };
        }
        return counter;
    }
    public void OpenNewCell(TerrainCellData cellData)
    {
        int regionIndex = cellData.regionData.index;
        tgs.CellSetColor(tgs.TerritoryGetCells(regionIndex)[ActiveCellsInRegion(regionIndex)], Color.clear);
        tgs.CellFlash(tgs.TerritoryGetCells(regionIndex)[ActiveCellsInRegion(regionIndex)], Color.yellow, 2, 3);
        GetCellData(tgs.TerritoryGetCells(regionIndex)[ActiveCellsInRegion(regionIndex)].index).level = 1;
        GetCellData(tgs.TerritoryGetCells(regionIndex)[ActiveCellsInRegion(regionIndex)].index).state = true;
        StartCoroutine(GetCellByIndex(tgs.TerritoryGetCells(regionIndex)[ActiveCellsInRegion(regionIndex)].index));
    }
    private TerrainCellData GenerateCellData(int x, int y)
    {
        TerrainCellData data = new TerrainCellData();


        data.index = tgs.CellGetIndex(x, y, true);
        data.owner = CurrentUserManager.Instance.GetCurrentUserId();
        data.state = false;
        int territoryIndex = tgs.CellGetTerritoryIndex(data.index);


        if (territoryIndex >= 0 && territoryIndex < regionDataList.Length)
        {
            data.regionData = regionDataList[territoryIndex];
            data.region = regionDataList[territoryIndex].ToString();
        }
        else
        {
            Debug.LogError("Territory index out of bounds or regionDataList is not properly initialized.");
            data.regionData = null; // Or handle the error in your desired way
        }

        // Initialize level

        data.productivite = data.regionData.productionRateBase;
        data.level = 0;



        data.CalculateMaterialsNeededForNextUpgrade();
        StartCoroutine(CreateCellInDatabase(data));
        return data;
    }
    private IEnumerator CreateCellInDatabase(TerrainCellData data)
    {
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(createCellUrl, "POST");
        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyData);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            StartCoroutine(AddCellToWorld(worldId, JsonUtility.FromJson<TerrainCellData>(request.downloadHandler.text)._id,data));
            dataCells.Add(JsonUtility.FromJson<TerrainCellData>(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError("Failed to create cell in the database: " + request.error);
        }
    }
    public void DrawCellLevelUi(TerrainCellData data)
    {
        Transform existingUi = UiRoot.Find("CellUI_" + data.index);
        if (existingUi != null)
        {
            Destroy(existingUi.gameObject);
        }

        GameObject cellUi = Instantiate(cellUiPrefab, new Vector3(tgs.CellGetPosition(data.index).x, 5f, tgs.CellGetPosition(data.index).z), Quaternion.identity, UiRoot);
        cellUi.name = "CellUI_" + data.index;
        
        if(data.level == 0) 
        {
            cellUi.transform.Find("Lock").gameObject.SetActive(true);
        }
        else
        {
            cellUi.GetComponentInChildren<TextMeshProUGUI>().text = data.level.ToString();
        }

        cellUi.SetActive(false);
    }
    public void ShowCellLevel(int index)
    {
        Transform existingUi = UiRoot.Find("CellUI_" + index);
        existingUi.gameObject.SetActive(true);
    }
    public void HideCellLevel(int index)
    {
        Transform existingUi = UiRoot.Find("CellUI_" + index);
        existingUi.gameObject.SetActive(false);
    }
    private IEnumerator GetAllCellsInDatabase()
    {
        UnityWebRequest request = UnityWebRequest.Get(getCellsUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            dataCells = JsonUtility.FromJson<CellList>(responseText).cells;
            LoadGrid();
        }
        else
        {
            Debug.LogError("Error fetching cells: " + request.error);
        }
    }
    public TerrainCellData PopulateGridWithCellData(int x, int y)
    {
        TerrainCellData data = new TerrainCellData();

        foreach (TerrainCellData cellData in dataCells)
        {
            if (cellData.index == tgs.CellGetIndex(x, y, true))
            {
                data.index = cellData.index;
                data.owner = CurrentUserManager.Instance.GetCurrentUserId();
                data.state = cellData.state;
                int territoryIndex = tgs.CellGetTerritoryIndex(data.index);

                if (territoryIndex >= 0 && territoryIndex < regionDataList.Length)
                {
                    data.regionData = regionDataList[territoryIndex];
                    data.region = cellData.region;
                }
                else
                {
                    Debug.LogError("Territory index out of bounds or regionDataList is not properly initialized.");
                    data.regionData = null;
                }

                // Initialize level

                data.productivite = cellData.productivite;
                data.level = cellData.level;
                DrawCellLevelUi(data);


                data.CalculateMaterialsNeededForNextUpgrade();
            }

        }

        return data;
    }
    public IEnumerator DeleteAllCells()
    {
        UnityWebRequest request = UnityWebRequest.Delete(deleteALL);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            InitializeGrid();
        }
        else
        {
            Debug.LogError($"Failed to delete all cells: {request.error}");
        }
    }
    public int GetProductionSum(ResourceType type)
    {
        int sum = 0;
        for (int x = 0; x < tgs.columnCount; x++)
        {
            for (int y = 0; y < tgs.rowCount; y++)
            {

                if (cellDataGrid[x, y].state == true && cellDataGrid[x, y].regionData.resourceType == type)
                {
                    sum += cellDataGrid[x, y].regionData.productionRateBase * cellDataGrid[x, y].regionData.levelUpMultiplier;
                    //Debug.Log("type "+ type+" "+sum);

                }
            }
        }

        return sum;
    }
    public IEnumerator GetCellByIndex(int cellIndex)
    {
        string url = $"http://127.0.0.1:9090/world/{CurrentUserManager.Instance.GetCurrentUserId()}/{cellIndex}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            TerrainCellData currentCell = JsonUtility.FromJson<TerrainCellData>(responseText);         
            StartCoroutine(UpdateCellDataRequest(currentCell));
        }
        else
        {
            Debug.LogError("Error retrieving cell data: " + request.error);
        }

        request.Dispose();
    }
    IEnumerator UpdateCellDataRequest(TerrainCellData cellData)
    {
        string url = "http://127.0.0.1:9090/cell/" + cellData._id;

        UpdateCellData cell = new UpdateCellData();
        cell.state = true;
        cell.level = 1;
        string jsonData = JsonUtility.ToJson(cell);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            tgs.CellSetColor(cellData.index, Color.clear);
            tgs.CellFlash(cellData.index, Color.yellow, 2, 3);
            DrawCellLevelUi(JsonUtility.FromJson<TerrainCellData>(request.downloadHandler.text));           
        }
        else
        {
            Debug.LogError($"Failed to update cell on server: {request.error}");
        }
    }
    public IEnumerator GetWorldForPlayer()
    {
        string url = "http://127.0.0.1:9090/world/" + CurrentUserManager.Instance.GetCurrentUserId();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                dataCells = JsonUtility.FromJson<CellList>(json).cells;
                LoadGrid();
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
    public IEnumerator CreateWorld()
    {
        string url = "http://127.0.0.1:9090/world/create";

        JSONObject jsonBody = new JSONObject();
        jsonBody.AddField("player", CurrentUserManager.Instance.GetCurrentUserId());

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                WorldData world = JsonUtility.FromJson<WorldData>(jsonResponse);
                worldId = world._id;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
    public IEnumerator AddCellToWorld(string worldId, string cellId, TerrainCellData data)
    {
        string url = "http://127.0.0.1:9090/world/addCell";

        JSONObject jsonBody = new JSONObject();
        jsonBody.AddField("worldId", worldId);
        jsonBody.AddField("cellId", cellId);


        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
               DrawCellLevelUi(data);              
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

    }
    public IEnumerator CheckIfPlayerHasWorld()
    {
        string url = "http://127.0.0.1:9090/check/" + CurrentUserManager.Instance.GetCurrentUserId();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                bool playerHasWorld = bool.Parse(jsonResponse);

                if (playerHasWorld)
                {
                    StartCoroutine(GetWorldForPlayer());
                }
                else
                {
                    StartCoroutine(CreateWorld());
                    CreateNewTerrain();
                }
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    public void SetCanUpgradeForCell(TerrainCellData cell, bool newValue)
    {
       
        int index = dataCells.FindIndex(c => c.index == cell.index);
        dataCells[index].canUpgrade = newValue;
    }
    public bool GetCanUpgradeForCell(TerrainCellData cell)
    {
        int index = dataCells.FindIndex(c => c.index == cell.index);
        return dataCells[index].canUpgrade;
    }


}


[System.Serializable]
public class CellList
{
    public List<TerrainCellData> cells;
}
[System.Serializable]
public class WorldData
{
    public string _id;
    public string player;
}
