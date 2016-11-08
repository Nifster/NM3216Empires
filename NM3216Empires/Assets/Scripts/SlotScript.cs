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
        Rock,
        Barracks,
        Townhall,
        School
    };

    public Building currBuilding;
    public int eraBuilt;
    public GameObject buildingObj = null;
    public GameObject assignedIcon;
    
    public PlatformMapScript.Point point;
    public PlatformMapScript.Coord coord;
    public int resourceHealth = 10;
    public int maxHealth = 10;
    bool hasHealth = false;
    bool hasTimer = false;
    bool resourceTicking = false;
    //public int rockHealth = 10;
    public GameObject healthSliderPrefab;
    public GameObject healthSliderObj;
    public GameObject resourceTimerSliderPrefab;
    GameObject resourceTimerSlider;
    GameObject canvasObj;
    public bool hasWorkerAssigned = false;

	// Use this for initialization
	void Start () {
        assignedIcon.SetActive(false);
        genericBuildingPreview = GameObject.FindGameObjectWithTag("GenericBuildingPreview");
        genericBuildingPreview.GetComponent<SpriteRenderer>().enabled = false;
        canvasObj = GameObject.Find("Canvas");
        highlight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //find size of childindex?
        if(transform.childCount > 2)
        {
            buildingObj = transform.GetChild(2).gameObject;
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
        else if (buildingObj.name.Contains("Townhall"))
        {
            currBuilding = Building.Townhall;
        }
        else if (buildingObj.name.Contains("School"))
        {
            currBuilding = Building.School;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (PlatformGameManager.instance.selectedBuildingToBuild != null && Input.GetMouseButtonDown(1))
        {
            PlatformGameManager.instance.selectedBuildingToBuild = null;
            
        }
        if (PlatformGameManager.instance.demolishMode && Input.GetMouseButtonDown(1))
        {
            PlatformGameManager.instance.demolishMode = false;
        }
    }

    public void OnMouseDown()
    {
        if (!PlatformGameManager.instance.isPaused &&!PlatformGameManager.instance.isGameOver && !PlatformGameManager.instance.isCutscene)
        {
            Citizen freeCitizen = PlatformGameManager.instance.GetCitizen(point);
            if (currBuilding == Building.None)
            {
                if (PlatformGameManager.instance.selectedBuildingIndexToBuild != -1 && freeCitizen != null && !hasWorkerAssigned)
                {
                    //if ladder, do ladder check
                    if(PlatformGameManager.instance.selectedBuildingIndexToBuild == 2)
                    {
                        if (PlatformGameManager.instance.HasLadderCheck(this.point.y))
                        {
                            PlatformGameManager.instance.ChangeSpeechText("There's already a ladder on that level!");
                            return;
                        }
                    }
                    //do resource check
                    if (PlatformGameManager.instance.ResourceCheck(PlatformGameManager.instance.ChooseBuildingFromIndex(PlatformGameManager.instance.selectedBuildingIndexToBuild)))
                    {
                        hasWorkerAssigned = true;
                        StartCoroutine(freeCitizen.GoToSlot(this.gameObject, PlatformGameManager.instance.selectedBuildingIndexToBuild));
                        freeCitizen.goalSlotObj = this.gameObject;
                        
                    }
                    else
                    {
                        Debug.Log("Not enough resources");
                        PlatformGameManager.instance.ChangeSpeechText("You don't have enough resources for that building!");
                    }
                    //turn off preview after clicked
                    PlatformGameManager.instance.selectedBuildingToBuild = null;

                }else if (hasWorkerAssigned)
                {
                    PlatformGameManager.instance.ChangeSpeechText("A worker is already assigned to that!");
                    return;
                }

            }
            else
            {
                if(currBuilding == Building.Tree || currBuilding == Building.Rock || PlatformGameManager.instance.demolishMode)
                {
                    if (freeCitizen != null && !hasWorkerAssigned)
                    {
                        if (PlatformGameManager.instance.demolishMode && currBuilding == Building.Ladder)
                        {
                            PlatformGameManager.instance.ChangeSpeechText("Why would you want to cut off your access to resources?");
                            PlatformGameManager.instance.demolishMode = false;
                            return;
                        }
                        else
                        {
                            hasWorkerAssigned = true;
                            assignedIcon.SetActive(true);
                            StartCoroutine(freeCitizen.GoToSlot(this.gameObject, -1));
                            freeCitizen.goalSlotObj = this.gameObject;
                            
                            if (currBuilding == Building.Tree && resourceHealth <= 3)
                            {
                                PlatformGameManager.instance.ChangeSpeechText("You might run out of trees if you're not careful");
                            }
                            if (currBuilding == Building.Rock && resourceHealth <= 3)
                            {
                                PlatformGameManager.instance.ChangeSpeechText("You might run out of ores if you're not careful");
                            }
                        }
                        
                        
                        
                    }
                    else if (hasWorkerAssigned)
                    {
                        PlatformGameManager.instance.ChangeSpeechText("A worker is already assigned to that!");
                        return;
                    }

                }
                
            }

            PlatformGameManager.instance.clickButtonSfx.Play();
        }
        
    }

    public void UpdateResourceValue()
    {
        if(!hasHealth)
        {
            healthSliderObj = Instantiate(healthSliderPrefab);
            healthSliderObj.transform.SetParent(canvasObj.transform);
            healthSliderObj.transform.position = transform.position;
            healthSliderObj.transform.localScale = new Vector3(1, 1, 1);
            hasHealth = true;
            healthSliderObj.name += " " + name; 
            
        }
        Slider sliderInfo = healthSliderObj.GetComponent<Slider>();
        sliderInfo.value = resourceHealth;
        if (resourceHealth == maxHealth)
        {
            //healthSlider.SetActive(false);
            Destroy(healthSliderObj);
            hasHealth = false;
        }
    }

    public void UpdateResourceTimerValue(PlatformGameManager.Buildings buildingType)
    {
        
        if (!hasTimer)
        {
            resourceTimerSlider = Instantiate(resourceTimerSliderPrefab);
            resourceTimerSlider.transform.SetParent(canvasObj.transform);
            resourceTimerSlider.transform.position = new Vector3 (transform.position.x, transform.position.y +1.5f, 0);
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
                yield return null;
            }
            resourceTicking = false;
            Destroy(resourceTimer.gameObject);
            hasTimer = false;
        }
        else
        {
            Debug.Log("resource tick");
            yield return null;
        }
        
    }

    public void DestroyHealth()
    {
        Destroy(healthSliderObj);
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnMouseOver()
    {
        if (!PlatformGameManager.instance.isPaused && !PlatformGameManager.instance.isGameOver && !PlatformGameManager.instance.isCutscene)
        {
            Color translucent = highlight.color;
            translucent.a = 0.7f;
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

            
        }
        
    }

    public void OnMouseExit()
    {
        genericBuildingPreview.GetComponent<SpriteRenderer>().enabled = false;
        Color transparent = highlight.color;
        transparent.a = 0;
        highlight.color = transparent;
    }

    public void ResourceRegen()
    {
        resourceHealth = maxHealth;
        UpdateResourceValue();
        hasWorkerAssigned = false;
    }
}
