using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlatformGameManager : MonoBehaviour {

    [SerializeField]
    private int _citizenCount;

    [SerializeField]
    private int _soldierCount;
    public static PlatformGameManager instance;

    public GameObject eventNoticePanel;

    [HeaderAttribute("Buildings")]
    public GameObject housePrefab;
    public GameObject schoolPrefab;
    public GameObject ladderPrefab;

    

    [System.Serializable]
    public class Buildings
    {
        public GameObject prefab;
        public int labourCost;
        public int lumberCost;
        public int oreCost;
        public int influenceCost; //only for monuments
        public int influenceReward;
        public float timeToBuild;
    }

    [HeaderAttribute("House Attributes")]
    public int CITIZEN_PER_HOUSE;

    [HeaderAttribute("Barracks Attributes")]
    public int SOLDIERS_PER_BARRACKS;

    public Buildings House;
    public Buildings Barracks;
    public Buildings Ladder;
    public Buildings Pyramid;


    //public enum BuildingToBuild
    //{
    //    House,
    //    Barrack,
    //    Ladder
    //}
    [HeaderAttribute("")]
    public int selectedBuildingIndexToBuild = -1; //-1 by default, meaning unselected
    //0 for house, 1 for school etc

    public Buildings selectedBuildingToBuild;

    private int _lumberCount;
    private int _oreCount;
    private int _influenceCount;

    public Text lumberText;
    public Text citizenText;
    public Text oreText;
    public Text influenceText;

    public List<Citizen> citizenPool;
    public List<Soldier> soldierPool;

    [SerializeField]
    private GameObject citizenPrefab;

    [SerializeField]
    private GameObject soldierPrefab;

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

        //initiate soldier pool
        for (int i = 0; i < _soldierCount; i++)
        {
            GameObject soldierObj = Instantiate(soldierPrefab);
            //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            soldierObj.transform.localPosition = new Vector3(0, -1.7f, -1); //also temp
            Soldier newSoldier = soldierObj.GetComponent<Soldier>();
            soldierPool.Add(newSoldier);
            newSoldier.isBusy = false;
        }

        eventNoticePanel.SetActive(false);


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

    public void AddSoldier(Vector3 pos)
    {
        for (int i = 0; i < SOLDIERS_PER_BARRACKS; i++)
        {
            GameObject soldierObj = Instantiate(soldierPrefab);
            //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            soldierObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.8f, pos.z - 1); //also temp
            Soldier newSoldier = soldierObj.GetComponent<Soldier>();
            soldierPool.Add(newSoldier);
            newSoldier.isBusy = false;
            _soldierCount++;
        }
    }

    public Soldier GetSoldier(PlatformMapScript.Point slotPoint)
    {
        float closestX = 10000;
        bool soldierFound = false;
        bool ladderFound = false;
        PlatformMapScript.Point ladderPoint = null;
        Soldier foundSoldier = null;
        //search through pool for free citizen
        for (int i = 0; i < _soldierCount; i++)
        {
            if (!soldierPool[i].isBusy)
            {
                //y check
                if (soldierPool[i].pointY == slotPoint.y)
                {
                    if (Mathf.Abs(soldierPool[i].pointX - slotPoint.x) < closestX)
                    {
                        soldierFound = true;
                        closestX = Mathf.Abs(soldierPool[i].pointX - slotPoint.x);
                        foundSoldier = soldierPool[i];
                        Debug.Log("Same level found");
                    }

                }
            }
        }

        //if cannot find citizen on same level, get citizen nearest to ladder
        closestX = 10000;
        if (!soldierFound)
        {
            if (!ladderFound)
            {
                for (int j = 0; j < ladderSlots.Count; j++)
                {
                    if (Mathf.Abs(slotPoint.x - ladderSlots[j].point.x) < closestX)
                    {
                        ladderFound = true;
                        closestX = Mathf.Abs(slotPoint.x - ladderSlots[j].point.x);
                        ladderPoint = ladderSlots[j].point;
                        Debug.Log("Ladder found");
                    }
                }
            }
            closestX = 10000;
            if (ladderFound)
            {
                //closest ladder found
                for (int i = 0; i < _soldierCount; i++)
                {
                    if (!soldierPool[i].isBusy)
                    {

                        if (Mathf.Abs(ladderPoint.x - soldierPool[i].pointX) < closestX)
                        {
                            soldierFound = true;
                            closestX = Mathf.Abs(ladderPoint.x - soldierPool[i].pointX);
                            foundSoldier = soldierPool[i];
                            Debug.Log("Other level found");
                        }

                    }
                }
            }
        }

        return foundSoldier;
    }
    public Citizen GetCitizen(PlatformMapScript.Point slotPoint)
    {
        float closestX = 10000;
        bool citizenFound = false;
        bool ladderFound = false;
        PlatformMapScript.Point ladderPoint = null;
        Citizen foundCitizen = null;
        //search through pool for free citizen
        for(int i=0; i<_citizenCount; i++)
        {
            if (!citizenPool[i].isBusy)
            {   
                //y check
                if(citizenPool[i].pointY == slotPoint.y)
                {
                    if (Mathf.Abs(citizenPool[i].pointX - slotPoint.x) < closestX )
                    {
                        citizenFound = true;
                        closestX = Mathf.Abs(citizenPool[i].pointX - slotPoint.x);
                        foundCitizen = citizenPool[i];
                        Debug.Log("Same level found");
                    }
                    
                }
            }
        }
        
        //if cannot find citizen on same level, get citizen nearest to ladder
        closestX = 10000;
        if (!citizenFound)
        {
            if (!ladderFound)
            {
                for (int j = 0; j < ladderSlots.Count; j++)
                {
                    if (Mathf.Abs(slotPoint.x - ladderSlots[j].point.x) < closestX)
                    {
                        ladderFound = true;
                        closestX = Mathf.Abs(slotPoint.x - ladderSlots[j].point.x);
                        ladderPoint = ladderSlots[j].point;
                        Debug.Log("Ladder found");
                    }
                }
            }
            closestX = 10000;
            if (ladderFound)
            {
                //closest ladder found
                for (int i = 0; i < _citizenCount; i++)
                {
                    if (!citizenPool[i].isBusy)
                    {
                        
                        if (Mathf.Abs(ladderPoint.x- citizenPool[i].pointX) < closestX)
                        {
                            citizenFound = true;
                            closestX = Mathf.Abs(ladderPoint.x - citizenPool[i].pointX);
                            foundCitizen = citizenPool[i];
                            Debug.Log("Other level found");
                        }

                    }
                }
            }
        }

        return foundCitizen;
    }

    public void PickBuildingToBuild(int buildingIndex)
    {
        selectedBuildingIndexToBuild = buildingIndex;
        switch (buildingIndex)
        {
            case (0):
                selectedBuildingToBuild = House;
                break;
            case (1):
                selectedBuildingToBuild = Barracks;
                break;
            case (2):
                selectedBuildingToBuild = Ladder;
                break;
        }
    }

    public void BuildSelected(GameObject slotToBuildIn)
    {
        //TODO: Resource check should be done before citizen goes to slot
        if (selectedBuildingIndexToBuild == 0)
        {
            //check resources
            if(ResourceCheck(House))
            {

                //build house
                GameObject newBuilding = Instantiate(House.prefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 0.8f, 0);
                //add citizen at spot
                AddCitizen(slotToBuildIn.transform.position);
                SpendResources(House);
            }
            
        }
        else if (selectedBuildingIndexToBuild == 1)
        {
            //barrack
            if (ResourceCheck(Barracks))
            {

                //build barracks
                GameObject newBuilding = Instantiate(Barracks.prefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1.05f, 0);
                //add soldiers at spot
                AddSoldier(slotToBuildIn.transform.position);

                SpendResources(Barracks);
            }
        }
        else if (selectedBuildingIndexToBuild == 2)
        {
            //ladder
            if (ResourceCheck(Ladder))
            {
                GameObject newBuilding = Instantiate(Ladder.prefab);
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

        }else if(selectedBuildingIndexToBuild == 3)
        {
            //monument
            if (ResourceCheck(Pyramid))
            {
                GameObject newBuilding = Instantiate(Pyramid.prefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1.0f, 0);
                SpendResources(Pyramid);
                NextEra();
            }
        }
        //check if resource req for selected building is met
        //builds building
        //tells GameManager to remove req resources from count
        //reset building selected
        selectedBuildingIndexToBuild = -1;
        selectedBuildingToBuild = null;
    }

    public bool ResourceCheck(Buildings buildingType)
    {
        bool result = false;

        if(_lumberCount >= buildingType.lumberCost &&
            _citizenCount >= buildingType.labourCost &&
            _oreCount >= buildingType.oreCost &&
            _influenceCount >= buildingType.influenceCost)
        {
            result = true;
        }

        Debug.Log("Resource check satisfied: " + result);
        return result;
    }

    public void SpendResources(Buildings buildingType)
    {
        _lumberCount -= buildingType.lumberCost;
        _oreCount -= buildingType.oreCost;
        _influenceCount -= buildingType.influenceCost;
        _influenceCount += buildingType.influenceReward;
        
    }
    
    public void NextEra()
    {
        //placeholder
        Debug.Log("This era is over! Next level!");
    }
}
