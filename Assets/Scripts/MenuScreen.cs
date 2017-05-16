using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour {

    public GameObject ropeSprite;

	// Use this for initialization
	void Start () {

        //Reset the levels
        LevelData.level = 1;
        LevelData.round = 1;

        //Create the ropes

        float xStart = -4.7f;
        float yStart = -1.9f;
        float ropeHeight = 0.3f;

        for(int i=0; i< 28; ++i) {
            var ob = Instantiate(ropeSprite, new Vector3(xStart, yStart + i * ropeHeight, 5), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0)) {
            SceneManager.LoadScene("LevelScreen");
        }
    }
}
