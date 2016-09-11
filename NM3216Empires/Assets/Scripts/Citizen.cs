using UnityEngine;
using System.Collections;

public class Citizen : MonoBehaviour {

    public bool isBusy = false;
    float tChange = 0;
    public int randomMoveThreshold;
    public float lowerMoveThreshold;
    public float higherMoveThreshold;
    private int randomX;
    private int randomY;
    public float moveSpeed;
    bool turnBack = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //if isBusy false, walk around randomly
        if (!isBusy)
        {
            if (Time.time >= tChange)
            {
                randomX = Random.Range(-randomMoveThreshold, randomMoveThreshold); // with float parameters, a random float
                //randomY = Random.Range(-randomMoveThreshold, randomMoveThreshold); //  between -2.0 and 2.0 is returned
                                                                                   // set a random interval between 0.5 and 1.5
                tChange = Time.time + Random.Range(lowerMoveThreshold, higherMoveThreshold);
            }
            
            transform.Translate(new Vector3(randomX, 0, 0) * moveSpeed * Time.deltaTime);
            
        }
	}

    public void GoToSlot(GameObject slot)
    {
        //
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Blocked")
        {
            Debug.Log("Blocked");
            //turnBack = true;
            randomX = -randomX;
        }
    }
}
