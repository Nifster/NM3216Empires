using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlatformGameManager : MonoBehaviour {

    public bool isPaused = false;
    public GameObject pausePanel;

    [SerializeField]
    private int _citizenCount;

    [SerializeField]
    private int _soldierCount;

    private int _enemyCount;

    public static PlatformGameManager instance;

    public GameObject eventNoticePanel;

    public int eraIndex = 0; //0 is egyptian, 1 is roman, etc;

    

    [System.Serializable]
    public class Buildings
    {
        public Sprite buildingSprite;
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

    [HeaderAttribute("Building Properties")]
    public Buildings House;
    public Buildings Barracks;
    public Buildings Ladder;
    public Buildings Pyramid;
    public Buildings Tree;
    public Buildings Rock;

    [HeaderAttribute("Enemy Spawn Checkpoints")]
    public int firstCheckpoint;
    public int secondCheckpoint;
    //public enum BuildingToBuild
    //{
    //    House,
    //    Barrack,
    //    Ladder
    //}
    [HeaderAttribute("")]
    public int selectedBuildingIndexToBuild = -1; //-1 by default, meaning unselected
    //0 for house, 1 for school etc

    public Buildings selectedBuildingToBuild = null;

    private int _lumberCount;
    private int _oreCount;
    private int _influenceCount;

    public Text lumberText;
    public Text citizenText;
    public Text oreText;
    public Text influenceText;

    public List<Citizen> citizenPool;
    public List<Soldier> soldierPool;
    public List<Enemy> enemyPool;

    [SerializeField]
    private GameObject citizenPrefab;

    [SerializeField]
    private GameObject soldierPrefab;

    [SerializeField]
    private GameObject enemyPrefab;

    public List<SlotScript> ladderSlots;

    void Awake()
    {
        instance = this;
        selectedBuildingToBuild = null;
    }
	// Use this for initialization
	void Start () {
        
        _lumberCount = 0;
        _oreCount = 0;
        _influenceCount = 0;
        pausePanel.SetActive(false);
        //need to initialise the map, with coords

        //initialize citizen pool
        for (int i=0; i<_citizenCount; i++)
        {
            GameObject citizenObj = Instantiate(citizenPrefab);
            //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            citizenObj.transform.localPosition = new Vector3(0, -1.9f,-1); //also temp
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
            soldierObj.transform.localPosition = new Vector3(0, -1.9f, -1); //also temp
            Soldier newSoldier = soldierObj.GetComponent<Soldier>();
            soldierPool.Add(newSoldier);
            newSoldier.isBusy = false;
        }

        //initialize enemy pool
        for(int i=0;i<_enemyCount; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab);
            enemyObj.transform.localPosition = new Vector3(0, -500, -1); //also temp, offscreen
            Enemy newEnemy = enemyObj.GetComponent<Enemy>();
            enemyPool.Add(newEnemy);
            newEnemy.isBusy = false;
        }

        eventNoticePanel.SetActive(false);


    }

    // Update is called once per frame
    void Update () {
        lumberText.text = _lumberCount.ToString();
        citizenText.text = _citizenCount.ToString();
        oreText.text = _oreCount.ToString();
        influenceText.text = _influenceCount.ToString();

        //pause check
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeButton()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Menu");
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
            citizenObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1); //also temp
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
            soldierObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1); //also temp
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
            case (3):
                selectedBuildingToBuild = Pyramid;
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

        //do a check on influence to see if enemy soldiers should come
        EnemyWaveCheck();
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

    public void EnemyWaveCheck()
    {
        //check era
        if(eraIndex == 0)
        {
            //Egyptian
            if (_influenceCount == firstCheckpoint)
            {
                //spawn 2 enemies
                SpawnEnemies(2);

            }
            else if (_influenceCount == secondCheckpoint)
            {
                //spawn 2 enemies
                SpawnEnemies(2);
            }
        }
        
    }

    public void SpawnEnemies(int enemyCount)
    {
        int enemyLeftToSpawn = enemyCount;
        PlatformMapScript.Point spawnPoint = new PlatformMapScript.Point(0, 0);
        //check pool if got enough, if not instantiate
        for(int i=0; i < enemyCount; i++)
        {
            //find non-busy
            for(int j = 0; j < enemyPool.Count; j++)
            {
                if (!enemyPool[j].isBusy)
                {
                    enemyPool[j].isBusy = true;
                    enemyPool[j].transform.localPosition = new Vector3(spawnPoint.PointToCoord().x, spawnPoint.PointToCoord().y + 0.6f,-1);
                    enemyLeftToSpawn--;
                }
            }
        }

        if (enemyLeftToSpawn >= 0)
        {
            //if not enough enemy in pool, spawn more
            for(int i = 0; i < enemyLeftToSpawn; i++)
            {
                GameObject enemyObj = Instantiate(enemyPrefab);
                enemyObj.transform.localPosition = new Vector3(spawnPoint.PointToCoord().x, spawnPoint.PointToCoord().y + 0.6f, -1); //also temp, offscreen
                Enemy newEnemy = enemyObj.GetComponent<Enemy>();
                enemyPool.Add(newEnemy);
                newEnemy.isBusy = true;
            }
        }

        //if enemy dies, remember to change isBusy back to false
    }

    
}
