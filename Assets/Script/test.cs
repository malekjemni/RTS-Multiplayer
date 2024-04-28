using System;
using System.Collections;
using System.Collections.Generic;
using TGS;
using UnityEngine;


public class test : MonoBehaviour
{
    TerrainGridSystem tgs;
 
    // Start is called before the first frame update
    void Start()
    {
        tgs = TerrainGridSystem.instance;


       //Debug.Log( UpgradeHandler.Instance.isUpgradeable(TerrainGridManager.Instance.cellDataGrid[1, 1]));

        //tgs.OnCellClick += (grid, cellIndex, buttonIndex) => Debug.Log("this  "+tgs.TerritoryGetCells(tgs.territoryLastClickedIndex, tgs.territoryRegionLastClickedIndex)[0].index);
        //  tgs.OnCellClick += (grid, cellIndex, buttonIndex) => Debug.Log("this region index is " + tgs.CellGetTerritoryIndex(tgs.cellLastClickedIndex));
        //tgs.OnCellClick += (grid, cellIndex, buttonIndex) => Debug.Log("cell state " + manager.cellDataGrid[tgs.CellGetColumn(tgs.cellLastClickedIndex), tgs.CellGetRow(tgs.cellLastClickedIndex)].state);
        // Debug.Log("cell state " + TerrainGridManager.cellDataGrid[tgs.territoryLastClickedIndex, tgs.territoryRegionLastClickedIndex].state);



        //perminutes
        //public float WoodProductionRate;
        //public float IronProductionRate;
        //public float MudProductionRate;
        //public float SoliaireProductionRate;
        //public float WindProductionRate;
        //public float WaterireProductionRate;

        //Debug.Log(ResourceManager.Instance.GetResourceAmountStorage(ResourceType.Iron));
        //ResourceManager.Instance.AddResourceStorage(ResourceType.Iron, 50);

    }

// Update is called once per frame
void Update()
    {
        //Debug.Log(TerrainGridManager.Instance.GetProductionSum(ResourceType.Wood));

        //Debug.Log(TerrainGridManager.Instance.cellDataGrid[1, 1].CalculateMaterialsNeededForNextUpgrade()[0]);


        //Debug.Log(UpgradeHandler.Instance.isUpgradeable(TerrainGridManager.Instance.cellDataGrid[1, 1]));

    }
}
