using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour
{
    public SpriteRenderer highlight;

    public enum Building
    {
        None,
        Tree,
        House,
        Ladder,
        Rock
    };

    public Building currBuilding;
    public GameObject buildingObj = null;
    
    public PlatformMapScript.Point point;
    public PlatformMapScript.Coord coord;
    public int treeHealth = 10;
    public int rockHealth = 10;

	// Use this for initialization
	void Start () {
        
        highlight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //find size of childindex?
        if(transform.childCount > 1)
        {
            buildingObj = transform.GetChild(1).gameObject;
        }
        
        if(buildingObj == null)
        {
            currBuilding = Building.None;
        }else if (buildingObj.name.Contains("Tree"))
        {
            currBuilding = Building.Tree;
        }else if (buildingObj.name.Contains("House"))
        {
            currBuilding = Building.House;
        }else if (buildingObj.name.Contains("Ladder"))
        {
            currBuilding = Building.Ladder;
        }else if (buildingObj.name.Contains("Rock"))
        {
            currBuilding = Building.Rock;
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseDown()
    {
        
        if (currBuilding == Building.None)
        {
            //check what is the building selected to be built
            PlatformGameManager.instance.BuildSelected(this.gameObject);

        }else
        {
            Citizen freeCitizen = PlatformGameManager.instance.GetCitizen();
            if (freeCitizen != null)
            {
                StartCoroutine(freeCitizen.GoToSlot(this.gameObject));
                freeCitizen.goalSlotObj = this.gameObject;
            }
            else
            {
                return; //all citizens busy, maybe give a message
            }
        }
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnMouseEnter()
    {
        Color translucent = highlight.color;
        translucent.a = 1.0f;
        highlight.color = translucent;
    }

    public void OnMouseExit()
    {
        Color transparent = highlight.color;
        transparent.a = 0;
        highlight.color = transparent;
    }
}
