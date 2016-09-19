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

        public Coord PointToCoord()
        {
            Coord newCoord = new Coord(this.x *XOFFSET - Mathf.Floor(map.columns / 2) * XOFFSET, this.y*YOFFSET - YOFFSET);
            return newCoord;
        }
    }

    public class Coord
    {
        public float x;
        public float y;

        public Coord(float xX, float yY)
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

    public static Map map;
    public int mapSize = 5; //no. of platforms per row
    public static float XOFFSET = 1.75f;
    public static float YOFFSET = 2.5f;
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
                GameObject newSlot = Instantiate(slotPrefab);
                //newSlot.transform.SetParent(transform.parent);
                newSlot.transform.localPosition = new Vector2(i * XOFFSET - (Mathf.Floor(map.columns/2) * XOFFSET), (j * YOFFSET) - YOFFSET);
                //newSlot.transform.localScale = new Vector3(1, 1, 1);
                newSlot.GetComponent<SlotScript>().point = new Point(i, j);
            }
        }

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
