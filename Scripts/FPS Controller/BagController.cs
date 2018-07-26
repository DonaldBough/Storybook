using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour {
	Vector3 bagStartPosition;
	Quaternion bagStartRotation;
	bool didSlide;
	bool wasThrown;
	bool respawnedBag;
	double speed;
	System.DateTime thrown;

	public GameController gameController;
	public float slickness;
	public double respawnTime;
	public ParticleSystem explosion;

	// Use this for initialization
	void Start () {
		bagStartPosition = this.gameObject.transform.position;
		bagStartRotation = this.gameObject.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if ( wasThrown && (System.DateTime.Now - thrown).TotalSeconds > respawnTime) {
			ReturnBag ();
		}
	}

	public void InitGameObject(GameController gameController) {
		this.gameController = gameController;
	}

	public void SetThrownTime() {
		wasThrown = true;
		thrown = System.DateTime.Now;
	}

	public void SetThrownSpeed(double speed) {
		this.speed = speed;
	}


//	void OnCollisionEnter(Collision col) {
//	}

	void OnTriggerEnter(Collider col) {
		GameObject o = col.gameObject;
		if (o.tag.Contains("boardHole") || o.tag == "target") {
			Debug.Log ("Hit the target");
			explosion.transform.position = o.transform.position;
			explosion.Play ();

			if (o.tag == "boardHole2Pt")
				this.gameController.GetComponent<GameController> ().addToScore (2);
			else if (o.tag == "boardHole4Pt")
				this.gameController.GetComponent<GameController> ().addToScore (4);
			else if (o.tag == "target")
				this.gameController.GetComponent<GameController> ().addToScore (1);
			else
				Debug.Log ("HIT TARGET w/UNKNOWN POINT VALUE");

			if (!respawnedBag)
				ReturnBag ();
//			this.gameObject.SetActive (false);
			Destroy(this.gameObject);
		}
		else if (o.tag == "board") {
			Debug.Log ("Hit the board");
			if (!didSlide) {
				didSlide = true;
				speed = (speed) * slickness;
				this.gameObject.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * (float) (-slickness));
			}
		}
	}

	void ReturnBag() {
		Debug.Log ("Spawned new bag");
		respawnedBag = true;
		Instantiate(gameObject, bagStartPosition, bagStartRotation);
		enabled = false;
	}
}
