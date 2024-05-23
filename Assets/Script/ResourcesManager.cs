using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ResourceManager : MonoBehaviour
{

    public static event Action<int> OnGemChange; 
    public int Gem { get; set; } = 1000; 

    public void SetGem(int newValue)
    {
        Gem = newValue;
        OnGemChange?.Invoke(newValue); 
    }
   
    public static ResourceManager Instance;
    public static Action OnStorageChange;
    public static Action OnProductionChange;

    private Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();//storage
    private Dictionary<ResourceType, int> resourceProduction = new Dictionary<ResourceType, int>();//production
    private Dictionary<ResourceType, int> maxResourceAmounts = new Dictionary<ResourceType, int>();//Maxamounts


    public int MaxCapacityForETypeSolaire = 10000;
    public int MaxCapacityForIron = 20000;
    public int MaxCapacityForMud = 30000;
    public int MaxCapacityForWood = 25000;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
         InitializeResourceAmounts();
        StartCoroutine(GetMaxStorageCoroutine());       
    }
    private void Start()
    {
        InvokeRepeating("ProductionIncome", 0.1f, 2f);//Production0.1
    }
    void ProductionIncome()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            AddResourceStorage(type, GetResourceAmountProduction(type));
        }
    }
    private void InitializeResourceAmounts()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            switch (type)
            {
                case ResourceType.ETypeSolaire:
                    resourceAmounts.Add(type, 0);
                    resourceProduction.Add(type, 25); 
                    break;
                case ResourceType.Iron:
                    resourceAmounts.Add(type,0);
                    resourceProduction.Add(type, 5);
                    break;
                case ResourceType.ETypeWater:
                    resourceAmounts.Add(type, 0);
                    resourceProduction.Add(type, 25);
                    break;
                case ResourceType.Mud:
                    resourceAmounts.Add(type, 0);
                    resourceProduction.Add(type, 75);
                    break;
                case ResourceType.Wood:
                    resourceAmounts.Add(type, 0);
                    resourceProduction.Add(type, 100);
                    break;
                case ResourceType.ETypeWind:
                    resourceAmounts.Add(type, 0);
                    resourceProduction.Add(type, 25);
                    break;
              
            }
        }
    }
    private void InitializeMaxResourceAmounts()
    {
        maxResourceAmounts.Add(ResourceType.Wood, MaxCapacityForWood); 
        maxResourceAmounts.Add(ResourceType.Mud, MaxCapacityForMud);
        maxResourceAmounts.Add(ResourceType.ETypeSolaire, MaxCapacityForETypeSolaire);
        maxResourceAmounts.Add(ResourceType.Iron, MaxCapacityForIron);
    }

    public void AddGem(int amount)
    {
        Gem += amount;
        OnGemChange?.Invoke(Gem);
    }
    public void SubtractGem(int amount)
    {
        Gem -= amount;
        OnGemChange?.Invoke(Gem);
    }
    //storage handler
    public void AddResourceStorage(ResourceType type, int amount)
    {

        if (resourceAmounts.ContainsKey(type) && maxResourceAmounts.ContainsKey(type))
        {
            int maxAmount = maxResourceAmounts[type];
            
            // Check if adding the amount would exceed the maximum allowed value
            if (resourceAmounts[type] + amount <= maxAmount)
            {
                resourceAmounts[type] += amount;
                OnStorageChange?.Invoke();
            }
            else
            {
                // Cap the resource amount at the maximum allowed value
                resourceAmounts[type] = maxAmount;
                OnStorageChange?.Invoke();
                //Debug.Log("Resource amount capped at maximum: " + type);
            }
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }

    public void SubtractResourceStorage(ResourceType type, int amount)
    {
        if (resourceAmounts.ContainsKey(type))
        {
            resourceAmounts[type] -= amount;
            if (resourceAmounts[type] < 0)
            {
                resourceAmounts[type] = 0;
                OnStorageChange?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }

    public int GetResourceAmountStorage(ResourceType type)
    {
        if (resourceAmounts.ContainsKey(type))
        {
            return resourceAmounts[type];
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
            return 0;
        }
    }

    public void AddResourceRate(ResourceType type, int amount)
    {
        if (resourceProduction.ContainsKey(type))
        {
            resourceProduction[type] += amount;
            OnProductionChange?.Invoke();
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }
    public void SetResourceRate(ResourceType type, int value)
    {
        if (resourceProduction.ContainsKey(type))
        {
            resourceProduction[type] = value;
            OnProductionChange?.Invoke();
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }
    public void SetResourceStorage(ResourceType type, int value)
    {
        if (resourceAmounts.ContainsKey(type) && maxResourceAmounts.ContainsKey(type))
        {
            int maxAmount = maxResourceAmounts[type];
            if (value >= 0 && value <= maxAmount)
            {
                resourceAmounts[type] = value;
                OnStorageChange?.Invoke();
            }
            else if (value < 0)
            {
                resourceAmounts[type] = 0;
                OnStorageChange?.Invoke();
                Debug.LogWarning("Resource amount set to zero: " + type);
            }
            else
            {
                resourceAmounts[type] = maxAmount;
                OnStorageChange?.Invoke();
                Debug.LogWarning("Resource amount capped at maximum: " + type);
            }
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }

    public void UpgradeResourceStorage(ResourceType type, int additionalCapacity)
    {
        switch (type)
        {
            case ResourceType.Wood:
                MaxCapacityForWood += additionalCapacity;
                break;
            case ResourceType.Mud:
                MaxCapacityForMud += additionalCapacity;
                break;
            case ResourceType.ETypeSolaire:
                MaxCapacityForETypeSolaire += additionalCapacity;
                break;
            case ResourceType.Iron:
                MaxCapacityForIron += additionalCapacity;
                break;
            default:
                Debug.LogWarning("Unsupported resource type: " + type);
                return;
        }

        if (maxResourceAmounts.ContainsKey(type))
        {
            maxResourceAmounts[type] += additionalCapacity;
        }
        else
        {
            maxResourceAmounts.Add(type, additionalCapacity);
        }
        if (resourceAmounts.ContainsKey(type))
        {
            if (resourceAmounts[type] > maxResourceAmounts[type])
            {
                resourceAmounts[type] = maxResourceAmounts[type];
            }
        }
        Storage storage = new Storage
        {
            MaxWood = MaxCapacityForWood,
            MaxMud = MaxCapacityForMud,
            MaxClay = MaxCapacityForIron, 
            MaxEnergie = MaxCapacityForETypeSolaire 
        };
        string jsonData = JsonUtility.ToJson(storage);
        StartCoroutine(UpdateStorageAttributesCoroutine(jsonData));
    }
    public void SubtractResourceRate(ResourceType type, int amount)
    {
        if (resourceProduction.ContainsKey(type))
        {
            resourceProduction[type] -= amount;
            if (resourceProduction[type] < 0)
            {
                resourceProduction[type] = 0;
                OnProductionChange?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
        }
    }

    public int GetResourceAmountProduction(ResourceType type)
    {
        if (resourceProduction.ContainsKey(type))
        {
            return resourceProduction[type];
        }
        else
        {
            Debug.LogWarning("Resource not found in dictionary: " + type);
            return 0;
        }
    }

    IEnumerator GetMaxStorageCoroutine()
    {
        string url = $"http://localhost:9090/getOnce/{CurrentUserManager.Instance.GetCurrentUserId()}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Storage starage = JsonUtility.FromJson<Storage>(json);
            MaxCapacityForETypeSolaire = starage.MaxEnergie;
            MaxCapacityForIron = starage.MaxClay;
            MaxCapacityForMud = starage.MaxMud;
            MaxCapacityForWood = starage.MaxWood;

            InitializeMaxResourceAmounts();

        }
        else
        {
            Debug.LogError("Failed to retrieve storage data: " + request.error);
        }
    }
    IEnumerator UpdateStorageAttributesCoroutine(string jsonData)
    {
        string url = $"http://localhost:9090/players/{CurrentUserManager.Instance.GetCurrentUsername()}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Storage starage = JsonUtility.FromJson<Storage>(json);
            MaxCapacityForETypeSolaire = starage.MaxEnergie;
            MaxCapacityForIron = starage.MaxClay;
            MaxCapacityForMud = starage.MaxMud;
            MaxCapacityForWood = starage.MaxWood;

        }
        else
        {
            Debug.LogError("Failed to update storage attributes: " + request.error);
        }
    }

}
public class Storage
{
    public int MaxWood;
    public int MaxMud;
    public int MaxClay;
    public int MaxEnergie;
}

