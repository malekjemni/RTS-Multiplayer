using System.Collections;
using System.Collections.Generic;
using TGS;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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
    private List<TerrainCellData> dataCells;

    public Transform UiRoot; 


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        //Hethi acitviha ki awel mara bech thel database
        //CreateNewTerrain();
        LoadTerrain();
    }


    public void CreateNewTerrain()
    {
        //when database is empty InitializeGrid()
        InitializeGrid();
        //StartCoroutine(DeleteAllCells());      
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
            // cell.regionData.activeCells++;
            // tgs.CellFadeOut(tgs.TerritoryGetCells(i)[0].index, UnityEngine.Color.yellow, 10, 10);

        }
    }

    public int ActiveCellsInRegion(int index)
    {
        int counter = 0;
        for (int i = 0; i < tgs.TerritoryGetCells(index).Count; i++)
        {
            if (GetCellData(tgs.TerritoryGetCells(index)[i].index).state == true)
            {
                counter++;
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
            if (data.index == 58 && data.level == 0)
            {
                StartCoroutine(GetCellByIndex(data.index));              
            }
            DrawCellLevelUi(data);

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

        GameObject cellUi = Instantiate(cellUiPrefab, new Vector3(tgs.CellGetPosition(data.index).x, tgs.CellGetPosition(data.index).y + 3f, tgs.CellGetPosition(data.index).z), Quaternion.identity, UiRoot);
        cellUi.name = "CellUI_" + data.index; 
        cellUi.GetComponentInChildren<TextMeshProUGUI>().text = data.level.ToString();
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
    public IEnumerator GetCellByIndex(int index)
    {
        string url = "http://127.0.0.1:9090/cell/" + index;
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
            Debug.Log("Cell updated successfully: "+ cellData.index);
            tgs.CellSetColor(cellData.index, Color.clear);
            tgs.CellFlash(cellData.index, Color.yellow, 2, 3);
            DrawCellLevelUi(cellData);
        }
        else
        {
            Debug.LogError($"Failed to update cell on server: {request.error}");
        }
    }

}
[System.Serializable]
public class CellList
{
    public List<TerrainCellData> cells;
}