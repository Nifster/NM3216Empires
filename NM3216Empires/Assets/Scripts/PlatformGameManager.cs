using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlatformGameManager : MonoBehaviour {

    public bool isPaused = false;
    public GameObject pausePanel;

    [HeaderAttribute("Next Era Assets")]
    public List<Sprite> eraBackgrounds;
    public List<Sprite> eraOverlays;
    public List<Sprite> citizenSprites;
    public List<Sprite> soldierSprites;
    public List<Sprite> kingSprites;
    public List<Sprite> houseSprites;
    public List<Sprite> barrackSprites;
    public List<Sprite> schoolSprites;
    public List<Sprite> townhallSprites;
    public List<Sprite> monumentSprites;
    public List<Buildings> monumentInfo;
    public List<Sprite> monumentIcon;
    public Sprite activeButtonSprite;
    public List<AudioClip> bgmClips;

    public GameObject backgroundObj;
    public GameObject overlayObj;
    public GameObject kingObj;
    public GameObject monumentButtonObj;
    public GameObject townhallButtonObj;
    public GameObject schoolButtonObj;

    public GameObject gameOverScreen;
    public bool isGameOver;

    [HeaderAttribute("")]

    [SerializeField]
    private int _citizenCount;

    [SerializeField]
    private int _soldierCount;

    public int _currSoldierCount;

    private int _enemyCount;

    public static PlatformGameManager instance;

    public GameObject eventNoticePanel;

    public int eraIndex = 0; //0 is egyptian, 1 is roman, etc;

    public bool demolishMode = false;


    List<SlotScript> houseList;
    List<SlotScript> barracksList;
    List<SlotScript> townHallList;
    int era1houseCount;
    int era2houseCount;
    int era1barracksCount;
    int era2barracksCount;
    int era1townHallCount;
    int era2townHallCount;
    public int oldHouseInfluence;
    public int oldBarracksInfluence;
    public int oldTownhallInfluence;

    int currEnemyCount;

    

    [System.Serializable]
    public class Buildings
    {
        public Sprite buildingSprite;
        public GameObject prefab;
        public int labourCost;
        public int lumberCost;
        public int oreCost;
        public int influenceCost; //only for monuments
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
    public Buildings School;
    public Buildings Townhall;

    [HeaderAttribute("Enemy Spawn Checkpoints")]
    public List<int> firstCheckpoint;
    public List<int> secondCheckpoint;
    public List<int> thirdCheckpoint;
    public List<int> fourthCheckpoint;
    public List<int> enemyWaveSize;
    bool firstEnemyWaveSpawned = false;
    bool secondEnemyWaveSpawned = false;
    bool thirdEnemyWaveSpawned = false;
    bool fourthEnemyWaveSpawned = false;
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
    public Text influenceTextOnes;
    public Text influenceTextTens;
    public Text influenceTextHuns;
    public Text influenceTextThous;
    public Text soldierText;

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

    [HeaderAttribute("Harvest Reward")]
    public int lumberHarvestReward;
    public int oreHarvestReward;

    [HeaderAttribute("Demolish Reward")]
    public int lumberDemolishReward;
    public int oreDemolishReward;

    [HeaderAttribute("Timer")]
    public Text minutes;
    public Text seconds;
    public int maxMinutes = 5;
    int minutesValue = 5;
    float secondsValue;

    [HeaderAttribute("Speech Info")]
    public GameObject kingSpeechObj;
    public Text kingSpeechText;
    float speechTimer;
    public float maxSpeechTimer;
    bool showSpeech = false;

    [HeaderAttribute("Audio")]
    public AudioSource bgm;
    public AudioSource sfx;
    public AudioClip invasionBgm;
    public AudioClip clickSfx;
    public AudioClip woodChopSfx;
    public AudioClip miningSfx;
    public AudioClip buildingSfx;
    public AudioSource sheepSfx;

    bool promptShown = false;

    void Awake()
    {
        instance = this;
        selectedBuildingToBuild = null;
    }
	// Use this for initialization
	void Start () {
        houseList = new List<SlotScript>();
        barracksList = new List<SlotScript>();
        townHallList = new List<SlotScript>();
        bgm.clip = bgmClips[0];
        gameOverScreen.SetActive(false);
        speechTimer = maxSpeechTimer;
        ChangeSpeechText("Welcome to my Lamb-pire! Build me a monument NOW");
        soldierPrefab.GetComponent<SpriteRenderer>().sprite = soldierSprites[0];
        citizenPrefab.GetComponent<SpriteRenderer>().sprite = citizenSprites[0];
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
            newCitizen.isActive = true;
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
        //cheat codes for editor
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameOver();
        }
        //for testing soldiers and enemies
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space");
            StartCoroutine(SpawnEnemies(1));
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PlatformMapScript.Point spawnpoint = new PlatformMapScript.Point(6, 0);
            Vector3 spawnpointvec = new Vector3(spawnpoint.PointToCoord().x, (spawnpoint.PointToCoord().y));
            AddSoldier(spawnpointvec);
        }
        //cheat for adding resources
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _lumberCount+=10;
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            _oreCount+=10;
        }
        if (Input.GetKey(KeyCode.Home))
        {
            _influenceCount += 1;
        }

        //for testing next era
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            NextEra();
        }
#endif
        minutes.text = minutesValue.ToString();
        seconds.text = ((int)secondsValue).ToString();
        secondsValue -= Time.deltaTime;
        if((int)secondsValue < 10)
        {
            seconds.text = "0" + ((int)secondsValue).ToString();
        }
        if(secondsValue <= 0)
        {
            secondsValue = 59;
            minutesValue--;

        }
        if(minutesValue < 0)
        {
            GameOver();
        }
        if(_citizenCount <= 0)
        {
            GameOver();
        }

        lumberText.text = _lumberCount.ToString();
        citizenText.text = _citizenCount.ToString();
        oreText.text = _oreCount.ToString();
        int influenceOnes = (((_influenceCount % 1000) %100)%10);
        int influenceTens = (int)((_influenceCount % 1000) % 100) / 10;
        int influenceHuns = (int)(_influenceCount % 1000) / 100;
        int influenceThous = (int)(_influenceCount / 1000);
        influenceTextOnes.text = influenceOnes.ToString();
        influenceTextTens.text = influenceTens.ToString();
        influenceTextHuns.text = influenceHuns.ToString();
        influenceTextThous.text = influenceThous.ToString();
        soldierText.text = _currSoldierCount.ToString();

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
            pausePanel.SetActive(false);
        }

        if (showSpeech)
        {
            kingSpeechObj.SetActive(true);
            speechTimer -= Time.deltaTime;
            if(speechTimer <= 0)
            {
                showSpeech = false;
                speechTimer = maxSpeechTimer;
            }
        }
        else
        {
            kingSpeechObj.SetActive(false);
        }
        
        if(minutesValue <= 1)
        {
            ChangeSpeechText("You're running out of time. WHERE'S MY MONUMENT?!");
        }

        
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        gameOverScreen.SetActive(true);
        isGameOver = true;
    }

    public void ChangeSpeechText(string content)
    {
        showSpeech = true;
        kingSpeechText.text = content;
        sheepSfx.Play();

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

    public void RestartButton()
    {
        SceneManager.LoadScene("Platform");
        isGameOver = false;
    }


    public void TreeHarvested()
    {
        _lumberCount += lumberHarvestReward;
        //update lumber text
    }

    public void RockHarvested()
    {
        _oreCount += oreHarvestReward;
        //update lumber text
    }

    public void BuildingDemolishedAddReward()
    {
        _lumberCount += lumberDemolishReward;
        _oreCount += oreDemolishReward;
    }

    public void AddCitizen(Vector3 pos)
    {

        ////check pool, if got inactive set active, if not instantiate

        //_citizenCount += CITIZEN_PER_HOUSE;
        //if (citizenPool.Count < _citizenCount)
        //{
        //    for (int i = 0; i < CITIZEN_PER_HOUSE; i++)
        //    {
        //        GameObject citizenObj = Instantiate(citizenPrefab);
        //        //citizenObj.transform.SetParent(GameObject.Find("Map").transform);
        //        //citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
        //        citizenObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1); //also temp
        //        Citizen newCitizen = citizenObj.GetComponent<Citizen>();
        //        citizenPool.Add(newCitizen);
        //        newCitizen.isBusy = false;

        //    }
        //}
        //else
        //{
        //    //for each new citizen
        //    //if there is an inactive citizen in the pool
        //    //set it to active, and reset its position
        //    for (int j = 0; j < CITIZEN_PER_HOUSE; j++)
        //    {
        //        for (int i = 0; i < citizenPool.Count; i++)
        //        {
        //            if (!citizenPool[i].gameObject.activeSelf)
        //            {
        //                citizenPool[i].gameObject.SetActive(true);
        //                citizenPool[i].transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1); //also temp
        //                citizenPool[i].ResetPointPosition();
        //                citizenPool[i].isBusy = false;
        //            }
        //        }
        //    }
        //}
        int citizensLeftToSpawn = CITIZEN_PER_HOUSE;
        //PlatformMapScript.Point spawnPoint = new PlatformMapScript.Point(0, 0);
        //check pool if got enough, if not instantiate
        for (int i = 0; i < CITIZEN_PER_HOUSE; i++)
        {
            //find non-active
            for (int j = 0; j < citizenPool.Count; j++)
            {
                if (!citizenPool[j].isActive && citizensLeftToSpawn >0)
                {
                    citizenPool[j].isActive = true;
                    citizenPool[j].transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1);
                    //reset point
                    citizenPool[j].ResetPointPosition();
                    citizenPool[j].isBusy = false;
                    //yield return new WaitForSeconds(1);
                    //_soldierCount++;
                    //_currSoldierCount++;
                    citizensLeftToSpawn--;
                    _citizenCount++;
                }
            }

        }

        if (citizensLeftToSpawn > 0)
        {
            //if not enough soldier in pool, spawn more
            for (int i = 0; i < citizensLeftToSpawn; i++)
            {
                GameObject citizenObj = Instantiate(citizenPrefab);
                citizenObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1);
                Citizen newCitizen = citizenObj.GetComponent<Citizen>();
                citizenPool.Add(newCitizen);
                newCitizen.isActive = true;
                newCitizen.isBusy = false;
                //_soldierCount++;
                //_currSoldierCount++;
                _citizenCount++;
                //yield return new WaitForSeconds(1);
            }
        }



    }

    public void AddSoldier(Vector3 pos)
    {

        int soldiersLeftToSpawn = SOLDIERS_PER_BARRACKS;
        //PlatformMapScript.Point spawnPoint = new PlatformMapScript.Point(0, 0);
        //check pool if got enough, if not instantiate
        for (int i = 0; i < SOLDIERS_PER_BARRACKS; i++)
        {
            //find non-active
            for (int j = 0; j < soldierPool.Count; j++)
            {
                if (!soldierPool[j].isActive && soldiersLeftToSpawn >0)
                {
                    soldierPool[j].isActive = true;
                    soldierPool[j].transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1);
                    soldierPool[j].ResetPointPosition();
                    //yield return new WaitForSeconds(1);
                    //_soldierCount++;
                    soldiersLeftToSpawn--;
                    _currSoldierCount++;
                }
            }

        }

        if (soldiersLeftToSpawn > 0)
        {
            //if not enough soldier in pool, spawn more
            for (int i = 0; i < soldiersLeftToSpawn; i++)
            {
                GameObject soldierObj = Instantiate(soldierPrefab);
                soldierObj.transform.localPosition = new Vector3(pos.x, pos.y + 0.6f, pos.z - 1);
                Soldier newSoldier = soldierObj.GetComponent<Soldier>();
                soldierPool.Add(newSoldier);
                newSoldier.isActive = true;
                _soldierCount++;
                _currSoldierCount++;
                //yield return new WaitForSeconds(1);
            }
        }

        //if soldier dies, remember to change isActive back to false
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
            if (!soldierPool[i].isBusy && soldierPool[i].isActive)
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
                    if (!soldierPool[i].isBusy && soldierPool[i].isActive)
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
        for(int i=0; i<citizenPool.Count; i++)
        {
            if (!citizenPool[i].isBusy && citizenPool[i].isActive)
            {   
                //y check
                if(citizenPool[i].pointY == slotPoint.y)
                {
                    if (Mathf.Abs(citizenPool[i].pointX - slotPoint.x) < closestX )
                    {
                        citizenFound = true;
                        closestX = Mathf.Abs(citizenPool[i].pointX - slotPoint.x);
                        foundCitizen = citizenPool[i];
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
                    if (!citizenPool[i].isBusy && citizenPool[i].isActive)
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
            case (4):
                selectedBuildingToBuild = Townhall;
                break;
            case (5):
                selectedBuildingToBuild = School;
                break;
        }
    }

    public Buildings ChooseBuildingFromIndex(int buildingIndex)
    {
        Buildings resultBuilding = null;
        switch (buildingIndex)
        {
            case (0):
                resultBuilding = House;
                break;
            case (1):
                resultBuilding = Barracks;
                break;
            case (2):
                resultBuilding = Ladder;
                break;
            case (3):
                resultBuilding = Pyramid;
                break;
            case (4):
                resultBuilding = Townhall;
                break;
            case (5):
                resultBuilding = School;
                break;
        }
        return resultBuilding;
    }

    public void BuildSelected(GameObject slotToBuildIn, int selectedBuilding)
    {
        //TODO: Resource check should be done before citizen goes to slot
        if (selectedBuilding == 0)
        {
            //check resources
            if(ResourceCheck(House))
            {

                //build house
                GameObject newBuilding = Instantiate(House.prefab);
                newBuilding.GetComponent<SpriteRenderer>().sprite = House.buildingSprite;
                slotToBuildIn.GetComponent<SlotScript>().currBuilding = SlotScript.Building.House;
                slotToBuildIn.GetComponent<SlotScript>().eraBuilt = eraIndex;
                slotToBuildIn.GetComponent<SlotScript>().buildingObj = newBuilding;
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1f, 0);
                //add citizen at spot
                AddCitizen(slotToBuildIn.transform.position);
                SpendResources(House);
                houseList.Add(slotToBuildIn.GetComponent<SlotScript>());
            }
            
        }
        else if (selectedBuilding == 1)
        {
            //barrack
            if (ResourceCheck(Barracks))
            {

                //build barracks
                GameObject newBuilding = Instantiate(Barracks.prefab);
                newBuilding.GetComponent<SpriteRenderer>().sprite = Barracks.buildingSprite;
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                slotToBuildIn.GetComponent<SlotScript>().currBuilding = SlotScript.Building.Barracks;
                slotToBuildIn.GetComponent<SlotScript>().eraBuilt = eraIndex;
                slotToBuildIn.GetComponent<SlotScript>().buildingObj = newBuilding;
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1f, 0);
                //add soldiers at spot
                AddSoldier(slotToBuildIn.transform.position);
                barracksList.Add(slotToBuildIn.GetComponent<SlotScript>());
                SpendResources(Barracks);
            }
        }
        else if (selectedBuilding == 2)
        {
            //ladder
            if (ResourceCheck(Ladder))
            {
                GameObject newBuilding = Instantiate(Ladder.prefab);
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.4f, 1, 1);
                newBuilding.transform.localPosition = new Vector3(0, 1.3f, 0);
                //save position of ladder for later reference
                SlotScript slotToBuildInScript = slotToBuildIn.GetComponent<SlotScript>();
                slotToBuildIn.GetComponent<SlotScript>().buildingObj = newBuilding;
                slotToBuildInScript.currBuilding = SlotScript.Building.Ladder;
                PlatformMapScript.Point slotPoint = slotToBuildInScript.point;
                ladderSlots.Add(slotToBuildInScript);
                SpendResources(Ladder);
                ////if slot y is not 2, add the slot on top as well
                //if(slotToBuildIn.GetComponent<SlotScript>().point.y < 2.0f)
                //{
                //    ladderSlots.Add(PlatformMapScript.instance.slotArray[(int)slotPoint.y+1, (int)slotPoint.x].GetComponent<SlotScript>());
                //}
            }

        }else if(selectedBuilding == 3)
        {
            //monument
            if (ResourceCheck(Pyramid))
            {
                GameObject newBuilding = Instantiate(Pyramid.prefab);
                newBuilding.GetComponent<SpriteRenderer>().sprite = Pyramid.buildingSprite;
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1.2f, 0);
                slotToBuildIn.GetComponent<SlotScript>().buildingObj = newBuilding;
                SpendResources(Pyramid);
                NextEra();
            }
        }
        else if (selectedBuilding == 4)
        {
            //townhall
            if (ResourceCheck(Townhall))
            {
                GameObject newBuilding = Instantiate(Townhall.prefab);
                newBuilding.GetComponent<SpriteRenderer>().sprite = Townhall.buildingSprite;
                newBuilding.transform.SetParent(slotToBuildIn.transform);
                slotToBuildIn.GetComponent<SlotScript>().eraBuilt = eraIndex;
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0, 1.2f, 0);
                slotToBuildIn.GetComponent<SlotScript>().buildingObj = newBuilding;
                SpendResources(Townhall);
                townHallList.Add(slotToBuildIn.GetComponent<SlotScript>());
                //do townhall skills
            }
        }
        //check if resource req for selected building is met
        //builds building
        //tells GameManager to remove req resources from count
        //reset building selected
        //selectedBuildingIndexToBuild = -1;
        //selectedBuildingToBuild = null;

        //do a check on influence to see if enemy soldiers should come
        EnemyWaveCheck();
        EnemyComingPrompt();
    }

    public void DemolishBuilding(SlotScript slot,bool isRewarded)
    {
        if(slot.currBuilding == SlotScript.Building.House)
        {
            houseList.Remove(slot);
            //minus influence
            _influenceCount -= House.influenceCost;
            if (eraIndex > 0)
            {
                _influenceCount -= (slot.eraBuilt+1) * House.influenceCost / (eraIndex * 2);
            }
            
        }else if(slot.currBuilding == SlotScript.Building.Barracks)
        {
            barracksList.Remove(slot);
            //minus influence
            _influenceCount -= Barracks.influenceCost;
            if (eraIndex > 0)
            {
                _influenceCount -= (slot.eraBuilt + 1) * Barracks.influenceCost / (eraIndex * 2);
            }
        }
        else if(slot.currBuilding == SlotScript.Building.Townhall)
        {
            townHallList.Remove(slot);
            //minus influence
            _influenceCount -= Townhall.influenceCost;
            if (eraIndex > 0)
            {
                _influenceCount -= (slot.eraBuilt + 1) * Townhall.influenceCost / (eraIndex * 2);
            }
        }
        Destroy(slot.buildingObj);
        if (isRewarded)
        {
            BuildingDemolishedAddReward();
        }
        
        demolishMode = false;
        slot.currBuilding = SlotScript.Building.None;
    }

    public bool ResourceCheck(Buildings buildingType)
    {
        bool result = false;

        if(_lumberCount >= -buildingType.lumberCost &&
            _citizenCount >= -buildingType.labourCost &&
            _oreCount >= -buildingType.oreCost &&
            _influenceCount >= -buildingType.influenceCost)
        {
            result = true;
        }

        Debug.Log("Resource check satisfied: " + result);
        return result;
    }

    public void SpendResources(Buildings buildingType)
    {
        _lumberCount += buildingType.lumberCost;
        _oreCount += buildingType.oreCost;
        _influenceCount += buildingType.influenceCost;
        //_influenceCount += buildingType.influenceReward;
        
    }
    
    public void NextEra()
    {
        if (eraIndex <= 2)
        {
            eraIndex++;
            maxMinutes++;
            //change sprites
            backgroundObj.GetComponent<SpriteRenderer>().sprite = eraBackgrounds[eraIndex];
            overlayObj.GetComponent<Image>().sprite = eraOverlays[eraIndex];
            kingObj.GetComponent<Image>().sprite = kingSprites[eraIndex];
            soldierPrefab.GetComponent<SpriteRenderer>().sprite = soldierSprites[eraIndex];
            citizenPrefab.GetComponent<SpriteRenderer>().sprite = citizenSprites[eraIndex];
            House.buildingSprite = houseSprites[eraIndex];
            Barracks.buildingSprite = barrackSprites[eraIndex];
            Pyramid.buildingSprite = monumentSprites[eraIndex];
            Pyramid.influenceCost = monumentInfo[eraIndex].influenceCost;
            Pyramid.labourCost = monumentInfo[eraIndex].labourCost;
            Pyramid.lumberCost = monumentInfo[eraIndex].lumberCost;
            Pyramid.oreCost = monumentInfo[eraIndex].oreCost;
            Pyramid.timeToBuild = monumentInfo[eraIndex].timeToBuild;
            bgm.clip = bgmClips[eraIndex];
            bgm.Play();
            
            if (eraIndex >= 1)
            {
                //if 2nd era

                ChangeSpeechText("Welcome to the new era! We've discovered how to make townhalls!");
                townhallButtonObj.GetComponent<Button>().interactable = true;
                townhallButtonObj.GetComponent<Image>().sprite = activeButtonSprite;
            }
            if(eraIndex == 2)
            {

                ChangeSpeechText("Welcome to the new era! We've discovered how to make schools!");
                schoolButtonObj.GetComponent<Button>().interactable = true;
                schoolButtonObj.GetComponent<Image>().sprite = activeButtonSprite;
            }
            //monumentButtonObj.GetComponent<SpriteRenderer>().sprite = monumentIcon[eraIndex];
            for(int i=0;i<citizenPool.Count; i++)
            {
                citizenPool[i].GetComponent<SpriteRenderer>().sprite = citizenSprites[eraIndex];
            }
            for (int i = 0; i < soldierPool.Count; i++)
            {
                soldierPool[i].GetComponent<SpriteRenderer>().sprite = soldierSprites[eraIndex];
            }
            firstEnemyWaveSpawned = false;
            secondEnemyWaveSpawned = false;
            thirdEnemyWaveSpawned = false;
            fourthEnemyWaveSpawned = false;
            //reset resources value
            ResetResources();
            //find all the slots with resource on it, call resource regen on the slots
            for(int i=0; i < PlatformMapScript.instance.slotList.Count; i++)
            {
                if(PlatformMapScript.instance.slotList[i].GetComponent<SlotScript>().currBuilding == SlotScript.Building.Tree ||
                    PlatformMapScript.instance.slotList[i].GetComponent<SlotScript>().currBuilding == SlotScript.Building.Rock)
                {
                    PlatformMapScript.instance.slotList[i].GetComponent<SlotScript>().ResourceRegen();
                }
            }

        }else if(eraIndex >= 3)
        {
            //Grats! You beat the game!
        }
        
    }

    void ResetResources()
    {
        //all these values are PlaceHolder
        _lumberCount = 0;
        _oreCount = 0;
        era1houseCount = 0;
        era2houseCount = 0;
        era1barracksCount = 0;
        era2barracksCount = 0;
        era1townHallCount = 0;
        era2townHallCount = 0;
        for (int i= 0; i < houseList.Count; i++){
            if(houseList[i].eraBuilt == 0)
            {
                era1houseCount++;
            }else if(houseList[i].eraBuilt == 1)
            {
                era2houseCount++;
            }
        }
        for (int i = 0; i < barracksList.Count; i++)
        {
            if (barracksList[i].eraBuilt == 0)
            {
                era1barracksCount++;
            }
            else if (barracksList[i].eraBuilt == 1)
            {
                era2barracksCount++;
            }
        }
        for (int i = 0; i < townHallList.Count; i++)
        {
            if (townHallList[i].eraBuilt == 0)
            {
                era1townHallCount++;
            }
            else if (townHallList[i].eraBuilt == 1)
            {
                era2townHallCount++;
            }
        }
        Debug.Log("EraIndex" + era1houseCount);
        Debug.Log("eracalculation"+ era1houseCount * (House.influenceCost / (eraIndex * 2)));
        _influenceCount = (era1houseCount * (House.influenceCost / (eraIndex*2))) + (era1barracksCount*oldBarracksInfluence/(eraIndex * 2)) + (era1townHallCount*oldTownhallInfluence / (eraIndex * 2))
            + (era2houseCount * (House.influenceCost / (eraIndex))) + (era1barracksCount * oldBarracksInfluence / (eraIndex)) + (era1townHallCount * oldTownhallInfluence / (eraIndex));
        InitializeCitizens(2);
        InitializeSoldiers();
        //reset timer?
        minutesValue = maxMinutes;
        secondsValue = 0;

    }
    
    public void InitializeSoldiers()
    {
        //set soldiers to inactive
        for (int i = 0; i < soldierPool.Count; i++)
        {
            soldierPool[i].isActive = false;
        }
        _currSoldierCount = 0;
    }
    public void EnemyComingPrompt()
    {
        if(eraIndex==0 && _influenceCount >= 10 && !promptShown)
        {
            ChangeSpeechText("How about building a barracks? We may need some defenders against the big bad wolves");
            promptShown = true;
        }
        if(_influenceCount >= firstCheckpoint[eraIndex]-20 && !promptShown)
        {
            ChangeSpeechText("I smell the big bad wolves coming!");
            promptShown = true;
        }
        if (_influenceCount >= secondCheckpoint[eraIndex]-20 && !promptShown)
        {
            ChangeSpeechText("I smell the big bad wolves coming!");
            promptShown = true;
        }
        if (_influenceCount >= thirdCheckpoint[eraIndex]-20 && !promptShown && eraIndex>=1)
        {
            ChangeSpeechText("I smell the big bad wolves coming!");
            promptShown = true;
        }
        if (_influenceCount >= fourthCheckpoint[eraIndex]-20 && !promptShown && eraIndex >=2)
        {
            ChangeSpeechText("I smell the big bad wolves coming!");
            promptShown = true;
        }
    }

    public void EnemyWaveCheck()
    {
        Debug.Log("INVASION CHECK");
        //check era
        if (eraIndex == 0)
        {
            //Egyptian
            if (_influenceCount >= firstCheckpoint[0] && !firstEnemyWaveSpawned)
            {
                //spawn 2 enemies
                Debug.Log("INVASION");
                StartCoroutine(SpawnEnemies(enemyWaveSize[0]));
                firstEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                //reset the warning prompt
                promptShown = false;

            }
            else if (_influenceCount >= secondCheckpoint[0] && !secondEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[0]));
                secondEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }
            
        }
        if (eraIndex == 1)
        {
            //Roman
            if (_influenceCount >= firstCheckpoint[1] && !firstEnemyWaveSpawned)
            {
                //spawn 2 enemies
                Debug.Log("INVASION");
                StartCoroutine(SpawnEnemies(enemyWaveSize[1]));
                firstEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;

            }
            else if (_influenceCount >= secondCheckpoint[1] && !secondEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[1]));
                secondEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }
            else if (_influenceCount >= thirdCheckpoint[1] && !thirdEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[1]));
                thirdEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }

        }
        if (eraIndex == 2)
        {
            //Roman
            if (_influenceCount >= firstCheckpoint[2] && !firstEnemyWaveSpawned)
            {
                //spawn 2 enemies
                Debug.Log("INVASION");
                StartCoroutine(SpawnEnemies(enemyWaveSize[2]));
                firstEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;

            }
            else if (_influenceCount >= secondCheckpoint[2] && !secondEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[2]));
                secondEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }
            else if (_influenceCount >= thirdCheckpoint[2] && !thirdEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[2]));
                thirdEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }
            else if (_influenceCount >= fourthCheckpoint[2] && !fourthEnemyWaveSpawned)
            {
                //spawn 2 enemies
                StartCoroutine(SpawnEnemies(enemyWaveSize[2]));
                fourthEnemyWaveSpawned = true;
                PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.invasionBgm;
                PlatformGameManager.instance.bgm.Play();
                promptShown = false;
            }

        }

    }

    public IEnumerator SpawnEnemies(int enemyCount)
    {
        int enemyLeftToSpawn = enemyCount;
        currEnemyCount += enemyCount;
        PlatformMapScript.Point spawnPoint = new PlatformMapScript.Point(0, 0);
        //check pool if got enough, if not instantiate
        for(int i=0; i < enemyCount; i++)
        {
            //find non-active
            for(int j = 0; j < enemyPool.Count; j++)
            {
                if (!enemyPool[j].isActive && enemyLeftToSpawn > 0)
                {
                    enemyPool[j].isActive = true;
                    enemyPool[j].transform.localPosition = new Vector3(spawnPoint.PointToCoord().x, spawnPoint.PointToCoord().y + 0.6f,-1);
                    enemyLeftToSpawn--;
                    yield return new WaitForSeconds(1);
                }
            }
            
        }

        if (enemyLeftToSpawn > 0)
        {
            //if not enough enemy in pool, spawn more
            for(int i = 0; i < enemyLeftToSpawn; i++)
            {
                GameObject enemyObj = Instantiate(enemyPrefab);
                enemyObj.transform.localPosition = new Vector3(spawnPoint.PointToCoord().x, spawnPoint.PointToCoord().y + 0.6f, -1); //also temp, offscreen
                Enemy newEnemy = enemyObj.GetComponent<Enemy>();
                enemyPool.Add(newEnemy);
                newEnemy.isActive = true;
                yield return new WaitForSeconds(1);
            }
        }

        //if enemy dies, remember to change isActive back to false
    }

    void InitializeCitizens(int citizenCount)
    {
        //go through the pool, set all inactive
        //set new amount of citizens as active
        //reset position to bottom row
        Debug.Log(citizenPool.Count);
        for(int i=0; i < citizenPool.Count; i++)
        {
            citizenPool[i].isActive = false;
            citizenPool[i].isAttacked = false;
        }
        for (int i = 0; i < citizenCount; i++)
        {
            citizenPool[i].isActive = true;
            citizenPool[i].transform.localPosition = new Vector3(0, -1.9f, -1); //also temp
            citizenPool[i].ResetPointPosition();
            citizenPool[i].isBusy = false;
            Debug.Log("reset citizens " + i);
            
        }
        _citizenCount = citizenCount;

    }

    public void DemolishButton()
    {
        demolishMode = !demolishMode;
        Debug.Log("demolishMode: " + demolishMode);
    }

    public void KillCitizen(GameObject citizen)
    {
        citizen.GetComponent<Citizen>().isActive = false;
        citizen.GetComponent<Citizen>().isBusy = false;
        _citizenCount--;
        Debug.Log("KillCitizen");
    }

    public void KillEnemy(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().isActive = false;
        currEnemyCount--;
        if(currEnemyCount <= 0)
        {
            PlatformGameManager.instance.bgm.clip = PlatformGameManager.instance.bgmClips[eraIndex];
            PlatformGameManager.instance.bgm.Play();
        }
    }

    public void KillSoldier(Soldier soldier)
    {
        soldier.isActive = false;
        soldier.currHealth = soldier.maxHealth;
        _currSoldierCount--;
        Debug.Log("Killsoldier");
    }

    public bool LadderCheck(SlotScript slot)
    {
        for(int i = 0; i < ladderSlots.Count; i++)
        {
            if(ladderSlots[i].point.y == slot.point.y)
            {
                return false;
            }
        }
        return true;
    }


}
