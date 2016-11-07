using UnityEngine;
using System.Collections;

public class Citizen : MonoBehaviour {

    public bool isBusy = false;
    public bool isActive = false;
    float tChange = 0;
    public int randomMoveThreshold;
    public float lowerMoveThreshold;
    public float higherMoveThreshold;
    private int randomX;
    private int randomY;
    public float idleMoveSpeed;
    public float workingMoveSpeed;
    float currMoveSpeed;
    bool turnBack = false;
    bool moveHorz;
    public GameObject goalSlotObj;
    Rigidbody2D rgBody;
    public bool isAttacked = false;

    public int pointY;//this is the Point system Y coordinate
    public int pointX;
    bool toLadderUp = false;
    bool toLadderDown = false;
    bool buildingSet = false;

    Vector3 prevPosition;

    // Use this for initialization
    void Start () {
        rgBody = this.GetComponent<Rigidbody2D>();
        /*pointY = (int)((transform.localPosition.y - 0.8f) / 2.5f + 1);*/ //TODO: Bug where citizens at higher Y will go to wrong coords
        if(transform.localPosition.y == -1.9f)
        {
            pointY = 0;
        }else if(transform.localPosition.y == 0.6f)
        {
            pointY = 1;
        }else if(transform.localPosition.y == 3.1f)
        {
            pointY = 2;
        }
        pointX = (int)(transform.localPosition.x / 1.75f)+4;
    }

    public void ResetPointPosition()
    {
        if (transform.localPosition.y == -1.9f)
        {
            pointY = 0;
        }
        else if (transform.localPosition.y == 0.6f)
        {
            pointY = 1;
        }
        else if (transform.localPosition.y == 3.1f)
        {
            pointY = 2;
        }
        else
        {
            pointY = -1;
        }
        pointX = (int)(transform.localPosition.x / 1.75f) + 4;
        prevPosition = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        
        if(prevPosition.x > this.transform.position.x)
        {
            //moving left
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            //moving right
            GetComponent<SpriteRenderer>().flipX = true;
        }
        prevPosition = this.transform.position;
        //if isBusy false, walk around randomly
        if (!isBusy && isActive)
        {
            currMoveSpeed = idleMoveSpeed;
            if (Time.time >= tChange)
            {
                randomX = Random.Range(-randomMoveThreshold, randomMoveThreshold); // with float parameters, a random float
                                                                                   //randomY = Random.Range(-randomMoveThreshold, randomMoveThreshold); //  between -2.0 and 2.0 is returned
                                                                                   // set a random interval between 0.5 and 1.5
                tChange = Time.time + Random.Range(lowerMoveThreshold, higherMoveThreshold);
            }

            //if x less than point 8,0 and more than point 0,0
            if(randomX < new PlatformMapScript.Point(8,0).PointToCoord().x && randomX > new PlatformMapScript.Point(0, 0).PointToCoord().x)
            {
                transform.Translate(new Vector3(randomX, 0, 0) * idleMoveSpeed * Time.deltaTime);
            }

        }

        if (isAttacked)
        {
            isBusy = true;
            currMoveSpeed = 0;
        }

        if (!isActive)
        {
            //i.e dead or inactive
            //offscreen and not moving
            transform.position = new Vector3(5000, 5000);
            isBusy = false;
        }

        pointX = (int)(transform.localPosition.x / 1.75f) + 4;


    }

    public IEnumerator GoToSlot(GameObject slot, int buildIndex)
    {
              
        //TODO: Bug where if you have 1 ladder, but you need 2 ladders to reach the goal slot at the top, isBusy never gets reset to false, 
        //citizen is thus stuck forever

        //TODO: Bug? where if there's more than 1 ladder per level, they cant decide which ladder to go to and become stuck

        //called by slot?
        //checks if y is higher, if true, find the nearest ladder, climbs it, and calls this method again
        PlatformMapScript.Point slotPoint = slot.GetComponent<SlotScript>().point;

        if (!buildingSet)
        {
            buildIndex = PlatformGameManager.instance.selectedBuildingIndexToBuild;
            buildingSet = true;
        }
        
        if (slotPoint.y  > pointY)
        {
            
            toLadderUp = true;

            //go to nearest ladder on same level
            for(int i =0; i < PlatformGameManager.instance.ladderSlots.Count; i++)
            {
                
                if(pointY == PlatformGameManager.instance.ladderSlots[i].point.y)
                {
                    
                    StartCoroutine(GoToSlot(PlatformGameManager.instance.ladderSlots[i].gameObject,buildIndex));
                }
            }

            //Debug.Log("No Ladder! Cannot reach!"); //TODO: this is wrong place to deduce this
        }
        else if (slotPoint.y < pointY)
        {
            toLadderDown = true;
            

            //go to nearest ladder on lower level
            for (int i = 0; i < PlatformGameManager.instance.ladderSlots.Count; i++)
            {

                if (pointY-1 == PlatformGameManager.instance.ladderSlots[i].point.y)
                {
                    
                    StartCoroutine(GoToSlot(PlatformMapScript.instance.slotArray[(int)PlatformGameManager.instance.ladderSlots[i].point.y+1, (int)PlatformGameManager.instance.ladderSlots[i].point.x],buildIndex));
                }
            }
            //Debug.Log("No Ladder! Cannot reach!"); //TODO: this is wrong place to deduce this
        }
        else
        {
            //moveHorz = true;
            isBusy = true;
            currMoveSpeed = workingMoveSpeed;
            while (transform.position.x != slot.GetComponent<SlotScript>().point.PointToCoord().x)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                            new Vector3(slotPoint.PointToCoord().x, transform.position.y, transform.position.z),
                            workingMoveSpeed * Time.deltaTime);
                yield return null;
            }
            if (toLadderUp)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
                pointY++;
                toLadderUp = false;
                StartCoroutine(GoToSlot(goalSlotObj,buildIndex));
                
            }
            else if (toLadderDown)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z);
                pointY--;
                toLadderDown = false;
                StartCoroutine(GoToSlot(goalSlotObj,buildIndex));
                
            }
            else
            {

                StartCoroutine(Harvest(slot,buildIndex));
            }

            
        }
        

        //slot x might be same or around the same as citizen, try to catch this case
        //when reaches slot, calls the appropriate method in slot/gamemanager to remove tree/build house
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Blocked")
        {
            //turnBack = true;
            randomX = -randomX;
        }

        if (other.gameObject.Equals(goalSlotObj)){
            
            //StartCoroutine(Harvest(1,goalSlotObj));
            
        }
    }

    /// <summary>
    /// Makes Citizen wait for secs no. of seconds, stop for a few secs to "harvest", and then calls gamemanager
    /// to do whatever on the slot. Citizen goes back to being free
    /// </summary>
    /// <param name="secs"></param>
    /// <returns></returns>
    IEnumerator Harvest(GameObject slotObj,int buildIndex)
    {
        //moveHorz = false;
        SlotScript slot = slotObj.GetComponent<SlotScript>();
        SlotScript.Building slotBuildingType = slot.currBuilding;
        GameObject buildingObj = slot.buildingObj;
        //int currentlyBuildingIndex = -1;
        PlatformGameManager.Buildings currentlyBuilding = null;
        if(buildIndex >= 0)
        {
            //if there's a building to build
            currentlyBuilding = PlatformGameManager.instance.ChooseBuildingFromIndex(buildIndex);
            slot.UpdateResourceTimerValue(currentlyBuilding);
            PlatformGameManager.instance.sfx.clip = PlatformGameManager.instance.buildingSfx;
            PlatformGameManager.instance.sfx.Play();
            yield return new WaitForSeconds(currentlyBuilding.timeToBuild);
            
        }
        else
        {
            if(slotBuildingType == SlotScript.Building.Tree || slotBuildingType == SlotScript.Building.Rock)
            {
                if(slotBuildingType == SlotScript.Building.Tree)
                {
                    PlatformGameManager.instance.sfx.clip = PlatformGameManager.instance.woodChopSfx;
                    PlatformGameManager.instance.sfx.Play();
                }
                else
                {
                    PlatformGameManager.instance.sfx.clip = PlatformGameManager.instance.miningSfx;
                    PlatformGameManager.instance.sfx.Play();
                }
                slot.UpdateResourceTimerValue(PlatformGameManager.instance.Tree);
                yield return new WaitForSeconds(5); //default harvest time
            }
            else if (PlatformGameManager.instance.demolishMode)
            {
                PlatformGameManager.instance.sfx.clip = PlatformGameManager.instance.buildingSfx;
                PlatformGameManager.instance.sfx.Play();
                slot.UpdateResourceTimerValue(PlatformGameManager.instance.Tree);
                yield return new WaitForSeconds(5); //default harvest time
            }
            
        }
       
        isBusy = false;
        
        //Get slot building enum type
        
        if(slotBuildingType == SlotScript.Building.None)
        {
            PlatformGameManager.instance.BuildSelected(slotObj, buildIndex);
            PlatformGameManager.instance.selectedBuildingIndexToBuild = -1;
            PlatformGameManager.instance.selectedBuildingToBuild = null;
        }
        else
        {
            if (PlatformGameManager.instance.demolishMode && (slotBuildingType != SlotScript.Building.Tree && slotBuildingType != SlotScript.Building.Rock))
            {
                //if in demolish mode, start demolishing the target building
                //Debug.Log("Demolishing");
                //slot.GetComponent<SlotScript>().currBuilding = SlotScript.Building.None;
                //Destroy(slot.buildingObj);
                ////give demolishing rewards
                //PlatformGameManager.instance.BuildingDemolishedAddReward();
                ////turn off demolishmode
                //PlatformGameManager.instance.demolishMode = false;
                PlatformGameManager.instance.DemolishBuilding(slot,true);
            }
            else
            {
                if (slotBuildingType == SlotScript.Building.Tree)
                {
                    //tell gamemanager to add to lumber count
                    PlatformGameManager.instance.TreeHarvested();
                    //Minus tree health, destroy if last health
                    slot.resourceHealth--;
                    if (slot.resourceHealth == 0)
                    {
                        Destroy(buildingObj);
                        slot.DestroyHealth();
                        //set building to none
                        slot.currBuilding = SlotScript.Building.None;
                    }


                }
                if (slotBuildingType == SlotScript.Building.Rock)
                {
                    //tell gamemanager to add to lumber count
                    PlatformGameManager.instance.RockHarvested();
                    //Minus tree health, destroy if last health
                    slot.resourceHealth--;
                    if (slot.resourceHealth == 0)
                    {
                        Destroy(buildingObj);
                        slot.DestroyHealth();
                        //set building to none
                        slot.currBuilding = SlotScript.Building.None;
                    }

                }
                slot.UpdateResourceValue();
            }
            
        }
        
    }


    
}
