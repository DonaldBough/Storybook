using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCameraController : MonoBehaviour {
	public GameObject girl;
	public GameObject hunter;
	public GameObject wolf;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowScene3() {
		wolf.SetActive (true);
		hunter.SetActive (false);
		girl.SetActive (false);
	}

	public void ShowScene5() {
		wolf.SetActive (false);
		hunter.SetActive (false);
		girl.SetActive (true);
	}
}
