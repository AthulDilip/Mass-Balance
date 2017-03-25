using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    // Use this for initialization
    public GameObject box;
    public GameObject currBox;
    public Rigidbody2D currBoxBody;
    public bool clicked = false;
    public bool balanceUpdated = true;
    public bool growBall = false;
    public float balanceVel = 1f;
    public float ballGrowingSpeed = 0.5f;
    public float balanceMovingSpeed = 0;
    public int weightDifference = 0;

	void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetMouseButtonDown(0)) {

            if (!clicked)
            {
                growBall = true;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                currBox = Instantiate(box, ray.origin, Quaternion.identity);
                //ob.transform.Translate(new Vector3(2,0,transform.position.z));
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (!clicked) {
                
                clicked = true;
                growBall = false;

                currBox.AddComponent<Rigidbody2D>();
            }
            
        }

        if(currBox != null && !growBall) {


            currBoxBody = currBox.GetComponent<Rigidbody2D>();

            if (currBoxBody.position.y < -6) {
                currBoxBody.Sleep();
            }

            if(currBoxBody.IsSleeping()) {

                //update the balance
                adjustBalance();

                //Remove the rigid body property of the box
                Destroy(currBoxBody);
                currBox.tag = "ground";
                currBox = null;
            }
        }

        if (growBall == true) {
            if (currBox != null && currBox.transform.localScale.x < 3) {
                currBox.transform.localScale += new Vector3(ballGrowingSpeed,ballGrowingSpeed,0)*Time.deltaTime;
            }
        }

        if(!balanceUpdated) {

            var ground = GameObject.Find("ground");
            
            if (ground.transform.position.y > weightDifference) {
                
                var grounds = GameObject.FindGameObjectsWithTag("ground");

                for (int i = 0; i < grounds.Length; ++i)
                {
                    grounds[i].transform.Translate(new Vector3(0,-balanceMovingSpeed , 0) * Time.deltaTime, Space.World);
                }
            }else {
                balanceUpdated = true;
                clicked = false;
            }
          
        }
	}

    void  adjustBalance() {
        balanceUpdated = false;
        var prevWeight = weightDifference;
        weightDifference = weightDifference - 1;
        
        if(weightDifference < -4) {
            weightDifference = -4;
        }
        if (weightDifference > 4) {
            weightDifference = 4;
        }

        var difference = prevWeight - weightDifference;

        balanceMovingSpeed = difference / balanceVel;

    }
}
