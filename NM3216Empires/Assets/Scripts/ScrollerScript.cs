using UnityEngine;
using System.Collections;

public class ScrollerScript : MonoBehaviour {

    public GameObject ground1;
    public GameObject ground2;
    public GameObject buildings;

    public float speed;
    float screenWidth;
	// Use this for initialization
	void Start () {

        //find screen width and save the value
        screenWidth = Camera.main.pixelWidth;
	}
	
	// Update is called once per frame
	void Update () {
        //make ground move to the left
        ScrollObject(ground1);
        ScrollObject(buildings);
        ScrollObject(ground2);
        if(ground1.transform.position.x <= -50)
        {
            ground1.transform.position = new Vector2(ground2.transform.position.x + 50, ground1.transform.position.y);
        }

        if(ground2.transform.position.x <= -50)
        {
            ground2.transform.position = new Vector2(ground1.transform.position.x + 50, ground2.transform.position.y);
        }
	
	}

    void ScrollObject(GameObject obj)
    {
        float x1 = obj.transform.position.x;
        float y1 = obj.transform.position.y;

        obj.transform.position = new Vector3(x1 - (speed * Time.deltaTime), y1, 0);
    }
}
