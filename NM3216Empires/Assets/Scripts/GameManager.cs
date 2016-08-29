using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject buildingPrefab;

    Vector3 tileSizeInUnits = new Vector3(2.0f, 1.0f, 0.5f);
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Calculate ratios for simple grid snap
            float xx = Mathf.Round(mousePos.y / tileSizeInUnits.y - mousePos.x / tileSizeInUnits.x);
            float yy = Mathf.Round(mousePos.y / tileSizeInUnits.y + mousePos.x / tileSizeInUnits.x);

            // Calculate grid aligned position from current position
            //Vector3 position;
            float x = (yy - xx) * 0.5f * tileSizeInUnits.x;
            float y = (yy + xx) * 0.5f * tileSizeInUnits.y;
            //float z = 1.0f * position.y - 0.1f * position.x;
            //Debug.Log("X" + Mathf.Round(mousePos.x)*0.5f + "Y" + Mathf.Round(mousePos.y) * 0.5f);
            //Vector2 translatedMousePos = new Vector2(Mathf.Round(mousePos.x) * 0.5f, Mathf.Round(mousePos.y) * 0.5f + 0.25f);
            //find the nearest grid co-ord
            //instantiate building
            Instantiate(buildingPrefab, new Vector2(x, y+.25f),this.transform.rotation);

        }
	
	}
}
