using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceManager : MonoBehaviour
{

    public static event Action<int> OnGemChange; // Événement signalant les changements dans la valeur des gemmes
    public int Gem { get; set; } = 1000; // Propriété pour la valeur des gemmes

    // Méthode pour modifier la valeur des gemmes et déclencher l'événement
    public void SetGem(int newValue)
    {
        Gem = newValue;
        OnGemChange?.Invoke(newValue); // Déclencher l'événement avec la nouvelle valeur des gemmes
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
   // public int Gem = 1000;

    private void Awake()
    {
        Instance = this;
        InitializeResourceAmounts();
        InitializeMaxResourceAmounts();
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
                Debug.Log("Resource amount capped at maximum: " + type);
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

    //Production Handler
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

}
