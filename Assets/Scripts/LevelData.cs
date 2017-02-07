using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {

    //public static List<int> randBoxesPerRound = new List<int>(new int[] {1,2,3});
    public static int round = 1;
    public static int level = 1;


    public static List<int> maxDiffPerLevel = new List<int>(new int[] { 5, 3, 1, 5, 3, 5, 3, 0});
    public static List<int> turnsPerLevel = new List<int>(new int[] { 3, 3, 3, 2, 2, 1, 1, 1});
    public static List<Color32> colorOfLevels =  new List<Color32> (new Color32[] {
        new Color32(14,180,100,255),
        new Color32(180,14,66,255),
        new Color32(14,53,180,255),
        new Color32(180,140,14,255),
        new Color32(130,14,160,255),
        new Color32(14,180,194,255),
        new Color32(180,61,14,255),
        new Color32(95,95,95,255)
    });

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
