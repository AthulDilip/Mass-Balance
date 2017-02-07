using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Camera>().backgroundColor = LevelData.colorOfLevels[LevelData.level - 1];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
