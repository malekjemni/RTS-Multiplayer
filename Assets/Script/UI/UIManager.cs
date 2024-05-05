using TGS;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnscriptedLogic.Experimental.Generation;

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


    public TMP_Text usernameText;

    public int timeElapsed = 0;
    public GameObject feedbackpos;
    public GameObject feedbackneg;
    public GameObject Clock;
    public CellDisplay cellDisplay;

    public Button LevelUpButton;
    public Button SkipButton;
    public Button UpgradeButton;

    public GameObject MaxReached;
    public GameObject MaxReachedText;

    private bool isUpgradeable = false;
    public StorageUpdateData playerStorageData;
    private int lastHighlightedCellIndex = -1;


    private TerrainCellData cellToLevel;

    private void Start()
    {
        Instance = this;
        GetPlayerStorage();
   

       
        tgs = TerrainGridSystem.instance;
        UpdateUiStorage();
        UpdateUiProduction();
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => cellDisplay.UpdateCellData(cellIndex);
        LevelUpButton.onClick.AddListener(() => IsUpgradeable(cellDisplay.currentCell));

        tgs.OnCellHighlight += OnHoverCell;



        Gem.text = ResourceManager.Instance.Gem.ToString();
        usernameText.text = CurrentUserManager.Instance.GetCurrentUsername();
    }

    private void OnHoverCell(TerrainGridSystem sender, int cellIndex, ref bool cancelHighlight)
    {
        if (lastHighlightedCellIndex != -1)
        {
            TerrainGridManager.Instance.HideCellLevel(lastHighlightedCellIndex);
        }
        TerrainGridManager.Instance.ShowCellLevel(cellIndex);
        lastHighlightedCellIndex = cellIndex;
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
    }
    public void UpdateCellDescription(TerrainCellData cell)
    {
        if (cell.level >= 5)
        {
            MaxReached.SetActive(false);
            MaxReachedText.SetActive(true);
        }
        else 
        {
            MaxReached.SetActive(true);
            MaxReachedText.SetActive(false);           
        }
        if(!cell.canUpgrade)
        {
            LevelUpButton.gameObject.SetActive(false);
        }
        else if(!Clock.activeInHierarchy)
        {
            LevelUpButton.gameObject.SetActive(true);
        }

        cell.CalculateMaterialsNeededForNextUpgrade();
        cellDescription.text = cell.regionData.RegionDescription;
        woodRequired.text = cell.materialsNeeded[0].ToString();
        IronRequired.text = cell.materialsNeeded[1].ToString();
        MudRequired.text = cell.materialsNeeded[2].ToString();
        EnergeyRequired.text = cell.materialsNeeded[3].ToString();

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
            && cell.materialsNeeded[3] < playerStorageData.storageenergie)
        {   
            cellToLevel = cell;
            upgrade(cellToLevel);         
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

        if (cell.level >= 5)
        {
            TerrainGridManager.Instance.OpenNewCell(cell);
            TerrainGridManager.Instance.ActiveCellsInRegion(cell.regionData.index);
        }

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
        StartCoroutine(UpdateCellDataRequest(cell._id, cell.level));
        TerrainGridManager.Instance.DrawCellLevelUi(cell);
        TerrainGridManager.Instance.SetCanUpgradeForCell(cell, true);
        cellDisplay.UpdateCellData(cellDisplay.currentCell.index);      
        UpdateUiStorage();
        cellToLevel = null;
    }
    public void upgrade(TerrainCellData cell)
    {
        if (cell.state && cell.level <= 5 && TerrainGridManager.Instance.GetCanUpgradeForCell(cell))
        {
            StartCoroutine(TimerOn(cell));          
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
        Debug.Log("TimerOn");
        int timetowait = cell.level * 30;
        TerrainGridManager.Instance.SetCanUpgradeForCell(cell, false);
        LevelUpButton.gameObject.SetActive(false);
        Clock.SetActive(true);
        SkipButton.interactable = true;
       

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
        Clock.SetActive(false);
        SkipButton.interactable = false;
        LevelUpButton.gameObject.SetActive(true);
        UpgradeRoutine(cell);
    }
    public void SkipWaiting()
    {
        if (cellToLevel.index == cellDisplay.currentCell.index)
        {
            StopCoroutine("TimerOn");
            timeElapsed = cellToLevel.level * 30;
            ResourceManager.Instance.Gem -= 50;
            UpdateGoldOnServer(50);
            Gem.text = ResourceManager.Instance.Gem.ToString();
        }
                  
    }
    public void UpdateStorageAttributesOnServer()
    {
 
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

        StartCoroutine(UpdateStorageAttributesCoroutine(CurrentUserManager.Instance.GetCurrentUsername(), JsonUtility.ToJson(data)));   
    }
    IEnumerator UpdateStorageAttributesCoroutine(string username, string jsonData)
    {
        string url = $"http://localhost:9090/players/{username}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            StorageUpdateData res = JsonUtility.FromJson<StorageUpdateData>(json);

            playerStorageData.storagewood = res.storagewood;
            playerStorageData.storageclay = res.storageclay;
            playerStorageData.storagemud = res.storagemud;
            playerStorageData.storageenergie = res.storageenergie;

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
            StartCoroutine(GetPlayerScore());        
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
            ResourceManager.Instance.SetResourceStorage(ResourceType.Wood, playerStorageData.storagewood);
            ResourceManager.Instance.SetResourceStorage(ResourceType.Iron, playerStorageData.storageclay);
            ResourceManager.Instance.SetResourceStorage(ResourceType.Mud, playerStorageData.storagemud);
            ResourceManager.Instance.SetResourceStorage(ResourceType.ETypeSolaire, playerStorageData.storageenergie);
            ResourceManager.Instance.SetResourceStorage(ResourceType.ETypeWind, playerStorageData.storageenergie);
            ResourceManager.Instance.SetResourceStorage(ResourceType.ETypeWater, playerStorageData.storageenergie);
        }
        else
        {
            Debug.LogError("Failed to retrieve storage data: " + request.error);
        }
    }
    IEnumerator UpdatePlayerScore(int score)
    {
        string url = $"http://localhost:9090/players/{CurrentUserManager.Instance.GetCurrentUsername()}";
        PlayerDataScore playerData = new PlayerDataScore();
        playerData.score = loginmanager.LoadedPlayerData.score + score;


        string jsonData = JsonUtility.ToJson(playerData);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score updated successfully on server.");
        }
        else
        {
            Debug.LogError($"Failed to update Score on server: {request.error}");
        }
    }
    IEnumerator GetPlayerScore()
    {
        string url = $"http://localhost:9090/getOnce/{CurrentUserManager.Instance.GetCurrentUserId()}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            loginmanager.LoadedPlayerData.score = JsonUtility.FromJson<PlayerData>(json).score;
            StartCoroutine(UpdatePlayerScore(1));
        }
        else
        {
            Debug.LogError("Failed to retrieve storage data: " + request.error);
        }
    }

    public void KeepStorageCheckOnServer()
    {
        StartCoroutine(KeepStorageCheck());
    }
    IEnumerator KeepStorageCheck()
    {
        string url = $"http://localhost:9090/getOnce/{CurrentUserManager.Instance.GetCurrentUserId()}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            playerStorageData = JsonUtility.FromJson<StorageUpdateData>(json);
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
[System.Serializable]
public class PlayerDataScore
{
    public int score;
}