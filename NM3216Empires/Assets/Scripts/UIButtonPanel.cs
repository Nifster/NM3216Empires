using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButtonPanel : MonoBehaviour {

    public float target;
    public float moveTime;
    bool moving = false;
    public Vector3 originalPos;
    public Vector3 extendedPos;
    public GameObject panelToggle;
    public GameObject arrowImage;
    public Sprite leftSprite;
    public Sprite rightSprite;
	// Use this for initialization
	void Start () {
        originalPos = this.GetComponent<RectTransform>().anchoredPosition;
        extendedPos = new Vector3(target, this.GetComponent<RectTransform>().anchoredPosition.y, 0);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //ButtonPanel
    public void ButtonPanelTrigger()
    {
        //Debug.Log("_____" + this.GetComponent<RectTransform>().anchoredPosition);
        if (panelToggle.GetComponent<Toggle>().isOn)
        {
            StartCoroutine(MoveFromTo(originalPos, extendedPos, moveTime));
            arrowImage.GetComponent<Image>().sprite = rightSprite;
        }
        else
        {
            StartCoroutine(MoveFromTo(extendedPos, originalPos, moveTime));
            PlatformGameManager.instance.selectedBuildingToBuild = null;
            arrowImage.GetComponent<Image>().sprite = leftSprite;
        }
        
    }

    IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time)
    {
        if (!moving)
        { // do nothing if already moving
            moving = true; // signals "I'm moving, don't bother me!"
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time; // sweeps from 0 to 1 in time seconds
                this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(pointA, pointB, t); // set position proportional to t
                yield return 0; // leave the routine and return here in the next frame
            }
            moving = false; // finished moving
        }
    }
}
