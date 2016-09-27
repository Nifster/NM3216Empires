﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlatformGameManager : MonoBehaviour {

    [SerializeField]
    private int _citizenCount; 
    public static PlatformGameManager instance;

    [HeaderAttribute("Buildings")]
    public GameObject housePrefab;
    public GameObject schoolPrefab;
    public GameObject ladderPrefab;

    

    [System.Serializable]
    public class Buildings
    {
        public int labourCost;
        public int lumberCost;
        public int oreCost;
        public int influenceReward;
    }

    [HeaderAttribute("House Attributes")]
    public int CITIZEN_PER_HOUSE;

    public Buildings House;
    public Buildings Barracks;
    public Buildings Ladder;


    //public enum BuildingToBuild
    //{
    //    House,
    //    Barrack,
    //    Ladder
    //}
    [HeaderAttribute("")]
    public int selectedBuildingToBuild = -1; //-1 by default, meaning unselected
    //0 for house, 1 for school etc

    private int _lumberCount;
    private int _oreCount;
    private int _influenceCount;

    public Text lumberText;
    public Text citizenText;
    public Text oreText;
    public Text influenceText;

    public List<Citizen> citizenPool;

    [SerializeField]
    private GameObject citizenPrefab;

    public List<SlotScript> ladderSlots;
	// Use this for initialization
	void Start () {
        instance = this;
        _lumberCount = 0;
        _oreCount = 0;
        _influenceCount = 0;

        //need to initialise the map, with coords

        //initialize citizen pool
        for(int i=0; i<_citizenCount; i++)
        {
            GameObject citizenObj = Instantiate(citizenPrefab);
            //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            citizenObj.transform.localPosition = new Vector3(0, -1.7f,-1); //also temp
            Citizen newCitizen = citizenObj.GetComponent<Citizen>();
            citizenPool.Add(newCitizen);
            newCitizen.isBusy = false;
        }
    }

    // Update is called once per frame
    void Update () {
        lumberText.text = _lumberCount.ToString();
        citizenText.text = _citizenCount.ToString();
        oreText.text = _oreCount.ToString();
        influenceText.text = _influenceCount.ToString();
    }

    public void HouseBuilt()
    {
        //if citizenCount > 2?
        _citizenCount++;
    }

    public void TreeHarvested()
    {
        _lumberCount++;
        //update lumber text
    }

    public void RockHarvested()
    {
        _oreCount++;
        //update lumber text
    }

    public void AddCitizen(Vector3 pos)
    {
        for(int i=0; i< CITIZEN_PER_HOUSE; i++)
        {
            GameObject citizenObj = Instantiate(citizenPrefab);
            //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            citizenObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.8f, pos.z - 1); //also temp
            Citizen newCitizen = citizenObj.GetComponent<Citizen>();
            citizenPool.Add(newCitizen);
            newCitizen.isBusy = false;
            _citizenCount++;
        }
        
    }

    public Citizen GetCitizen()
    {
        
        //search through pool for free citizen
        for(int i=0; i<_citizenCount; i++)
        {
            if (!citizenPool[i].isBusy)
            {
                return citizenPool[i];
            }
        }
        return null;
    }

    public void PickBuildingToBuild(int buildingIndex)
    {
        selectedBuildingToBuild = buildingIndex;
    }

    public void BuildSelected(GameObject slotToBuildIn)
    {
        if (selectedBuildingToBuild == 0)
        {
            //check resources
            if(ResourceCheck(House))
            {

                //build house
                GameObject newBuilding = Instantiate(housePrefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 0.8f, 0);
                //add citizen at spot
                AddCitizen(slotToBuildIn.transform.position);
                SpendResources(House);
            }
            
        }
        else if (selectedBuildingToBuild == 1)
        {
            //barrack
        }
        else if (selectedBuildingToBuild == 2)
        {
            //ladder
            if (ResourceCheck(Ladder))
            {
                GameObject newBuilding = Instantiate(PlatformGameManager.instance.ladderPrefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                //newBuilding.transform.localScale = transform.localScale;
                newBuilding.transform.localPosition = new Vector3(0, 1.6f, 0);
                //save position of ladder for later reference
                SlotScript slotToBuildInScript = slotToBuildIn.GetComponent<SlotScript>();
                PlatformMapScript.Point slotPoint = slotToBuildInScript.point;
                ladderSlots.Add(slotToBuildInScript);
                SpendResources(Ladder);
                ////if slot y is not 2, add the slot on top as well
                //if(slotToBuildIn.GetComponent<SlotScript>().point.y < 2.0f)
                //{
                //    ladderSlots.Add(PlatformMapScript.instance.slotArray[(int)slotPoint.y+1, (int)slotPoint.x].GetComponent<SlotScript>());
                //}
            }

        }
        //check if resource req for selected building is met
        //builds building
        //tells GameManager to remove req resources from count
        //reset building selected
        selectedBuildingToBuild = -1;
    }

    public bool ResourceCheck(Buildings buildingType)
    {
        bool result = false;

        if(_lumberCount >= buildingType.lumberCost &&
            _citizenCount >= buildingType.labourCost &&
            _oreCount >= buildingType.oreCost)
        {
            result = true;
        }
        return result;
    }

    public void SpendResources(Buildings buildingType)
    {
        _lumberCount -= buildingType.lumberCost;
        _oreCount -= buildingType.oreCost;
        _influenceCount += buildingType.influenceReward;
    }
    

}
