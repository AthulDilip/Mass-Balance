using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour {

    // Use this for initialization
    public GameObject box;
    public GameObject ropeSprite;
    public GameObject pully;
    public GameObject currBox;
    public Rigidbody2D currBoxBody;
    public GameObject weightText;
    public GameObject roundText;
    public GameObject[] overlay;

    private bool clicked = true;
    private bool balanceUpdated = true;
    private bool growBox = false;
    private bool roundRestart = false;
    private bool levelRestart = false;

    //Varaiables that will effect the gameplay
    private int balanceVel = 1;
    private float boxGrowingSpeed = 3f;
    private float boxMaxSize = 2.2f;
    private float balanceLimit = 3.6f;
    private float balanceFriction = 250;
    private float balanceYStart = -1f;
    private int weightExpander = 30;     //Directlt controls difficulty of the game

    private float balanceMovingSpeed = 0;
    private float[] balanceWeights = { 0, 0 };
    private float weightDifference = 0;
    public List<GameObject> ropeList;
    public List<GameObject> randomBalls;
    private float yEnd = 0f;
    private float xEnd = 0f;
    private float xStart = 0f;
    private float ropeHeight = 0.3f;
    private int turns = 0;
    private int maxTurns = 0;
    private float maxDiff = 0;

    // Use this for initialization
    void Start () {

        //Initialize the Variables
        maxDiff = LevelData.maxDiffPerLevel[LevelData.level - 1];
        maxTurns = LevelData.turnsPerLevel[LevelData.level - 1];

        //Get the text object
        weightText = GameObject.Find("WeightText");
        roundText = GameObject.Find("RoundText");
        overlay = GameObject.FindGameObjectsWithTag("overlay");

        roundText.transform.position = new Vector3(Screen.width/2, 40, 0);
        roundText.GetComponent<Text>().text = "ROUND " + LevelData.round;

        // reate The rope
        var balance1 = GameObject.Find("Balance1");
        var balance2 = GameObject.Find("Balance2");
        var balanceHeight = balance1.GetComponent<BoxCollider2D>().bounds.size.y;
        ropeHeight = 0.3f;

        var yStart = balanceYStart + balanceHeight/2 + ropeHeight/2;
        yEnd = yStart + 15 * ropeHeight;

        xStart = balance1.transform.position.x;
        xEnd = balance2.transform.position.x;

        for(var i=0; i<60; ++i) {
            var ob = ropeSprite;
            if (i < 15) {
                ob = Instantiate(ropeSprite, new Vector3(balance1.transform.position.x, yStart + i * ropeHeight, 5), Quaternion.identity);
            }else if(i>=15 && i< 45) {
                ob = Instantiate(ropeSprite, new Vector3(xStart + (i - 15) * ropeHeight, yEnd, 5), Quaternion.Euler(0,0,90));
                //print(ob.transform.rotation.y);
            }else if(i >= 45) {
                ob = Instantiate(ropeSprite, new Vector3(balance2.transform.position.x, yEnd -(i-44)*ropeHeight, 5), Quaternion.Euler(0, 0, 180));
            }
            ropeList.Add(ob);
        }

        //Create the pullies
        Instantiate(pully, new Vector3(xStart-0.1f, yEnd-0.1f, 4), Quaternion.identity);
        Instantiate(pully, new Vector3(xEnd+0.1f, yEnd-0.1f, 4), Quaternion.identity);

        //Create Random boxes to start the game
        var lastX = balance2.transform.position.x - 2;
        for(var i=0; i<LevelData.round; ++i) {
            var randSpace = Random.Range(0.4f, 0.8f);

            var randWeight = 1.0f;

            if(LevelData.round == 1) {
                randWeight = Random.Range(1f, 1.8f);
            }else {
                randWeight = Random.Range(0.8f,1.8f);
            }

            var boxOb = Instantiate(box, new Vector3(lastX+randSpace, balanceYStart+2, 0), Quaternion.identity);
            boxOb.transform.localScale = new Vector3(randWeight,randWeight,0);
            boxOb.AddComponent<Rigidbody2D>();

            balanceWeights[1] += calculateWeight(boxOb.GetComponent<BoxCollider2D>().bounds.size.x*10 ) ;
            randomBalls.Add(boxOb);

            lastX = boxOb.transform.position.x + boxOb.GetComponent<BoxCollider2D>().bounds.size.x/2;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //Handle Random Balls
        var allRandomBallsSleeping = true;
        for (var i=0; i<randomBalls.Count; ++i) {
            if(!randomBalls[i].GetComponent<Rigidbody2D>().IsSleeping()) {
                allRandomBallsSleeping = false;
            }
        }

        if(allRandomBallsSleeping) {
            for(var i=0; i<randomBalls.Count; ++i) {

                randomBalls[i].tag = "balance2";

                var randBoxBody = randomBalls[i].GetComponent<Rigidbody2D>();
                Destroy(randBoxBody);
                adjustBalances();
            }

            randomBalls.Clear();
        }

        //Handle mouseDown event
        if (Input.GetMouseButtonDown(0)) {

            if(roundRestart) {
                roundRestart = false;
                SceneManager.LoadScene("GameScreen");
            }

            if(levelRestart) {
                levelRestart = false;
                SceneManager.LoadScene("LevelScreen");
            }

            if (!clicked) {
                placeBall();
            }
        }

        //Handle mouseUp Event
        if (Input.GetMouseButtonUp(0)) {
            if (!clicked) {
                dropBall();
            }

        }

        //check if box has come to rest
        if (currBox != null && !growBox) {

            var outOfScreen = false;
            currBoxBody = currBox.GetComponent<Rigidbody2D>();

            if (currBoxBody.position.y < -10) {
                currBoxBody.Sleep();
                outOfScreen = true;
            }

            if (currBoxBody.IsSleeping()) {
                
                if (currBoxBody.position.y >= -5) {

                    //Set tag for the box
                    if (currBoxBody.position.x < 0) {
                        currBox.tag = "balance1";
                    }
                    else {
                        currBox.tag = "balance2";
                    }

                    ++turns;
                    //update the balance
                    adjustBalances();
                }else {
                    clicked = false;
                }

                //Remove the rigid body property of the box
                Destroy(currBoxBody);
                if(outOfScreen) {
                    Destroy(currBox);
                    outOfScreen = false;
                }
                currBox = null;
            }
        }

        //check if box has to be grown
        if (growBox == true) {
            if (currBox != null && currBox.transform.localScale.x < boxMaxSize) {

                currBox.transform.localScale += new Vector3(boxGrowingSpeed, boxGrowingSpeed, 0) * Time.deltaTime;
            }
        }

        //update the balances
        if(!balanceUpdated) {
            var b1 = GameObject.Find("Balance1");
            var b2 = GameObject.Find("Balance2");

            if (Mathf.Abs(b1.transform.position.y + (weightDifference-balanceYStart)) < Mathf.Abs(balanceMovingSpeed*Time.deltaTime)) {

                checkLevelCompleted();

                balanceUpdated = true;
                clicked = false;
                
            } else {
                
                //update first balance
                var balance1 = GameObject.FindGameObjectsWithTag("balance1");
                for (int i = 0; i < balance1.Length; ++i) {
                    balance1[i].transform.Translate(new Vector3(0, -balanceMovingSpeed, 0) * Time.deltaTime, Space.World);
                }

                //update first balance
                var balance2 = GameObject.FindGameObjectsWithTag("balance2");
                for (int i = 0; i < balance2.Length; ++i) {
                    balance2[i].transform.Translate(new Vector3(0, balanceMovingSpeed, 0) * Time.deltaTime, Space.World);
                }

                //update the rope
                updateRope(-balanceMovingSpeed);
            }
        }

    }

    //This function will start growing a ball
    void placeBall() {
        growBox = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        currBox = Instantiate(box, new Vector3(ray.origin.x, 4.2f, 0), Quaternion.identity);
    }

    //This function will drop the ball
    void dropBall() {
        if (growBox == true) {
            growBox = false;
            clicked = true;

            currBox.AddComponent<Rigidbody2D>();
        }
    }

    //This funcion will update the rope
    void updateRope(float dist) {

        for(var i=0; i<60; ++i) {
            if(ropeList[i].transform.rotation.eulerAngles.z == 0) {
                //update the distance
                ropeList[i].transform.Translate(new Vector3(0, dist, 0) * Time.deltaTime, Space.World);

                if(ropeList[i].transform.position.y + ropeHeight/2 > yEnd) {
                    ropeList[i].transform.position =  new Vector3(ropeList[i+1].transform.position.x-ropeHeight + dist*Time.deltaTime, yEnd, 5);
                    ropeList[i].transform.rotation = Quaternion.Euler(0,0,90);
                }

            }else if(ropeList[i].transform.rotation.eulerAngles.z == 180) {
                ropeList[i].transform.Translate(new Vector3(0, -dist, 0) * Time.deltaTime, Space.World);

                if (ropeList[i].transform.position.y + ropeHeight / 2 > yEnd) {
                    ropeList[i].transform.position = new Vector3(ropeList[i - 1].transform.position.x + ropeHeight, yEnd, 5);
                    ropeList[i].transform.rotation = Quaternion.Euler(0, 0, 90);
                }

            } else if(ropeList[i].transform.rotation.eulerAngles.z == 90) {
                ropeList[i].transform.Translate(new Vector3(dist, 0, 0) * Time.deltaTime, Space.World);

                if (ropeList[i].transform.position.x + ropeHeight / 2 > xEnd) {
                    ropeList[i].transform.position = new Vector3(xEnd, ropeList[i + 1].transform.position.y + ropeHeight - dist * Time.deltaTime, 5);
                    ropeList[i].transform.rotation = Quaternion.Euler(0, 0, 180);
                }

                if (ropeList[i].transform.position.x - ropeHeight / 2 < xStart) {
                    ropeList[i].transform.position = new Vector3(xStart, ropeList[i - 1].transform.position.y + ropeHeight, 5);
                    ropeList[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        } 
    }

    //This function will calculate the differences and adjust the balances
    void adjustBalances() {

        balanceUpdated = false;
        var prevWeight = 0f;

        if (turns > 0) {
            prevWeight = weightDifference;

            if (currBox.tag == "balance1") {
                balanceWeights[0] += calculateWeight(currBox.GetComponent<BoxCollider2D>().bounds.size.x * 10);
            } else if (currBox.tag == "balance2") {
                balanceWeights[1] += calculateWeight(currBox.GetComponent<BoxCollider2D>().bounds.size.x * 10);
            }
        }

        weightDifference = (balanceWeights[0] - balanceWeights[1])/balanceFriction;

        if (weightDifference < -balanceLimit)
        {
            weightDifference = -balanceLimit;
        }
        if (weightDifference > balanceLimit)
        {
            weightDifference = balanceLimit;
        }

        var displacement = prevWeight - weightDifference;

        if (displacement > 0) {
            balanceMovingSpeed = -Mathf.Abs(displacement / balanceVel);
        }else if (displacement < 0) {
            balanceMovingSpeed = Mathf.Abs(displacement / balanceVel);
        } else {
            balanceUpdated = true;
            clicked = false;
            //also check level
        }

        weightText.GetComponent<Text>().text = ""+Mathf.Abs(Mathf.Floor(weightDifference*weightExpander));

    }

    void checkLevelCompleted() {
        if(turns < maxTurns) {
            if(Mathf.Abs(Mathf.Floor(weightDifference * weightExpander)) <= maxDiff) {
                if(LevelData.round == 3) {
                    //Level Completed
                    ++LevelData.level;
                    LevelData.round = 1;
                    showFeedBack(true);
                    levelRestart = true;
                } else {
                    //Round Completed
                    ++LevelData.round;
                    showFeedBack(true);
                    roundRestart = true;
                }
            }
        }else {
            if(Mathf.Abs(Mathf.Floor(weightDifference * weightExpander)) <= maxDiff) {
                if (LevelData.round == 3) {
                    //Level Completed
                    ++LevelData.level;
                    LevelData.round = 1;
                    showFeedBack(true);
                    levelRestart = true;
                } else {
                    //Round Completed
                    ++LevelData.round;
                    showFeedBack(true);
                    roundRestart = true;
                }

            } else {
                //Round Failed
                LevelData.round = 1;
                showFeedBack(false);
                levelRestart = true;
            }
        }
    }

    void showFeedBack(bool won) {
        
        foreach(var item in overlay) {
            if(item.name == "Overlay") {
                item.GetComponent<Image>().color = new Color32(51, 51, 51, 180);
            }
            else {
                item.GetComponent<Image>().color = new Color32(255,255,255,255);
            }
        
        }

        GameObject.Find("TargetText").GetComponent<Text>().color = new Color32(50, 50, 50, 255);
        GameObject.Find("DifferenceText").GetComponent<Text>().color = new Color32(50, 50, 50, 255);



        if (won) {
            GameObject.Find("WinText").GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        } else {
            GameObject.Find("GameOverText").GetComponent<Text>().color = new Color32(255, 255, 255, 255);
        }
    }

    float calculateWeight(float r) {
        return (4 / 3) * 3.14f * r * r;
    }
}
