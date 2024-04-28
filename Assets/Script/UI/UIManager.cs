using TGS;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance;

    public static Action OnCellHover;

    /// <summary>
    /// /Time
    /// </summary>
    public TextMeshProUGUI timeText;

    TerrainGridSystem tgs;


    public TextMeshProUGUI WoodStorage;
    public TextMeshProUGUI IronStorage;
    public TextMeshProUGUI MudStorage;
    public TextMeshProUGUI EnergieStorage;

    public TextMeshProUGUI WoodProduction;
    public TextMeshProUGUI IronProduction;
    public TextMeshProUGUI MudProduction;
    public TextMeshProUGUI EnergieProduction;


    public TextMeshProUGUI RegionText;
    public TextMeshProUGUI ProductivityText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI StateText;



    public TextMeshProUGUI cellDescription;
    public TextMeshProUGUI woodRequired;
    public TextMeshProUGUI IronRequired;
    public TextMeshProUGUI MudRequired;
    public TextMeshProUGUI EnergeyRequired;

    public TextMeshProUGUI Gem;

    public TextMeshProUGUI clokTime;

    public int timeElapsed = 0;


    public GameObject feedbackpos;
    public GameObject feedbackneg;


    public GameObject Clock;

    public CellDisplay cellDisplay;


    public Button LevelUpButton;
    public Button SkipButton;
    public Button MoreButton;
    public GameObject MaxReached;
    public GameObject MaxReachedText;


    public GameObject uiTest;
    private bool isUpgradeable = false;
    public StorageUpdateData playerStorageData;


    private void Start()
    {
        Instance = this;
        GetPlayerStorage();
       
        //if (loginmanager.LoadedPlayerData != null)
        //{
        //    // Assuming ResourceManager has methods to add resources to storage
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.Wood, loginmanager.LoadedPlayerData.storagewood);
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.Iron, loginmanager.LoadedPlayerData.storageclay); // Assuming clay maps to iron in your model
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.Mud, loginmanager.LoadedPlayerData.storagemud);
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeSolaire, loginmanager.LoadedPlayerData.storageenergie); // You may need to adjust this for your energy resources
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeWind, loginmanager.LoadedPlayerData.storageenergie);
        //    ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeWater, loginmanager.LoadedPlayerData.storageenergie);
        //    // Update UI to reflect the combined total resources
        //    UpdateUiStorage();
        //}


        tgs = TerrainGridSystem.instance;
        UpdateUiStorage();
        UpdateUiProduction();
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => cellDisplay.UpdateCellData(cellIndex);
        //tgs.OnCellClick += (grid, cellIndex, buttonIndex) => UpdateCellDescription();
        LevelUpButton.onClick.AddListener(() => IsUpgradeable(cellDisplay.currentCell));
        SkipButton.onClick.AddListener(() => SkipWaiting(cellDisplay.currentCell));
        MoreButton.onClick.AddListener(() => UpdateCellDescription());
  

        Gem.text = ResourceManager.Instance.Gem.ToString();


    }


    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdateTime;
        ResourceManager.OnStorageChange += UpdateUiStorage;
        ResourceManager.OnProductionChange += UpdateUiProduction;


    }

    private void OnDisable()
    {

        TimeManager.OnMinuteChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdateTime;
        ResourceManager.OnStorageChange -= UpdateUiStorage;
        ResourceManager.OnProductionChange -= UpdateUiProduction;

    }

    private void UpdateTime()
    {
       // timeText.text = $"{TimeManager.Hour:00}:{TimeManager.Minute:00}";
    }

    private void UpdateUiStorage()
    {
        UpdateStorageAttributesOnServer();
    }

    private void UpdateUiProduction()
    {
        WoodProduction.text = ResourceManager.Instance.GetResourceAmountProduction(ResourceType.Wood).ToString();
        IronProduction.text = ResourceManager.Instance.GetResourceAmountProduction(ResourceType.Iron).ToString();
        MudProduction.text = ResourceManager.Instance.GetResourceAmountProduction(ResourceType.Mud).ToString();
        int energyValue = ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeSolaire) + ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeWind) + ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeWater);
        EnergieProduction.text = energyValue.ToString();
        // Debug.Log(TerrainGridManager.Instance.GetProductionSum(ResourceType.Wood));

        //WoodProduction.text = TerrainGridManager.Instance.GetProductionSum(ResourceType.Wood).ToString();
        //IronProduction.text = TerrainGridManager.Instance.GetProductionSum(ResourceType.Iron).ToString(); ;
        //MudProduction.text = TerrainGridManager.Instance.GetProductionSum(ResourceType.Mud).ToString();
        //int energyValue = TerrainGridManager.Instance.GetProductionSum(ResourceType.ETypeSolaire) + TerrainGridManager.Instance.GetProductionSum(ResourceType.ETypeWind) + TerrainGridManager.Instance.GetProductionSum(ResourceType.ETypeWater);
        //EnergieProduction.text = energyValue.ToString();

    }




    void UpdateCellDescription()
    {
        
        if (cellDisplay.currentCell.level == 5)
        {
            MaxReached.SetActive(false);
            MaxReachedText.SetActive(true);
        }
        else
        {
            MaxReached.SetActive(true);
            MaxReachedText.SetActive(false);    
        }

        // cellDisplay.UpdateCellData(cellDisplay.currentCell.index);
        //updateCell(cellDisplay.currentCell.index);
        cellDisplay.currentCell.CalculateMaterialsNeededForNextUpgrade();
        cellDescription.text = cellDisplay.currentCell.regionData.RegionDescription;
        woodRequired.text = cellDisplay.currentCell.materialsNeeded[0].ToString();
        IronRequired.text = cellDisplay.currentCell.materialsNeeded[1].ToString();
        MudRequired.text = cellDisplay.currentCell.materialsNeeded[2].ToString();
        EnergeyRequired.text = cellDisplay.currentCell.materialsNeeded[3].ToString();

    }



    public void IsUpgradeable(TerrainCellData cell)
    {
        int woodReserve = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Wood);
        int ironReserve = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Iron);
        int mudReserve = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Mud);
        int energieReserve = ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeSolaire) + ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeWind) + ResourceManager.Instance.GetResourceAmountProduction(ResourceType.ETypeWater);
 


        if (cell.materialsNeeded[0] < woodReserve 
            && cell.materialsNeeded[1] < ironReserve 
            && cell.materialsNeeded[2] < mudReserve 
            && cell.materialsNeeded[3] < loginmanager.LoadedPlayerData.storageenergie)
        {        
            upgrade(cell);         
        }
        else
        {
            StartCoroutine(feedbackOn(feedbackneg));
        }


    }

    public void UpgradeRoutine(TerrainCellData cell)
    {
        cell.CalculateMaterialsNeededForNextUpgrade();
        ResourceManager.Instance.SubtractResourceStorage(ResourceType.Wood, cell.materialsNeeded[0]);
        ResourceManager.Instance.SubtractResourceStorage(ResourceType.Iron, cell.materialsNeeded[1]);
        ResourceManager.Instance.SubtractResourceStorage(ResourceType.Mud, cell.materialsNeeded[2]);
        ResourceManager.Instance.SubtractResourceStorage(ResourceType.ETypeSolaire, cell.materialsNeeded[3]);
        cell.level++;
        
        StartCoroutine(feedbackOn(feedbackpos));
        if (cell.regionData.resourceType == ResourceType.Wood
            || cell.regionData.resourceType == ResourceType.Mud
            || cell.regionData.resourceType == ResourceType.Iron)
        {
            ResourceManager.Instance.AddResourceRate(cell.regionData.resourceType, cell.regionData.productionRateBase);
        }
        else
        {


            ResourceManager.Instance.AddResourceRate(ResourceType.ETypeSolaire, cell.regionData.productionRateBase);
        }
        UpdateCellDescription();
        UpdateUiStorage();

        StartCoroutine(UpdateCellDataRequest(cellDisplay.currentCell._id, cell.level));
    }

    public void upgrade(TerrainCellData cell)
    {
        Debug.Log(cell._id);
        if (cell.state && cell.level <= 5 && cell.canUpgrade)
        {          
            if (cell.level == 4)
            {
                UpgradeRoutine(cell);
                TerrainGridManager.Instance.OpenNewCell(cell);
                TerrainGridManager.Instance.ActiveCellsInRegion(cell.regionData.index);
            }
            else
            {
                UpgradeRoutine(cell);              
                StartCoroutine(TimerOn(cell));
                cell.canUpgrade = false;
            }
        }
        else
        {
            StartCoroutine(feedbackOn(feedbackneg));
        }

    }

    IEnumerator feedbackOn(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(2f);
        go.SetActive(false);
    }




    IEnumerator TimerOn(TerrainCellData cell)
    {
        int timetowait = cell.level * 30;
        LevelUpButton.gameObject.SetActive(false);
        Clock.SetActive(true);

        while (timeElapsed < timetowait)
        {
            yield return new WaitForSeconds(1);
            timeElapsed++;

            int remainingSeconds = timetowait - timeElapsed;
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            if (remainingSeconds >= 60)
            {
                clokTime.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }
            else
            {
                clokTime.text = seconds.ToString("00:00");
            }
        }

        timeElapsed = 0;
        cell.canUpgrade = true;
        Clock.SetActive(false);
        LevelUpButton.gameObject.SetActive(true);
    }

    public void SkipWaiting(TerrainCellData cell)
    {
        if (!cell.canUpgrade)
        {
            StopCoroutine("TimerOn"); // Stop the coroutine
            timeElapsed = cell.level * 30; // Set timeElapsed to its maximum value
            cell.canUpgrade = true; // Set canUpgrade to true to enable upgrading
            Clock.SetActive(false); // Hide the clock
            LevelUpButton.gameObject.SetActive(true); // Show the LevelUpButton
            ResourceManager.Instance.Gem -= 50;
            UpdateGoldOnServer(50);// Assuming you deduct 50 gems for skipping
            Gem.text = ResourceManager.Instance.Gem.ToString();
            cellDisplay.UpdateCellData(cellDisplay.currentCell.index);
        }
    }

    public void UpdateStorageAttributesOnServer()
    {
        // Ensure you have a method to get the username correctly
        string username = loginmanager.LoadedPlayerData.username;
        



        StorageUpdateData data = new StorageUpdateData
        {
            storagewood = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Wood),
            storagemud = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Mud),
            storageclay = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Iron), 
            storageenergie = ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeSolaire)
                           + ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeWind)
                           + ResourceManager.Instance.GetResourceAmountStorage(ResourceType.ETypeWater)
        };
        WoodStorage.text = data.storagewood.ToString();
        IronStorage.text = data.storageclay.ToString();
        MudStorage.text = data.storagemud.ToString();
        EnergieStorage.text = data.storageenergie.ToString();

        StartCoroutine(UpdateStorageAttributesCoroutine(username, JsonUtility.ToJson(data)));
    }

    IEnumerator UpdateStorageAttributesCoroutine(string username, string jsonData)
    {
        string url = $"http://localhost:9090/players/{username}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Storage attributes updated successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Failed to update storage attributes: " + request.error);
        }
    }
    public void UpdateGoldOnServer(int goldChange)
    {
        string username = loginmanager.LoadedPlayerData.username; 
        StartCoroutine(UpdateGoldCoroutine(username, goldChange));
    }

    IEnumerator UpdateGoldCoroutine(string username, int goldChange)
    {
        string url = $"http://localhost:9090/players/{username}";
            UpdatePlayerData playerData = new UpdatePlayerData();
            playerData.gold = loginmanager.LoadedPlayerData.gold - goldChange;

        string jsonData = JsonUtility.ToJson(playerData);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Gold updated successfully on server.");
        }
        else
        {
            Debug.LogError($"Failed to update gold on server: {request.error}");
        }
    }

    IEnumerator UpdateCellDataRequest(string cellId,int _level)
    {
        Debug.Log("id : " + cellId + "level" + _level);
        string url = "http://127.0.0.1:9090/cell/" + cellId;

        UpdateCellData cell = new UpdateCellData();
        cell.level = _level;
        cell.state = cellDisplay.currentCell.state;
        cell.productivite = cellDisplay.currentCell.regionData.productionRateBase * _level;




        string jsonData = JsonUtility.ToJson(cell);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if(_level == 5)
            {
                cellDisplay.UpdateCellData(cellDisplay.currentCell.index);
            }
            Debug.Log("Gold updated successfully on server.");         
        }
        else
        {
            Debug.LogError($"Failed to update cell on server: {request.error}");
        }
    }

    public void GetPlayerStorage()
    {
        StartCoroutine(GetPlayerStorageCoroutine());
    }

    IEnumerator GetPlayerStorageCoroutine()
    {
        string url = $"http://localhost:9090/getOnce/{CurrentUserManager.Instance.GetCurrentUserId()}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            playerStorageData = JsonUtility.FromJson<StorageUpdateData>(json);

            ResourceManager.Instance.AddResourceStorage(ResourceType.Wood, playerStorageData.storagewood);
            ResourceManager.Instance.AddResourceStorage(ResourceType.Iron, playerStorageData.storageclay);
            ResourceManager.Instance.AddResourceStorage(ResourceType.Mud, playerStorageData.storagemud);
            ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeSolaire, playerStorageData.storageenergie);
            ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeWind, playerStorageData.storageenergie);
            ResourceManager.Instance.AddResourceStorage(ResourceType.ETypeWater, playerStorageData.storageenergie);

        }
        else
        {
            Debug.LogError("Failed to retrieve storage data: " + request.error);
        }
    }




}
[System.Serializable]
public class StorageUpdateData
{
    public int storagewood;
    public int storagemud;
    public int storageclay;
    public int storageenergie;
}
[System.Serializable]
public class UpdateCellData
{
    public int level;
    public bool state;
    public int productivite;
}
[System.Serializable]
public class UpdatePlayerData
{
    public int gold;
}