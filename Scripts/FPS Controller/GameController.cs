using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public GameObject bag;
	public GameObject boardText;
	public GameObject countDownText;
	public GameObject cow;
	public GameObject cow2;
	public float gameLength;
	public List<string> targetsToHide;
    public GameObject target1;
    public GameObject target2;
    public GameObject backMusic;
    public GameObject yeeHaw;
    public GameObject moo;
    public int cowRespawnTime;

    Vector3 cowStartPos;
	Vector3 cow2StartPos;
	Quaternion cowStartRot;
	Quaternion cow2StartRot;
    Vector3 target1StartPos;
    Vector3 target2StartPos;
	float gameScore;
	float secondsRemaining;
	bool gameIsOver;
	bool cowHasBeenThrown;
	bool cow2HasBeenThrown;
    bool movingUp;
	System.DateTime cowThrowTime;
	System.DateTime cow2ThrowTime;

	// Use this for initialization
	void Start () {
		boardText.GetComponent<TextMesh>().text = "Score: 0";
		countDownText.GetComponent<TextMesh> ().text = "Time Remaining: " + gameLength.ToString ();
		secondsRemaining = gameLength; 

		foreach (string tag in targetsToHide) {
			GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);
			foreach (GameObject t in targets) {
				t.GetComponent<Renderer>().enabled = false; //hide target objects
			}
		}
		InvokeRepeating("updateCountdown",(float) 0, 1);

        cowStartPos = cow.transform.position;
		cowStartRot = cow.transform.rotation;
		cow2StartPos = cow2.transform.position;
		cow2StartRot = cow2.transform.rotation;

        backMusic.GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
        target1.transform.Rotate(Vector3.up * (120 * Time.deltaTime));
        target2.transform.Rotate(Vector3.up * (120 * Time.deltaTime));

        if (Input.GetKeyDown (KeyCode.Q)) {
			if ((System.DateTime.Now - cowThrowTime).TotalSeconds > cowRespawnTime) {
				throwCow (1);
			}
		}
		else if (cowHasBeenThrown && (System.DateTime.Now - cowThrowTime).TotalSeconds >= cowRespawnTime) {
			cowHasBeenThrown = false;
			returnCow (1);
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			if ((System.DateTime.Now - cow2ThrowTime).TotalSeconds > cowRespawnTime) {
				throwCow (2);
			}
		}
		else if (cow2HasBeenThrown && (System.DateTime.Now - cow2ThrowTime).TotalSeconds >= cowRespawnTime) {
			cow2HasBeenThrown = false;
			returnCow (2);
		}
	}

	void Awake ()
	{
		SetGameControllerReferences();
	}

	void SetGameControllerReferences () {
		this.bag.GetComponent<BagController> ().InitGameObject (this);
	}

	void updateCountdown() {
		if (secondsRemaining == 0) {
			gameIsOver = true;
			countDownText.GetComponent<TextMesh> ().text = "Game Over!";
		} else {
			secondsRemaining--;
			countDownText.GetComponent<TextMesh> ().text = "Time Remaining: " + secondsRemaining.ToString ();
		}
	}

	void throwCow(int cowNum) {
        moo.GetComponent<AudioSource>().Play();
		if (cowNum == 1) {
			cowHasBeenThrown = true;
			cow.GetComponent<Rigidbody> ().AddForce(cow.transform.forward * (float) 700);
			cow.GetComponent<Rigidbody> ().AddForce(cow.transform.up * (float) 400);
			cowThrowTime = System.DateTime.Now;
		} else if (cowNum == 2) {
			cow2HasBeenThrown = true;
			cow2.GetComponent<Rigidbody> ().AddForce(cow2.transform.forward * (float) 700);
			cow2.GetComponent<Rigidbody> ().AddForce(cow2.transform.up * (float) 400);
			cow2ThrowTime = System.DateTime.Now;
		}
	}

	void returnCow(int cowNum) {
		if (cowNum == 1) {
			cow.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			cow.transform.position = cowStartPos;
			cow.transform.rotation = cowStartRot;
		} else if (cowNum == 2) {
			cow2.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			cow2.transform.position = cow2StartPos;
			cow2.transform.rotation = cow2StartRot;
		}
	}

    void moveTarget()
    {
        movingUp = !movingUp;
        if (movingUp)
        {
            target1.transform.position = Vector3.Lerp(target1.transform.position,
                new Vector3(target1.transform.position.x, target1.transform.position.y + 15,
                    target1.transform.position.z), 5);
        } else
        {

        }
    }


    public void addToScore(int points) {
		if (!gameIsOver) {
            yeeHaw.GetComponent<AudioSource>().Play();
			gameScore += points;
			boardText.GetComponent<TextMesh> ().text = "Score: " + gameScore;
		}
	}
}
