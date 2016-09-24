﻿using UnityEngine;
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
        Ladder
    };

    public Building currBuilding;
    public GameObject buildingObj;
    
    public PlatformMapScript.Point point;
    public PlatformMapScript.Coord coord;

	// Use this for initialization
	void Start () {
        
        highlight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        buildingObj = transform.GetChild(1).gameObject;
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
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseDown()
    {
        
        if(currBuilding == Building.Tree)
        {
            //citizens gather tree
            //get citizen
            Debug.Log("CLICKED");
            Citizen freeCitizen = PlatformGameManager.instance.GetCitizen();
            if(freeCitizen != null)
            {
                freeCitizen.GoToSlot(this.gameObject);
            }
            else
            {
                return; //all citizens busy, maybe give a message
            }
            //tell gamemanager to add to lumber count
            //PlatformGameManager.instance.TreeHarvested();
            //destroy the tree
            //Destroy(buildingObj);
            //set building to none
            //_currBuilding = Building.None;
        }

        if(currBuilding == Building.None)
        {
            Debug.Log("CLICKED");
            //check what is the building selected to be built
            if (PlatformGameManager.instance.selectedBuildingToBuild == 0)
            {
                //build house
                GameObject newBuilding = Instantiate(PlatformGameManager.instance.housePrefab);
                newBuilding.transform.SetParent(transform);
                newBuilding.transform.localScale = new Vector3(0.3f, 0.8f, 0);
                newBuilding.transform.localPosition = new Vector3(0,0.8f,0);
                //add citizen at spot
                PlatformGameManager.instance.AddCitizen(transform.position);
            }else if(PlatformGameManager.instance.selectedBuildingToBuild == 1)
            {
                //barrack
            }else if(PlatformGameManager.instance.selectedBuildingToBuild == 2)
            {
                //ladder
                GameObject newBuilding = Instantiate(PlatformGameManager.instance.ladderPrefab);
                newBuilding.transform.SetParent(transform);
                //newBuilding.transform.localScale = transform.localScale;
                newBuilding.transform.localPosition = new Vector3(0, 1.6f, 0);
            }
            //check if resource req for selected building is met
            //builds building
            //tells GameManager to remove req resources from count
            //reset building selected
            PlatformGameManager.instance.selectedBuildingToBuild = -1;
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
