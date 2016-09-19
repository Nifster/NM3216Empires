using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image highlight;

    public enum Building
    {
        None,
        Tree,
        House
    };

    public Building currBuilding;
    public GameObject buildingObj;

	// Use this for initialization
	void Start () {

        highlight = transform.GetChild(0).GetComponent<Image>();
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
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        
        if(currBuilding == Building.Tree)
        {
            //citizens gather tree
            //get citizen
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
                newBuilding.transform.localScale = transform.localScale;
                newBuilding.transform.localPosition = new Vector3(0,40,0);
            }
            //check if resource req for selected building is met
            //builds building
            //tells GameManager to remove req resources from count
        }
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        Color translucent = highlight.color;
        translucent.a = 0.5f;
        highlight.color = translucent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color transparent = highlight.color;
        transparent.a = 0;
        highlight.color = transparent;
    }
}
