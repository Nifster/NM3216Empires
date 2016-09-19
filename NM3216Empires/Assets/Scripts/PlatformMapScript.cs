using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlatformMapScript : MonoBehaviour
{
    //0000000000
    //xxxxxxxxxx
    //0000000000
    //xxxxxxxxxx
    //0000000000
    //xxxxxxxxxx

    //0 is slot
    //x is platform

    public class Point
    {
        public float x;
        public float y;

        public Point(float xX, float yY)
        {
            x = xX;
            y = yY;
        }

    }

    

    public class Map
    {
        public List<Point> points;
        public int rows = 3; // default no. of rows is three
        public int columns;

        
    }

    public Map map;
    public int mapSize = 5; //no. of platforms per row
    static int XOFFSET = 90;
    static int YOFFSET = 120;
    public GameObject slotPrefab;

    // Use this for initialization
    void Start () {

        map = new Map();
        map.points = new List<Point>();
        map.columns = mapSize;
        for(int i=0; i < (map.columns); i++)
        {
            for(int j=0; j<map.rows; j++)
            {
                Point newPoint = new Point(i, j);
                map.points.Add(newPoint);
                Debug.Log(i+ " , "+ j);
                Debug.Log((i * 90 - 180) + " , " + (j*120 - 120));
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(transform.parent);
                newSlot.transform.localPosition = new Vector2(i * XOFFSET - (Mathf.Floor(map.columns/2) * XOFFSET), (j * YOFFSET) - YOFFSET);
                newSlot.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
