using UnityEngine;
[System.Serializable]
public class TerrainCellData
{
    public RegionData regionData;
    public string region;
    public bool state;
    public int level;
    public int productivite;
    public string owner;
    public int productionRate;

    public string _id;
    public int index;
    public int[] materialsNeeded;
    public bool canUpgrade = true;



    public TerrainCellData()
    {
        materialsNeeded = new int[4];
    }

    // Method to calculate the materials needed for the next upgrade
    public int[] CalculateMaterialsNeededForNextUpgrade()
    {
        // Example: Define materials needed for each level
      if(level==0)
        {
          
            switch (regionData.resourceType)
            {
                case ResourceType.ETypeSolaire:
                    materialsNeeded[0] = 50; // Wood
                    materialsNeeded[1] = 30; // Iron
                    materialsNeeded[2] = 200; // Mud
                    materialsNeeded[3] = 250; // Energy

                    break;
                case ResourceType.Iron:
                    materialsNeeded[0] = 50; // Wood
                    materialsNeeded[1] = 70; // Iron
                    materialsNeeded[2] = 200; // Mud
                    materialsNeeded[3] = 100; // Energy

                    break;
                case ResourceType.ETypeWater:
                    materialsNeeded[0] = 50; // Wood
                    materialsNeeded[1] = 30; // Iron
                    materialsNeeded[2] = 200; // Mud
                    materialsNeeded[3] = 250; // Energy

                    break;
                case ResourceType.Mud:
                    materialsNeeded[0] = 50; // Wood
                    materialsNeeded[1] = 30; // Iron
                    materialsNeeded[2] = 800; // Mud
                    materialsNeeded[3] = 100; // Energy


                    break;
                case ResourceType.Wood:
                    materialsNeeded[0] = 200; // Wood
                    materialsNeeded[1] = 30; // Iron
                    materialsNeeded[2] = 200; // Mud
                    materialsNeeded[3] = 100; // Energy

                    break;
                case ResourceType.ETypeWind:
                    materialsNeeded[0] = 50; // Wood
                    materialsNeeded[1] = 30; // Iron
                    materialsNeeded[2] = 200; // Mud
                    materialsNeeded[3] = 250; // Energy


                    break;
              
            }
        }

    
        else
        {
            switch (regionData.resourceType)
            {
                case ResourceType.ETypeSolaire:
                    materialsNeeded[0] = 50 * level ; // Wood
                    materialsNeeded[1] = 30 * level ;// Iron
                    materialsNeeded[2] = 200 * level ;// Mud
                    materialsNeeded[3] = 250 * level ;// Energy

                    break;
                case ResourceType.Iron:
                    materialsNeeded[0] = 50 * level ; // Wood
                    materialsNeeded[1] = 70 * level ; // Iron
                    materialsNeeded[2] = 200 * level ; // Mud
                    materialsNeeded[3] = 100 * level ; // Energy

                    break;
                case ResourceType.ETypeWater:
                    materialsNeeded[0] = 50 * level ; // Wood
                    materialsNeeded[1] = 30 * level ; // Iron
                    materialsNeeded[2] = 200 * level ; // Mud
                    materialsNeeded[3] = 250 * level ; // Energy

                    break;
                case ResourceType.Mud:
                    materialsNeeded[0] = 50 * level ; // Wood
                    materialsNeeded[1] = 30 * level ; // Iron
                    materialsNeeded[2] = 800 * level ; // Mud
                    materialsNeeded[3] = 100 * level ; // Energy


                    break;
                case ResourceType.Wood:
                    materialsNeeded[0] = 200 * level ; // Wood
                    materialsNeeded[1] = 30 * level ; // Iron
                    materialsNeeded[2] = 200 * level ; // Mud
                    materialsNeeded[3] = 100 * level ; // Energy

                    break;
                case ResourceType.ETypeWind:
                    materialsNeeded[0] = 50 * level ; // Wood
                    materialsNeeded[1] = 30 * level ; // Iron
                    materialsNeeded[2] = 200 * level ; // Mud
                    materialsNeeded[3] = 250 * level ; // Energy


                    break;

            }
        }
     
        return materialsNeeded;
    }





}
