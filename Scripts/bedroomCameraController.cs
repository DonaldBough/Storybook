using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bedroomCameraController : MonoBehaviour {
	public GameObject hunter;
	public GameObject girl;
	public GameObject wolf;
	public GameObject bedWolf;
	public GameObject grandma;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowScene4() {
		hunter.SetActive (false);
		girl.SetActive (false);
		wolf.SetActive (true);
		bedWolf.SetActive (false);
		grandma.SetActive (true);
	}

	public void ShowScene6() {
		hunter.SetActive (false);
		girl.SetActive (true);
		wolf.SetActive (false);
		bedWolf.SetActive (true);
		grandma.SetActive (false);
	}

	public void ShowScene7() {
		hunter.SetActive (true);
		girl.SetActive (false);
		wolf.SetActive (false);
		bedWolf.SetActive (true);
		grandma.SetActive (false);
	}

	public void ShowScene8() {
		hunter.SetActive (true);
		girl.SetActive (true);
		wolf.SetActive (true);
		bedWolf.SetActive (false);
		grandma.SetActive (true);
	}

	public void ShowScene9() {
		hunter.SetActive (false);
		girl.SetActive (true);
		wolf.SetActive (false);
		bedWolf.SetActive (false);
		grandma.SetActive (true);
	}
}
