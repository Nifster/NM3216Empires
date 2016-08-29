using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class GridScript : MonoBehaviour {

    public GameObject gridNode; // Refering to the gird object prefab that we created

    int gridHeight = 10; //This is how tall your grid will be
    int gridWidth = 20; //This is how wide your grid will be

    //void Update() // This is called every frame of the game
    //{
    //    if (Input.GetKeyDown(KeyCode.Space)) //Cheaking if the player pressed space
    //    {
    //        gridGen(); //Calling the gridGen function (below)
    //    }
    //}

    void Start()  //If you want you can call this function at anytime with gridGen();
    {
        for (int vert = 0; vert < gridHeight; vert++) //adding 1 to vert everytime it is called
        {
            for (int hor = 0; hor < gridWidth; hor += 2) //adding 2 to hor everytime it is called
            {
                //Creating the prefab at the right x and y position:
                GameObject gridObject = Instantiate(gridNode, new Vector3(hor, vert, 0), Quaternion.identity) as GameObject;
                //Naming that object appropriatly:
                gridObject.name = "X:" + gridObject.transform.position.x + "Y:" + gridObject.transform.position.y;
                //EditorApplication.SaveScene();
                //EditorSceneManager.SaveOpenScenes();
            }
        }
    }
}
