using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public bool isBusy = false;
    public bool isActive = false;
    float tChange = 0;
    public int randomMoveThreshold;
    public float lowerMoveThreshold;
    public float higherMoveThreshold;
    private Vector3 directionX = Vector3.right;
    private int randomY;
    public float idleMoveSpeed;
    public float workingMoveSpeed;
    float currMoveSpeed;
    bool turnBack = false;
    bool moveHorz;
    public GameObject goalSlotObj;
    Rigidbody2D rgBody;

    public int pointY;//this is the Point system Y coordinate
    public int pointX;
    bool toLadderUp = false;
    bool toLadderDown = false;

    public int timeToKillCitizen;
    public int timeToDestroyBuilding;

    int turnAroundCount = 1;
    
    Vector3 prevPosition;

    void Start()
    {
        rgBody = this.GetComponent<Rigidbody2D>();
        /*pointY = (int)((transform.localPosition.y - 0.8f) / 2.5f + 1);*/ //TODO: Bug where citizens at higher Y will go to wrong coords
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
        pointX = (int)(transform.localPosition.x / 1.75f) + 4;
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
        pointX = (int)(transform.localPosition.x / 1.75f) + 4;
        prevPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevPosition.x > this.transform.position.x)
        {
            //moving left
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            //moving right
            GetComponent<SpriteRenderer>().flipX = false;
        }
        prevPosition = this.transform.position;

        //if isBusy false, walk around randomly
        if (!isBusy && isActive)
        {
            currMoveSpeed = idleMoveSpeed;

            //find nearest citizen / building, call gotoslot, change to busy, attack the building/citizen until destroyed, change busy back
            //to false
            //spawns only on bottom left,  (or left most), so go in one direction until you meet a citizen/building, and call
            //attack when you do

            transform.Translate(directionX * currMoveSpeed * Time.deltaTime);
        }

        pointX = (int)(transform.localPosition.x / 1.75f) + 4;

        

        if (!isActive)
        {
            //i.e dead or inactive
            //offscreen and not moving
            transform.position = new Vector3(1000, 1000);
            isBusy = false;
        }
        


    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Enemy clicked");
            //get nearest soldier, gotoslot
            PlatformMapScript.Point currPoint = new PlatformMapScript.Point(pointX, pointY);
            Soldier selectedSoldier = PlatformGameManager.instance.GetSoldier(currPoint);
            
            if (selectedSoldier != null)
            {
                
                selectedSoldier.coroutine = StartCoroutine(selectedSoldier.GoToSlot(currPoint));
                selectedSoldier.goalPoint = currPoint;
            }

        }
    }

    public IEnumerator GoToSlot(PlatformMapScript.Point slotPoint)
    {

        //TODO: Bug where if you have 1 ladder, but you need 2 ladders to reach the goal slot at the top, isBusy never gets reset to false, 
        //citizen is thus stuck forever

        //TODO: Bug? where if there's more than 1 ladder per level, they cant decide which ladder to go to and become stuck

        //called by slot?
        //checks if y is higher, if true, find the nearest ladder, climbs it, and calls this method again
        //PlatformMapScript.Point slotPoint = slot.GetComponent<SlotScript>().point;
        if (slotPoint.y > pointY)
        {

            toLadderUp = true;

            //go to nearest ladder on same level
            for (int i = 0; i < PlatformGameManager.instance.ladderSlots.Count; i++)
            {

                if (pointY == PlatformGameManager.instance.ladderSlots[i].point.y)
                {

                    StartCoroutine(GoToSlot(PlatformGameManager.instance.ladderSlots[i].point));
                }
            }

            //Debug.Log("No Ladder! Cannot reach!"); //TODO: prompt
        }
        else if (slotPoint.y < pointY)
        {
            toLadderDown = true;


            //go to nearest ladder on lower level
            for (int i = 0; i < PlatformGameManager.instance.ladderSlots.Count; i++)
            {

                if (pointY - 1 == PlatformGameManager.instance.ladderSlots[i].point.y)
                {

                    StartCoroutine(GoToSlot(PlatformMapScript.instance.slotArray[(int)PlatformGameManager.instance.ladderSlots[i].point.y + 1, (int)PlatformGameManager.instance.ladderSlots[i].point.x].GetComponent<SlotScript>().point));
                }
            }
            Debug.Log("No Ladder! Cannot reach!"); //TODO: prompt
        }
        else
        {
            //moveHorz = true;
            isBusy = true;
            currMoveSpeed = workingMoveSpeed;
            while (transform.position.x != slotPoint.PointToCoord().x)
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
                StartCoroutine(GoToSlot(goalSlotObj.GetComponent<SlotScript>().point));

            }
            else if (toLadderDown)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z);
                pointY--;
                toLadderDown = false;
                StartCoroutine(GoToSlot(goalSlotObj.GetComponent<SlotScript>().point));

            }
            else
            {

                //StartCoroutine(Attack(slot));
            }


        }


        //slot x might be same or around the same as citizen, try to catch this case
        //when reaches slot, calls the appropriate method in slot/gamemanager to remove tree/build house
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Blocked")
        {
            Debug.Log("Enemy Blocked");
            //turnBack = true;
            if(directionX == Vector3.right)
            {
                directionX = Vector3.left;
            }
            else
            {
                directionX = Vector3.right;
            }
            turnAroundCount++;
            if(turnAroundCount >= 2)
            {
                toLadderUp = true;
                turnAroundCount = 0;
            }
        }

        if(other.gameObject.tag == "Ladder" && toLadderUp)
        {
            //move enemy up by one level
            transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            pointY++;
            toLadderUp = false;
        }

        if (other.gameObject.tag == "Citizen" && !isBusy)
        {
            //can only attack one citizen at a time
            if (!other.GetComponent<Citizen>().isAttacked)
            {
                other.GetComponent<Citizen>().isAttacked = true;
                StartCoroutine(Attack(other.gameObject, true));
            }
            
        }

        if(other.gameObject.tag == "Building" && !isBusy)
        {
            if (!other.GetComponent<Building>().isAttacked)
            {
                other.GetComponent<Building>().isAttacked = true;
                StartCoroutine(Attack(other.gameObject, false));
            }
                
        }


    }


    IEnumerator Attack(GameObject victim, bool isCitizen)
    {
        isBusy = true;
        if (isCitizen)
        {
            yield return new WaitForSeconds(timeToKillCitizen);
            PlatformGameManager.instance.KillCitizen(victim);
        }
        else
        {
            yield return new WaitForSeconds(timeToDestroyBuilding);
            Destroy(victim); //destroy house with no reward //maybe influence goes down?
        }
        
        
        isBusy = false;
    }

    IEnumerator MoveTo(GameObject goal)
    {
        while (transform.position.x != goal.GetComponent<SlotScript>().point.PointToCoord().x)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                        new Vector3(goal.GetComponent<SlotScript>().point.PointToCoord().x, transform.position.y, transform.position.z),
                        workingMoveSpeed * Time.deltaTime);
        }

        yield return null;
    }

    
}
