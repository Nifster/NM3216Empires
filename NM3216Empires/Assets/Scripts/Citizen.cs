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
    GameObject goalSlotObj;
    Rigidbody2D rgBody;

    // Use this for initialization
    void Start () {
        rgBody = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(isBusy);
        //if isBusy false, walk around randomly
        if (!isBusy)
        {
            currMoveSpeed = idleMoveSpeed;
            if (Time.time >= tChange)
            {
                randomX = Random.Range(-randomMoveThreshold, randomMoveThreshold); // with float parameters, a random float
                                                                                   //randomY = Random.Range(-randomMoveThreshold, randomMoveThreshold); //  between -2.0 and 2.0 is returned
                                                                                   // set a random interval between 0.5 and 1.5
                tChange = Time.time + Random.Range(lowerMoveThreshold, higherMoveThreshold);
            }

            transform.Translate(new Vector3(randomX, 0, 0) * idleMoveSpeed * Time.deltaTime);

        }
        else
        {
            currMoveSpeed = workingMoveSpeed;
            if (moveHorz)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                        new Vector3(goalSlotObj.GetComponent<SlotScript>().point.PointToCoord().x, transform.position.y, transform.position.z),
                        workingMoveSpeed * Time.deltaTime);
                if(transform.position.x == goalSlotObj.GetComponent<SlotScript>().point.PointToCoord().x)
                {
                    Debug.Log("REACHED");
                    StartCoroutine(Harvest(1, goalSlotObj));
                    //call harvest action here
                }
            }
        }
    }

    public void GoToSlot(GameObject slot)
    {
        isBusy = true;
        goalSlotObj = slot;
        //called by slot?
        //checks if y is higher, if true, find the nearest ladder, climbs it, and calls this method again
        Vector2 slotPos = slot.transform.position;
        if(slotPos.y > transform.position.y)
        {
            //go to nearest ladder
        }
        else
        {
            moveHorz = true;
            
        }
        
        //slot x might be same or around the same as citizen, try to catch this case
        //when reaches slot, calls the appropriate method in slot/gamemanager to remove tree/build house
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Blocked")
        {
            Debug.Log("Blocked");
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
    IEnumerator Harvest(float secs,GameObject slotObj)
    {
        moveHorz = false;
        yield return new WaitForSeconds(5);
        isBusy = false;
        
        //Get slot building enum type
        SlotScript slot = slotObj.GetComponent<SlotScript>();
        SlotScript.Building slotBuildingType = slot.currBuilding;
        GameObject buildingObj = slot.buildingObj;
        if (slotBuildingType == SlotScript.Building.Tree)
        {
            //tell gamemanager to add to lumber count
            PlatformGameManager.instance.TreeHarvested();
            //Minus tree health, destroy if last health
            Destroy(buildingObj);
            //set building to none
            slot.currBuilding = SlotScript.Building.None;
        }
        
    }

    
}
