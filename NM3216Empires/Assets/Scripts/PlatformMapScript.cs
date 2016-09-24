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
    public GameObject treePrefab;
    public GameObject rockPrefab;


    public List<Point> treePositions;
    public List<Point> rockPositions;

    [System.Serializable]
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

    public static PlatformMapScript instance;
    public static Map map;
    public int mapSize = 5; //no. of platforms per row
    public static float XOFFSET = 1.75f;
    public static float YOFFSET = 2.5f;
    public GameObject slotPrefab;
    public GameObject[,] slotArray;

    // Use this for initialization
    void Awake () {
        instance = this;
        slotArray = new GameObject[3,mapSize];
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
                slotArray[j, i] = newSlot;
                
                for(int k=0; k<treePositions.Count; k++)
                {
                    if (treePositions[k].x == (newPoint.x) && treePositions[k].y == (newPoint.y))
                    {
                        GameObject newBuilding = Instantiate(treePrefab);
                        newBuilding.transform.SetParent(newSlot.transform);
                        newBuilding.transform.localScale = new Vector3(0.25f, 0.5f, 0);
                        newBuilding.transform.localPosition = new Vector3(0, 1.1f, 0);
                    }
                }
                for (int k = 0; k < rockPositions.Count; k++)
                {
                    if (rockPositions[k].x == (newPoint.x) && rockPositions[k].y == (newPoint.y))
                    {
                        GameObject newBuilding = Instantiate(rockPrefab);
                        newBuilding.transform.SetParent(newSlot.transform);
                        newBuilding.transform.localScale = new Vector3(0.8f, 2f, 0);
                        newBuilding.transform.localPosition = new Vector3(0, 1.05f, 0);
                    }
                }
            }
        }

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
