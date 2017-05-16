using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //Write to the Text objects
        var levelText = GameObject.Find("LevelText");
        var descText = GameObject.Find("DescText");

        levelText.GetComponent<Text>().text = "LEVEL "+LevelData.level;
        descText.GetComponent<Text>().text = "MAKE THE WEIGHT DIFFERENCE BELOW " + LevelData.maxDiffPerLevel[LevelData.level-1] +" IN "+ LevelData.turnsPerLevel[LevelData.level-1]+" TURNS";

    }
	
	// Update is called once per frame
	void Update () {
        //Handle mouseDown event
        if (Input.GetMouseButtonUp(0)) {
            SceneManager.LoadScene("GameScreen");
        }

    }
}
