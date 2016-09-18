using UnityEngine;
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
    
    public enum BuildingToBuild
    {
        House,
        Barrack
    }
    [HeaderAttribute("")]
    public int selectedBuildingToBuild = -1; //-1 by default, meaning unselected
    //0 for house, 1 for school etc

    private int _lumberCount;

    public Text lumberText;
    public Text citizenText;

    public List<Citizen> citizenPool;

    [SerializeField]
    private GameObject citizenPrefab;
	// Use this for initialization
	void Start () {
        instance = this;

        //need to initialise the map, with coords

        //initialize citizen pool
        for(int i=0; i<_citizenCount; i++)
        {
            GameObject citizenObj = Instantiate(citizenPrefab);
            citizenObj.transform.SetParent(GameObject.Find("Map").transform);
            citizenObj.transform.localScale = new Vector3(30f, 30f); //temp
            citizenObj.transform.localPosition = new Vector3(0, 30,-1); //also temp
            Citizen newCitizen = citizenObj.GetComponent<Citizen>();
            citizenPool.Add(newCitizen);
            newCitizen.isBusy = false;
        }
    }

    // Update is called once per frame
    void Update () {
        lumberText.text = _lumberCount.ToString();
        citizenText.text = _citizenCount.ToString();
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

    public void AddCitizen()
    {

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

}
