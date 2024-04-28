using UnityEngine;

[CreateAssetMenu(fileName = "NewRegionData", menuName = "Region Data")]
public class RegionData : ScriptableObject
{
    public int productionRateBase;
    public int levelUpMultiplier;
    public ResourceType resourceType;
    public string RegionDescription;
    public int activeCells=0;
    public int index;




    private void OnEnable()
    {
  
        switch (resourceType)
        {
            case ResourceType.ETypeSolaire:
                RegionDescription = "Description for Solar region";
                index = 0;
                break;
            case ResourceType.Iron:
                RegionDescription = "Description for Iron region";
                index = 1;
                break;
            case ResourceType.ETypeWater:
                RegionDescription = "Description for Water region";
                index = 4;
                break;
            case ResourceType.Mud:
                RegionDescription = "Description for Mud region";
                index = 5;
                break;
            case ResourceType.Wood:
                RegionDescription = "Description for Wood region";
                index = 3;
                break;
            case ResourceType.ETypeWind:
                RegionDescription = "Description for Wind region";
                index = 2;
                break;
            default:
                RegionDescription = "Unknown region type";
                break;
        }
    }
}

public enum ResourceType
{
    ETypeSolaire,
    Iron,
    ETypeWind,
    Wood,
    ETypeWater,
    Mud,
    
}

