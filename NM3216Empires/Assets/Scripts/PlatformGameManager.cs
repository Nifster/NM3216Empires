using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlatformGameManager : MonoBehaviour {

    public int citizenCount;
    public static PlatformGameManager instance;

    private int _lumberCount;

    public Text lumberText;
	// Use this for initialization
	void Start () {
        instance = this;

        //need to initialise the map, with coords
	}
	
	// Update is called once per frame
	void Update () {
        lumberText.text = _lumberCount.ToString();
	}

    public void HouseBuilt()
    {
        //if citizenCount > 2?
        citizenCount++;
    }

    public void TreeHarvested()
    {
        _lumberCount++;
        //update lumber text
    }
}
