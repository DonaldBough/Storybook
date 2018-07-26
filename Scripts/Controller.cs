using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public GameObject kitchenCamera;
	public GameObject bedroomCamera;
	public GameObject houseCamera;
	public GameObject forestCamera;
	Camera kitchen_cam;
	Camera bed_cam;
	Camera house_cam;
	Camera forest_cam;

	void Start() {
//		Debug.Log ("starting in Controller, not Google_API");
//		AfterLoadingStart ();
	}

	public void AfterLoadingStart() {
		InitCameras ();
		StartCoroutine(StartScenes ());
	}

	IEnumerator StartScenes() {
		int sceneCounter = 1;
		TimeLine timeLine = GetComponent<StoryData> ().timeLine;

		foreach (Paragraph p in timeLine.paragraphs) {
			switch (sceneCounter)
			{
			case 1:
				ShowScene1 ();
				break;
			case 2:
				ShowScene2 ();
				break;
			case 3:
				break;
			case 4:
				ShowScene3 ();
				break;
			case 5:
				ShowScene4 ();
				break;
			case 6:
				ShowScene6 ();
				break;
			case 7:
				ShowScene7 ();
				break;
			case 8:
				ShowScene8 ();
				break;
			case 9:
				ShowScene9 ();
				break;
			default:
				break;
			}
			yield return new WaitForSeconds (1); //let scene load
			Debug.Log (p.debugText);
			yield return PlayClip (p.narration);
			Debug.Log ("Counter: " + sceneCounter);
			yield return new WaitForSeconds (1);
			sceneCounter++;
		}
		/*
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 1");
		ShowScene1 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 2");
		ShowScene2 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 3");
		ShowScene3 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 4");
		ShowScene4 ();
		yield return new WaitForSeconds (1);
//		Debug.Log ("Showing scene 5");
//		ShowScene5 ();
//		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 6");
		ShowScene6 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 7");
		ShowScene7 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 8");
		ShowScene8 ();
		yield return new WaitForSeconds (1);
		Debug.Log ("Showing scene 9");
		ShowScene9 ();
		yield return new WaitForSeconds (1);
		*/
	}

	private void ShowScene1() {
		kitchen_cam.enabled = true;
		kitchen_cam = Camera.main;

		bed_cam.enabled = false;
		house_cam.enabled = false;
		forest_cam.enabled = false;

		kitchenCamera.GetComponent<kitchenCameraController> ().ShowScene1 ();
	}

	private void ShowScene2() {
		forest_cam.enabled = true;
		forest_cam = Camera.main;

		bed_cam.enabled = false;
		house_cam.enabled = false;

		forestCamera.GetComponent<ForestCameraController> ().ShowScene2 ();
	}

	private void ShowScene3() {
		house_cam.enabled = true;
		house_cam = Camera.main;

		bed_cam.enabled = false;

		houseCamera.GetComponent<HouseCameraController> ().ShowScene3 ();
	}

	private void ShowScene4() {
		bed_cam.enabled = true;
		bed_cam = Camera.main;

		bedroomCamera.GetComponent<bedroomCameraController> ().ShowScene4 ();
	}

	private void ShowScene5() {
		InitCameras ();
		house_cam.enabled = true;
		house_cam = Camera.main;

//		house_cam.enabled = false;
		bed_cam.enabled = false;
		forest_cam.enabled = false;
		kitchen_cam.enabled = false;

		houseCamera.GetComponent<HouseCameraController> ().ShowScene5 ();
	}

	private void ShowScene6() {
		InitCameras ();
		bed_cam.enabled = true;
		bed_cam = Camera.main;

		house_cam.enabled = false;

		bedroomCamera.GetComponent<bedroomCameraController> ().ShowScene6 ();
	}

	private void ShowScene7() {
		bedroomCamera.GetComponent<bedroomCameraController> ().ShowScene7 ();
	}

	private void ShowScene8() {
		bedroomCamera.GetComponent<bedroomCameraController> ().ShowScene8 ();
	}

	private void ShowScene9() {
		bedroomCamera.GetComponent<bedroomCameraController> ().ShowScene9 ();
	}

	IEnumerator PlayClip(AudioClip clip) {
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();
			yield return new WaitWhile (()=> source.isPlaying);
			Destroy(audioObject, clip.length);
		}
	}

	void InitCameras() {
		kitchen_cam = kitchenCamera.GetComponent<Camera> ();
		bed_cam = bedroomCamera.GetComponent<Camera> ();
		house_cam = houseCamera.GetComponent<Camera> ();
		forest_cam = forestCamera.GetComponent<Camera> ();
	}
}
