using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour
{
    public SpriteRenderer highlight;
    public GameObject genericBuildingPreview;

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
    public int resourceHealth = 10;
    bool hasHealth = false;
    bool hasTimer = false;
    bool resourceTicking = false;
    //public int rockHealth = 10;
    public GameObject healthSlider;
    public GameObject resourceTimerSliderPrefab;
    GameObject resourceTimerSlider;
    GameObject canvasObj;

	// Use this for initialization
	void Start () {
        genericBuildingPreview = GameObject.FindGameObjectWithTag("GenericBuildingPreview");
        genericBuildingPreview.GetComponent<SpriteRenderer>().enabled = false;
        canvasObj = GameObject.Find("Canvas");
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
        if (!PlatformGameManager.instance.isPaused)
        {
            Citizen freeCitizen = PlatformGameManager.instance.GetCitizen(point);
            if (currBuilding == Building.None)
            {
                if (PlatformGameManager.instance.selectedBuildingIndexToBuild != -1)
                {
                    StartCoroutine(freeCitizen.GoToSlot(this.gameObject));
                    freeCitizen.goalSlotObj = this.gameObject;
                    //check what is the building selected to be built
                }

            }
            else
            {

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
        
    }

    public void UpdateResourceValue()
    {
        if(!hasHealth)
        {
            healthSlider = Instantiate(healthSlider);
            healthSlider.transform.SetParent(canvasObj.transform);
            healthSlider.transform.position = transform.position;
            healthSlider.transform.localScale = new Vector3(1, 1, 1);
            hasHealth = true;
            healthSlider.name += " " + name; 
            
        }
        Slider sliderInfo = healthSlider.GetComponent<Slider>();
        sliderInfo.value = resourceHealth;
    }

    public void UpdateResourceTimerValue(PlatformGameManager.Buildings buildingType)
    {
        
        if (!hasTimer)
        {
            resourceTimerSlider = Instantiate(resourceTimerSliderPrefab);
            resourceTimerSlider.transform.SetParent(canvasObj.transform);
            resourceTimerSlider.transform.position = new Vector2 (transform.position.x, transform.position.y +1.5f);
            resourceTimerSlider.transform.localScale = new Vector3(1, 1, 1);
            hasTimer = true;
            resourceTimerSlider.name += " " + name;

        }
        Slider sliderInfo = resourceTimerSlider.GetComponent<Slider>();
        sliderInfo.maxValue = (buildingType.timeToBuild + 1) * 100; //not sure why need +1, possibly 0 index problem
        sliderInfo.value = (buildingType.timeToBuild + 1) * 100;
        StartCoroutine(ResourceTimerTickDown(sliderInfo));
    }

    IEnumerator ResourceTimerTickDown(Slider resourceTimer)
    {
        if (!resourceTicking)
        {
            resourceTicking = true;
            while(resourceTimer.value > 0)
            {
                resourceTimer.value -= 100*Time.deltaTime; //need to *100 because slider value cant handle float values well
                yield return 0;
            }
        }
        resourceTicking = false;
        Destroy(resourceTimer.gameObject);
        hasTimer = false;
    }

    public void DestroyHealth()
    {
        Destroy(healthSlider);
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnMouseOver()
    {
        if (!PlatformGameManager.instance.isPaused)
        {
            Color translucent = highlight.color;
            translucent.a = 1.0f;
            highlight.color = translucent;

            if (PlatformGameManager.instance.selectedBuildingToBuild != null)
            {
                Vector3 hoverPosition;
                //if there is a building selected to build, make a preview of building on slot
                genericBuildingPreview.GetComponent<SpriteRenderer>().enabled = true;
                genericBuildingPreview.GetComponent<SpriteRenderer>().sprite = PlatformGameManager.instance.selectedBuildingToBuild.buildingSprite;
                //hardcode offset T_T

                genericBuildingPreview.transform.localPosition = new Vector3(transform.position.x, transform.position.y + 0.8f);
            }

            if (Input.GetMouseButtonDown(1)) //do a check if you have a barracks
            {
                //TODO: get free soldier, go to slot
                Soldier freeSoldier = PlatformGameManager.instance.GetSoldier(point);
                //do a check if freeSoldier null
                StartCoroutine(freeSoldier.GoToSlot(this.gameObject));
                freeSoldier.goalSlotObj = this.gameObject;
            }
        }
        
    }

    public void OnMouseExit()
    {
        genericBuildingPreview.GetComponent<SpriteRenderer>().enabled = false;
        Color transparent = highlight.color;
        transparent.a = 0;
        highlight.color = transparent;
    }
}
