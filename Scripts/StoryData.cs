using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class StoryData : MonoBehaviour {
	public List<Keyword> keyWords;
	public List<Entity> entities;
	public TimeLine timeLine;
	public bool isLoaded;

	// Use this for initialization
	void Start () {
		keyWords = new List<Keyword> ();
		entities = new List<Entity> ();
		timeLine = new TimeLine ();
		timeLine.paragraphs = new List<Paragraph> ();
	}

	public void addParagraph(List<string> keyWords, List<Entity> entities) {
		Paragraph p = new Paragraph ();
		p.keyWords = keyWords;
		p.entities = entities;
		timeLine.paragraphs.Add (p);
	}

	public void addAudioClips(AudioClip[] audioClips) {
		int counter = 0;
		Debug.Log ("Adding clips");
		foreach (Paragraph p in timeLine.paragraphs) {
			p.narration = audioClips [counter];
			counter++;
		}
	}

	public void addDebugText(string[] debugText) {
		int counter = 0;
		foreach (Paragraph p in timeLine.paragraphs) {
			p.debugText = debugText [counter];
			counter++;
		}
	}

	public void doneLoading() {
		GetComponent<Controller> ().AfterLoadingStart ();
		WriteData();
	}

	public void WriteData() {
		Debug.Log ("writing data");
		List<Paragraph> paragraphs = GetComponent<StoryData> ().timeLine.paragraphs;
		StreamWriter data = new StreamWriter("Assets/ML_data.txt", false);
		int counter = 1;

		data.WriteLine ("--- Keywords ---");
		foreach (Paragraph p in paragraphs) {
			data.WriteLine ("* * Part of story " + counter);
			foreach (string keyword in p.keyWords) {
				data.WriteLine (keyword);
			}
			data.WriteLine ("");
			counter++;
		}
		counter = 1;
		data.WriteLine ("--- Entities ---");
		foreach (Paragraph p in paragraphs) {
			data.WriteLine ("* * Part of story " + counter);
			foreach (Entity e in p.entities) {
				data.WriteLine (e.text + ", " + e.type);
			}
			data.WriteLine ("");
			counter++;
		}
		data.Close ();
	}
}

public class TimeLine {
	public List<Paragraph> paragraphs;
}

public class Paragraph {
	public List<string> keyWords;
	public List<Entity> entities;
	public AudioClip narration;
	public string debugText;
}