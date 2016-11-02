using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{

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
    public PlatformMapScript.Point goalPoint;
    Rigidbody2D rgBody;
    public int maxHealth = 2;
    public int currHealth;
    public int timeToKillEnemy = 5;
    bool isAttacking;
    public Coroutine coroutine;

    public int pointY;//this is the Point system Y coordinate
    public int pointX;
    bool toLadderUp = false;
    bool toLadderDown = false;

    Vector3 prevPosition;

    // Use this for initialization
    void Start()
    {
        currHealth = maxHealth;
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
        prevPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (prevPosition.x > this.transform.position.x)
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

            transform.Translate(new Vector3(randomX, 0, 0) * idleMoveSpeed * Time.deltaTime);

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

    public IEnumerator GoToSlot(PlatformMapScript.Point slotPoint)
    {
        if (!isAttacking)
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
                    if (!isAttacking)
                    {
                        transform.position = Vector3.MoveTowards(transform.position,
                                new Vector3(slotPoint.PointToCoord().x, transform.position.y, transform.position.z),
                                workingMoveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        yield break;
                    }
                    yield return null;
                }
                if (toLadderUp)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
                    pointY++;
                    toLadderUp = false;
                    StartCoroutine(GoToSlot(goalPoint));

                }
                else if (toLadderDown)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z);
                    pointY--;
                    toLadderDown = false;
                    StartCoroutine(GoToSlot(goalPoint));

                }
                else
                {
                    isBusy = false;
                    //StartCoroutine(Attack(slot)); //have to reset to not busy
                }


            }


            //slot x might be same or around the same as citizen, try to catch this case
            //when reaches slot, calls the appropriate method in slot/gamemanager to remove tree/build house
        }
        else
        {
            Debug.Log("STOP");
            yield break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Blocked")
        {
            //turnBack = true;
            randomX = -randomX;
        }

        if(other.gameObject.tag == "Enemy")
        {
            if (!other.GetComponent<Enemy>().isBusy && !isAttacking)
            {
                //attack and kill
                isAttacking = true;
                //StopAllCoroutines();
                if(coroutine!= null)
                {
                    //StopCoroutine(coroutine);
                }
                other.GetComponent<Enemy>().isBusy = true;
                StartCoroutine(Attack(other.gameObject));
                //if health is zero, becomes inactive
                
            }
            else if(other.GetComponent<Enemy>().isBusy)
            {
                Debug.Log("Enemy busy..");
            }
            
        }

       
    }


    IEnumerator Attack(GameObject victim)
    {
        isBusy = true;
        yield return new WaitForSeconds(timeToKillEnemy);
        //set enemy to inactive
        PlatformGameManager.instance.KillEnemy(victim);
        currHealth--;
        if (currHealth <= 0)
        {
            PlatformGameManager.instance.KillSoldier(this);
        }
        isBusy = false;
        isAttacking = false;
    }

    IEnumerator MoveTo(GameObject goal)
    {
        while (transform.position.x != goal.GetComponent<SlotScript>().point.PointToCoord().x)
        {
            if (!isAttacking)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                            new Vector3(goal.GetComponent<SlotScript>().point.PointToCoord().x, transform.position.y, transform.position.z),
                            workingMoveSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("WHY");
                yield break;
            }
        }

        yield return null;
    }


}
