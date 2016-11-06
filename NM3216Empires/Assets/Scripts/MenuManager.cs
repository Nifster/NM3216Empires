using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour {

    public GameObject creditsPanel;
    public GameObject instructionsPanel;
    public List<Sprite> instructionsPanelSprites;
    public GameObject nextInstructionButton;
    int instructionsIndex;

	// Use this for initialization
	void Start () {
        creditsPanel.SetActive(false);
	}
	
    public void PlayGame()
    {
        SceneManager.LoadScene("Platform");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
    }
    
    public void BackFromCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void InstructionsButton()
    {
        //activate instructionsPanel;
        instructionsPanel.SetActive(true);
    }

    public void NextInstructionsButton()
    {
        if(instructionsIndex < instructionsPanelSprites.Count-1)
        {
            instructionsIndex++;
            instructionsPanel.GetComponent<Image>().sprite = instructionsPanelSprites[instructionsIndex];
            if(instructionsIndex >= 7)
            {
                //move the button to the new position
                nextInstructionButton.GetComponent<RectTransform>().localPosition = new Vector3(276, -173, 0);
            }
        }
        else
        {
            instructionsIndex = 0;
            instructionsPanel.GetComponent<Image>().sprite = instructionsPanelSprites[instructionsIndex];
            //move the button to the new position
            nextInstructionButton.GetComponent<RectTransform>().localPosition = new Vector3(276, -246, 0);
            instructionsPanel.SetActive(false);
        }
        
    }
}
