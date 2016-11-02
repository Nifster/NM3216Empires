using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private PlatformGameManager.Buildings thisBuilding;
    public enum BuildingType
    {
        House,
        Barracks,
        Ladder,
        Pyramid
    };

    public GameObject infoPanel;
    public Text labourText;
    public Text lumberText;
    public Text oreText;
    public Text influenceCostText;
    public Text influenceRewardText;
    public Text buildTimeText;
    

    public BuildingType buildingType;
	// Use this for initialization
	void Start () {
        switch (buildingType)
        {
            case (BuildingType.House):
                thisBuilding = PlatformGameManager.instance.House;
                break;
            case (BuildingType.Barracks):
                thisBuilding = PlatformGameManager.instance.Barracks;
                break;
            case (BuildingType.Ladder):
                thisBuilding = PlatformGameManager.instance.Ladder;
                break;
            case (BuildingType.Pyramid):
                thisBuilding = PlatformGameManager.instance.Pyramid;
                break;
        }
        
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
        labourText.text = thisBuilding.labourCost.ToString();
        lumberText.text = thisBuilding.lumberCost.ToString();
        oreText.text = thisBuilding.oreCost.ToString();
        influenceCostText.text = thisBuilding.influenceCost.ToString();
        //influenceRewardText.text = thisBuilding.influenceReward.ToString();
        buildTimeText.text = thisBuilding.timeToBuild.ToString();
        //infoPanel.transform.position = this.transform.position;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
