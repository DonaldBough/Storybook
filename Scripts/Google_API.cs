using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;

public class Google_API : MonoBehaviour {
	public string nativePath;
	public string targetPath;
	string storyLang;
	string targetLang;
	string nativeStory;
	public string[] lines;
	StreamWriter targetStory;

	void Start(){
		Debug.Log ("Started");
		fillFakeURLParams();
//		translates story in native language to target language, targetStory
		StartCoroutine(translateStory());
	}

	IEnumerator translateStory() {
		nativePath = "Assets/story.txt";
		targetPath = "Assets/target_story.txt";
		lines = System.IO.File.ReadAllLines(nativePath);
		List<string> apiChunks = GetChunksFromStory (2);
		targetStory = new StreamWriter(targetPath, false);

		//Send chunks of the story that the API can handle
		foreach (string chunk in apiChunks) {
			string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + storyLang + "&tl=" + targetLang + "&dt=t&q=" + chunk;
			yield return StartCoroutine (callTranslateApi (url));
		}
		targetStory.Close ();
		Debug.Log ("Story Translated!");
		GetComponent<IBM_API> ().getKeywords ();
	}

	IEnumerator callTranslateApi(string uri)
	{
		UnityWebRequest uwr = UnityWebRequest.Get(uri);
		yield return uwr.Send();

		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			string text = uwr.downloadHandler.text;
			string[] res = text.Split ('[', ']');
			for (int i = 3; i < res.Length; i += 2) {
				string sentence = extractSentence (res [i]);
				if (sentence != null) {
					targetStory.WriteLine (sentence);
//					Debug.Log ("sentence: " + sentence);
				}
			}
		}
	}

	//Get linesPerChunk lines at a time so the api can send
	//multiple lines from the story at once
	private List<string> GetChunksFromStory(int linesPerChunk) {
		int counter = 0;
		string chunk = "";
		List<string> list = new List<string> ();
		foreach (string line in lines) {
//			string line = stripQuoteParts (l);
			if (counter < linesPerChunk - 1) {
				chunk += line;
				counter++;
				continue;
			}
			chunk += line;
			list.Add (chunk);
			counter = 0;
			chunk = "";
		}
		if (chunk != null && chunk.Length != 0) {
			list.Add (chunk);
		}
		return list;
	}

	string stripQuoteParts(string line) {
		Debug.Log (line);
		try {
			while (line.Contains("\"")) {
				int firstQ = line.IndexOf ("\"");
				string sub = line.Substring(0, firstQ);
				string lineAfterQuote = line.Substring(firstQ + 1, line.Length - firstQ - 1);
				int secondQ = lineAfterQuote.IndexOf ("\"");
				if (secondQ == -1)
					break;
				string sub2 = lineAfterQuote.Substring(secondQ + 1, lineAfterQuote.Length - secondQ - 1);
				string final = sub + sub2;
				return final;
			}
		} catch  {
			return line;
		}
		return line;
	}

	string extractSentence(string apiString) {
		if (apiString == null || apiString.Length == 0 || apiString [0] != '"')
			return null;
		for (int i = 1; i < apiString.Length; i++) {
			if (apiString [i] == '\\') {
				i++;
			} else if (apiString[i] == '"') {
				string sentence = apiString.Substring (1, i - 1); 
				if (sentence.ToLower ().Contains ("null") || sentence.Length == 2 || sentence.Equals ("es"))
					return null;
				return sentence;
			}
		}
		return null;
	}

	void fillFakeURLParams() {
		this.storyLang = "auto";
		this.targetLang = "en";
	}
}