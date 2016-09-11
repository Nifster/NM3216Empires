using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image highlight;

    private enum Building
    {
        None,
        Tree,
        House
    };

    private Building _currBuilding;
    private GameObject buildingObj;

	// Use this for initialization
	void Start () {

        highlight = transform.GetChild(0).GetComponent<Image>();
        buildingObj = transform.GetChild(1).gameObject;
        if(buildingObj == null)
        {
            _currBuilding = Building.None;
        }else if (buildingObj.name.Contains("Tree"))
        {
            _currBuilding = Building.Tree;
        }else if (buildingObj.name.Contains("House"))
        {
            _currBuilding = Building.House;
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        Debug.Log("CLICKED");
        if(_currBuilding == Building.Tree)
        {
            //citizens gather tree
            //tell gamemanager to add to lumber count
            PlatformGameManager.instance.TreeHarvested();
            //destroy the tree
            Destroy(buildingObj);
            //set building to none
            _currBuilding = Building.None;
        }

        if(_currBuilding == Building.None)
        {
            //check what is the building selected to be built
            //check if resource req for selected building is met
            //builds building
            //tells GameManager to remove req resources from count
        }
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        Color translucent = highlight.color;
        translucent.a = 0.5f;
        highlight.color = translucent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color transparent = highlight.color;
        transparent.a = 0;
        highlight.color = transparent;
    }
}
