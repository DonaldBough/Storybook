using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetGenerator : MonoBehaviour {
	StoryData storyData;

	// Use this for initialization
	void Start () {
		StartCoroutine(StartOnLoad ());
	}

	IEnumerator StartOnLoad() {
		storyData = GetComponent<StoryData> ();
		yield return new WaitUntil (() => storyData.isLoaded);
		StartCoroutine(parseStoryData ());
	}

	IEnumerator parseStoryData() {
		Debug.Log ("Parsing story data");
		TimeLine timeLine = storyData.timeLine;
		foreach (Paragraph p in timeLine.paragraphs) {
			parseParagraph (p);
			yield return PlayClip (p.narration);
		}
	}

	private void parseParagraph(Paragraph p) {
		foreach (string keyword in p.keyWords) {
			Debug.Log ("keyword: " + keyword);

		}
		foreach (Entity e in p.entities) {
			Debug.Log ("entity: " + e.text + ", " + e.type);
		}
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

	// Update is called once per frame
	void Update () {
		
	}
}
