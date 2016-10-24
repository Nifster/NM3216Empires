using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public GameObject creditsPanel;
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
}
